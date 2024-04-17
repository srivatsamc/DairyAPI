namespace Dairy.API.Models.Login;

public class LoginRequest
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Contact { get; set; }
    public string? Address { get; set; }
    public int RoleId { get; set; } = 2;
}
