namespace CakeStore.Models.Dtos;

public class ReviewDto
{
    public int CakeId { get; set; }
    public int Rating { get; set; } // 1–5
    public string Comment { get; set; } = "";
}
