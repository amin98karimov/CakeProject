namespace CakeStore.Models.Dtos;

public class CreateCakeDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public List<CakePropertyDto> Properties { get; set; } = new();
}