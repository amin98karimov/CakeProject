namespace CakeStore.Models.Dtos;

public class CakeDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public List<CakePropertyDto> Properties { get; set; } = new();
}
