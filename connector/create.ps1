#####################################################
# HelloID-Conn-Prov-Target-ORION.M2-Create
#
# Version: 1.0.0
#####################################################
# Initialize default value's
$config = $configuration | ConvertFrom-Json
$p = $person | ConvertFrom-Json
$success = $false
$auditLogs = [System.Collections.Generic.List[PSCustomObject]]::new()

# Account mapping
$account = [PSCustomObject]@{
    externalId   = $p.ExternalId
    givenName    = $p.Name.GivenName
    familyName   = $p.Name.FamilyName
    userName     = $p.ExternalId
    initials     = $p.Name.Initials
    isActive     = $true
    emailAddress = $p.Contact.Business.Email
    description  = "Developer Test"
}

# Enable TLS1.2
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor [System.Net.SecurityProtocolType]::Tls12

# Set debug logging
switch ($($config.IsDebug)) {
    $true { $VerbosePreference = 'Continue' }
    $false { $VerbosePreference = 'SilentlyContinue' }
}

# Set update switch
$updatePerson = $false

#region functions
function Resolve-HTTPError {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory,
            ValueFromPipeline
        )]
        [object]$ErrorObject
    )
    process {
        $httpErrorObj = [PSCustomObject]@{
            FullyQualifiedErrorId = $ErrorObject.FullyQualifiedErrorId
            MyCommand             = $ErrorObject.InvocationInfo.MyCommand
            RequestUri            = $ErrorObject.TargetObject.RequestUri
            ScriptStackTrace      = $ErrorObject.ScriptStackTrace
            ErrorMessage          = ''
        }
        if ($ErrorObject.Exception.GetType().FullName -eq 'Microsoft.PowerShell.Commands.HttpResponseException') {
            $httpErrorObj.ErrorMessage = $ErrorObject.ErrorDetails.Message
        } elseif ($ErrorObject.Exception.GetType().FullName -eq 'System.Net.WebException') {
            $httpErrorObj.ErrorMessage = [System.IO.StreamReader]::new($ErrorObject.Exception.Response.GetResponseStream()).ReadToEnd()
        }
        Write-Output $httpErrorObj
    }
}
#endregion

# Begin
try {
    # Verify if a user must be created or correlated
    $response = Invoke-RestMethod -Uri "$($config.BaseUrl)/api/Person" -Method 'GET'
    $lookupAccount = $response | Group-Object -AsHashTable -Property 'externalId' -AsString
    $userAccount = $lookupAccount[$account.externalId]
    if (!$userAccount){
        $action = 'Create-Correlate'
    } elseif ($updatePerson -eq $true) {
        $action = 'Update-Correlate'
    } else {
        $action = 'Correlate'
    }

    # Add an auditMessage showing what will happen during enforcement
    if ($dryRun -eq $true){
        $auditLogs.Add([PSCustomObject]@{
            Message = "$action ORION.M2 account for: [$($p.DisplayName)], will be executed during enforcement"
        })
    }

    # Process
    if (-not($dryRun -eq $true)){
        switch ($action) {
            'Create-Correlate' {
                Write-Verbose "Creating ORION.M2 account"
                $splatCreatePersonParams = @{
                    Uri         = "$($config.BaseUrl)/api/Person"
                    Method      = 'POST'
                    ContentType = 'application/json'
                    Body        = $account | ConvertTo-Json -Depth 10
                }
                $personCreateResponse = Invoke-RestMethod @splatCreatePersonParams
                $accountReference = $personCreateResponse.id
                break
            }

            'Update-Correlate'{
                Write-Verbose "Correlating and updating ORION.M2 account"
                $accountReference = $person.id
                break
            }

            'Correlate'{
                Write-Verbose "Correlating ORION.M2 account"
                $accountReference = $person.id
                break
            }
        }

        $success = $true
        $auditLogs.Add([PSCustomObject]@{
            Message = "$action account was successful. AccountReference is: [$accountReference]"
            IsError = $false
        })
    }
} catch {
    $success = $false
    $ex = $PSItem
    if ($($ex.Exception.GetType().FullName -eq 'Microsoft.PowerShell.Commands.HttpResponseException') -or
        $($ex.Exception.GetType().FullName -eq 'System.Net.WebException')) {
        $errorObj = Resolve-HTTPError -ErrorObject $ex
        $errorMessage = "Could not $action ORION.M2 account. Error: $($errorObj.ErrorMessage)"
    } else {
        $errorMessage = "Could not $action ORION.M2 account. Error: $($ex.Exception.Message)"
    }
    Write-Verbose $errorMessage
    $auditLogs.Add([PSCustomObject]@{
        Message = $errorMessage
        IsError = $true
    })
# End
} finally {
   $result = [PSCustomObject]@{
        Success          = $success
        AccountReference = $accountReference
        Auditlogs        = $auditLogs
        Account          = $account
    }
    Write-Output $result | ConvertTo-Json -Depth 10
}
