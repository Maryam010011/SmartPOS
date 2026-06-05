namespace SmartPOS.Shared.DTOs.Users;

public class UserFilterDto
{
    public int? RoleId { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
