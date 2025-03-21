namespace ClassesLib;

public class UserProduct
{
    public int Id { get; set; }    
    public string ProductName { get; set; } = "";

    public UserProduct(){}

    public UserProduct(int id, string productName)
    {
        Id = id;        
        ProductName = productName;
    }

    public override string ToString() => ProductName;
}
