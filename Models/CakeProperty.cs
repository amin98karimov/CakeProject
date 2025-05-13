namespace CakeStore.Models;

public class CakeProperty
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";

    public int CakeId { get; set; }
    public Cake Cake { get; set; } = null!;
}
