namespace ORION.M2.DbContext.Models;

public class Permission
{
    public int Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? MemberUserName { get; set; }
}