namespace CakeStore.Models;

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";

    public User User { get; set; } = null!;
}
