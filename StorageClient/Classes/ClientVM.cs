using ClassesLib;
using StorageDBCodeFirst.Classes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StorageClient.Classes;

public class ClientVM : INotifyPropertyChanged
{
    public MyUser user;
    public MyUser User
    {
        get => user;
        set
        {
            user = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
        }
    }
    public ObservableCollection<UserProduct> UserProducts { get; set; } = [];
    public ObservableCollection<UserStorage> Storages { get; set; } = [];
    public ObservableCollection<string> ProductParty { get; set; } = [];
    public ObservableCollection<Location> Locations { get; set; } = []; // ??? 
    public ObservableCollection<MyDocument> Documents { get; set; } = [];

    public ObservableCollection<ProductAcceptance> ProductAcceptances { get; set; } = [];
    public ObservableCollection<ProductConsumption> ProductConsumptions { get; set; } = [];
    public AddDelRack AddDelRack { get; set; } = new();

    public ObservableCollection<int> ConsumptionRackNumbers { get; set; } = [];

    public List<Product> Products { get; set; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;
}
