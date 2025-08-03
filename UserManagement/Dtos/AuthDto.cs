namespace UserManagement.Dtos;

public class LoginParam
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginResponse
{
    public required string Token { get; set; }
}