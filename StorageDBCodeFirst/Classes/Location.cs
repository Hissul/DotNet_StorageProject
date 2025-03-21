namespace StorageDBCodeFirst.Classes;

public class Location
{
    public int Id { get; set; }
    public int RackNumber { get; set; }

    public Location(){}

    public Location(int id, int rackNumber)
    {
        Id = id;
        RackNumber = rackNumber;
    }

    public override string ToString() => $"{RackNumber}";
}
