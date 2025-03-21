using System.ComponentModel;

namespace StorageClient.Classes;

public class Rack : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string rackNumber = "";
    public string RackNumber
    {
        get => rackNumber;
        set
        {
            rackNumber = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RackNumber)));
        }
    }

    public string addRack = "";
    public string AddRack
    {
        get => addRack;
        set
        {
            addRack = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddRack)));
        }
    }

    public string deleteRack = "";
    public string DeleteRack
    {
        get => deleteRack;
        set
        {
            deleteRack = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeleteRack)));
        }
    }
}
