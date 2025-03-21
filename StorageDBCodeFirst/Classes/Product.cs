namespace StorageDBCodeFirst.Classes;

public class Product
{
    public int Id { get; set; }
    public string Party { get; set; } = "";

    public int ProductTypeId { get; set; }
    public ProductType ProductType { get; set; } = null!;     
}
