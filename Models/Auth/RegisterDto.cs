namespace CakeStore.Models.Auth;

public class RegisterDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string UserType { get; set; } = "Customer"; // or Baker
}