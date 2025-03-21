using Microsoft.Identity.Client.Extensions.Msal;
using StorageDBCodeFirst.Classes;
using Storage = StorageDBCodeFirst.Classes.Storage;


namespace ClassesLib;

public class ProductAndStorage
{
    public List<ProductType> ProductTypes { get; set; } = [];    
    public List<Storage> Storages { get; set; } = [];
}
