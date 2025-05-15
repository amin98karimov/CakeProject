namespace CakeStore.Models.Dtos;

public class ReviewDto
{
    public int CakeId { get; set; }
    public int Rating { get; set; } 
    public string Comment { get; set; } = "";
}
