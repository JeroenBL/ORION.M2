#####################################################
# HelloID-Conn-Prov-Target-ORION.M2-Enable
#
# Version: 1.0.0
#####################################################
# Initialize default value's
$config = $configuration | ConvertFrom-Json
$p = $person | ConvertFrom-Json
$aRef = $AccountReference | ConvertFrom-Json
$success = $false
$auditLogs = [System.Collections.Generic.List[PSCustomObject]]::new()

# Enable TLS1.2
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor [System.Net.SecurityProtocolType]::Tls12

# Set debug logging
switch ($($config.IsDebug)) {
    $true { $VerbosePreference = 'Continue' }
    $false { $VerbosePreference = 'SilentlyContinue' }
}

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

try {
    # Add an auditMessage showing what will happen during enforcement
    if ($dryRun -eq $true){
        $auditLogs.Add([PSCustomObject]@{
            Message = "Enable ORION.M2 account for: [$($p.DisplayName)], will be executed during enforcement"
        })
    }

    if (-not($dryRun -eq $true)) {
        Write-Verbose "Enabling ORION.M2 account: [$aRef]"
        $splatEnablePersonParams = @{
            Uri         = "$($config.BaseUrl)/api/Person/$aRef"
            Method      = 'PATCH'
            ContentType = 'application/json'
            Body        = @"
            [
                {
                    "operationType": "replace",
                    "path": "isActive",
                    "value": "true"
                }
            ]
"@
        }
        $personEnableResponse = Invoke-WebRequest @splatEnablePersonParams -UseBasicParsing
        if ($personEnableResponse.StatusCode -eq 200){
            $success = $true
            $auditLogs.Add([PSCustomObject]@{
                Message = "Enable account was successful."
                IsError = $false
            })
        }
    }
} catch {
    $success = $false
    $ex = $PSItem
    if ($($ex.Exception.GetType().FullName -eq 'Microsoft.PowerShell.Commands.HttpResponseException') -or
        $($ex.Exception.GetType().FullName -eq 'System.Net.WebException')) {
        $errorObj = Resolve-HTTPError -ErrorObject $ex
        $errorMessage = "Could not enable ORION.M2 account. Error: $($errorObj.ErrorMessage)"
    } else {
        $errorMessage = "Could not enable ORION.M2 account. Error: $($ex.Exception.Message)"
    }
    Write-Verbose $errorMessage
    $auditLogs.Add([PSCustomObject]@{
        Message = $errorMessage
        IsError = $true
    })
} finally {
    $result = [PSCustomObject]@{
        Account   = $personEnableResponse.Content | ConvertFrom-Json
        Success   = $success
        Auditlogs = $auditLogs
    }
    Write-Output $result | ConvertTo-Json -Depth 10
}
