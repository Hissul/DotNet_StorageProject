namespace ClassesLib;

public class UserStorage
{
    public int Id { get; set; }
    public string Party { get; set; } = "";
    public string ProductName { get; set; } = "";
    public int RackNumber { get; set; }
    public int Amount { get; set; }

    public UserStorage(){}

    public UserStorage(int id, string party, string productName, int rackNumber, int amount)
    {
        Id = id;
        Party = party;
        ProductName = productName;
        RackNumber = rackNumber;
        Amount = amount;
    }
}
