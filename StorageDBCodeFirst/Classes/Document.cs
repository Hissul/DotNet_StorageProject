namespace StorageDBCodeFirst.Classes;

public class Document
{
    public int Id { get; set; }

    public int DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = null!;

    public int ProductTypeID { get; set; }
    public ProductType ProductType { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string DocumentNumber { get; set; } = "";
    public int Amount { get; set; }
    public DateTime Date { get; set; }
    public string Party { get; set; } = "";

    public Document(){}

    public Document(int id, int documentTypeId, int productTypeID, int userId, string documentNumber, int amount, DateTime date, string party)
    {
        Id = id;
        DocumentTypeId = documentTypeId;
        ProductTypeID = productTypeID;
        UserId = userId;
        DocumentNumber = documentNumber;
        Amount = amount;
        Date = date;
        Party = party;
    }
}
