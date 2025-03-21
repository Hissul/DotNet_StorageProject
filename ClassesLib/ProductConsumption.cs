namespace ClassesLib;

public class ProductConsumption
{
    public const int DocumentTypeId = 2        ;
    public DateTime Date = DateTime.Now;
    public string DocumentNumber { get; set; } = "";
    public int ProductTypeId { get; set; }
    public string Party { get; set; } = "";
    public int Amount { get; set; }
    public int LocationId { get; set; }
    public string ProductName { get; set; } = "";
    public int RackNumber { get; set; }

    public ProductConsumption(){}

    public ProductConsumption(string documentNumber, int productTypeId, string party, int amount, int locationId, string productName, int rackNumber)
    {        
        DocumentNumber = documentNumber;
        ProductTypeId = productTypeId;
        Party = party;
        Amount = amount;
        LocationId = locationId;
        ProductName = productName;
        RackNumber = rackNumber;
    }
}
