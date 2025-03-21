namespace StorageDBCodeFirst.Classes;

public class ProductType
{
    public int Id {  get; set; }
    public string ProductName { get; set; } = "";

    public override string ToString() => ProductName;
}
