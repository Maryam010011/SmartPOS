namespace SmartPOS.Services.MaryamJ;

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RoleInfo Role { get; set; } = new();
}

public class RoleInfo
{
    public string Name { get; set; } = string.Empty;
}
