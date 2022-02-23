namespace ORION.M2.DbContext.Models;

public class Person
{
    public int Id { get; set; }
    public string ExternalId { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string UserName { get; set; }
    public string Initials { get; set; }
    public string EmailAddress { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
}