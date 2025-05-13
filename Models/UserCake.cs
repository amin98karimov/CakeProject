namespace CakeStore.Models;

public class UserCake
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CakeId { get; set; }
    public DateTime BoughtAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Cake Cake { get; set; } = null!;
}
