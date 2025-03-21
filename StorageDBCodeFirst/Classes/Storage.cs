namespace StorageDBCodeFirst.Classes;

public class Storage
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int Amount { get; set; }

    public Storage(){}

    public Storage(int id, int productId, Product product, int locationId, Location location, int amount)
    {
        Id = id;
        ProductId = productId;
        Product = product;
        LocationId = locationId;
        Location = location;
        Amount = amount;
    }
}
