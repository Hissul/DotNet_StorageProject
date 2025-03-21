namespace ClassesLib;

public class ProductAcceptance
{
    public const int DocumentTypeId = 1;
    public DateTime Date = DateTime.Now;
    public string DocumentNumber { get; set; } = "";
    public int ProductTypeId {  get; set; }
    public string Party { get; set; } = "";
    public int Amount { get; set; } 
    public int LocationId { get; set; }
    public string ProductName { get; set; } = "";

    public bool IsNew { get; set; } = false;

    public ProductAcceptance(){}

    public ProductAcceptance(string documentNumber, int productTypeId, string party, int amount, int locationId, bool isNew, string productName)
    {       
        DocumentNumber = documentNumber;
        ProductTypeId = productTypeId;
        Party = party;
        Amount = amount;
        LocationId = locationId;
        IsNew = isNew;
        ProductName = productName;
    }
}
