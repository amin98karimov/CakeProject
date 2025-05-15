namespace CakeStore.Models.Auth;

public class CakeReview
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CakeId { get; set; }
    public string Comment { get; set; } = "";
    public int Rating { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Cake Cake { get; set; } = null!;
}
