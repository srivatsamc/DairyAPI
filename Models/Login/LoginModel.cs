namespace Dairy.API.Models.Login;

public class LoginModel : LoginRequest
{
    public string? CustomerId {  get; set; }
}

public class Credentials
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
