﻿namespace CakeStore.Models;

public class Cake
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }

    public ICollection<CakeProperty> Properties { get; set; } = new List<CakeProperty>();
}
