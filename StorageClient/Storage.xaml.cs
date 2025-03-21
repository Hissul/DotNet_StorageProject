using ClassesLib;
using PrefixLib;
using Request;
using StorageClient.Classes;
using StorageDBCodeFirst.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace StorageClient;

public partial class Storage : Window
{
    TcpClient server;

    private List<Product> products = [];
    private List<Storage> storages = [];   

    private ProductAndStorage test = new ProductAndStorage();
    private ClientVM clientVM = new();

    private List<UserStorage> storageCopies = [];  // копия
    private List<MyDocument> documentsCopy = [];
    List<Location> locationCopies = [];


    public Storage(TcpClient server)
    {
        InitializeComponent();
        this.server = server;
        this.DataContext = clientVM;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {       
        //UserRequest.AskLocation
        // отправка запроса - список товаров + склад
        await server.SendInt32(UserRequest.AskProductAndStorage);        

        await ListenServer();
    }  

 
    // копия склада
    private void MakeStorageCopy(ObservableCollection<UserStorage> originals)
    {
        storageCopies.Clear();

        foreach (var original in originals)
        {
            storageCopies.Add(original);
        }
    }

    // СЛУШАЕМ СЕРВЕР
    private async Task ListenServer()
    {
        while (true)
        {
            int signal = await server.ReceiveInt32();

            switch (signal)
            {
                case ServerResponse.SendProductAndStorage:
                    await ReceiveProductAndStorage();
                    await server.SendInt32(UserRequest.AskLocation);
                    break;
                case ServerResponse.SendProductParty:
                    await ReceiveProductParty();
                    break;
                case ServerResponse.SendUserInfo:
                    await ReceiveUserInfo();
                    break;
                case ServerResponse.SendProduct:
                    await ReceiveProduct();
                    break;
                case ServerResponse.SendLocation:
                    await ReceiveLocation();
                    break;
                case ServerResponse.SendMessageAboutChanges:
                    await ReceiveMessageAboutChanges();
                    break;
                case ServerResponse.SendDocument:
                    await ReceiveDocument();
                    break;
            }            
        }
    }

    //
    private async Task ReceiveMessageAboutChanges()
    {
        string messageFromServer = await server.ReceiveString();

        if (messageFromServer == ServerAction.Saved.ToString())
        {
            MessageBox.Show($"Изменения сохранены");
        }
        else if (messageFromServer == ServerAction.NotSaved.ToString())
        {
            MessageBox.Show($"Изменения не сохранены");
        }else if(messageFromServer == ServerAction.NotDeleted.ToString())
        {
            MessageBox.Show($"\tНе удалось удалить стеллаж\n(для удаления стеллаж должен быть пустой)");
        }
    }

    //
    private async Task ReceiveProduct()
    {
        string productFromServer = await server.ReceiveString();
        var prod = JsonSerializer.Deserialize<List<Product>>(productFromServer);

        clientVM.Products.Clear();

        foreach (var pr in prod)
        {
            clientVM.Products.Add(pr);
        }
    }

    //
    private async Task ReceiveProductAndStorage()
    {
        string productFromServer = await server.ReceiveString();
        var prod = JsonSerializer.Deserialize<ProductAndStorage>(productFromServer);

        clientVM.UserProducts.Clear();

        foreach (var pr in prod.ProductTypes)
        {
            clientVM.UserProducts.Add(new UserProduct(pr.Id, pr.ProductName));
        }

        clientVM.Storages.Clear();

        foreach (var pr in prod.Storages)
        {
            clientVM.Storages.Add(new UserStorage(pr.Id, pr.Product.Party, pr.Product.ProductType.ProductName, pr.Location.RackNumber, pr.Amount));
        }

        MakeStorageCopy(clientVM.Storages); // делаем копию склада
    }

    // 
    private async Task ReceiveProductParty()
    {    
        string productTypeFromServer = await server.ReceiveString();
        var prod = JsonSerializer.Deserialize<List<string>>(productTypeFromServer);

        clientVM.ProductParty.Clear();

        foreach (var pr in prod)
        {
            clientVM.ProductParty.Add(pr);
        }
    }

    //
    private async Task ReceiveUserInfo()
    {
        string userFromServer = await server.ReceiveString();
        clientVM.User = JsonSerializer.Deserialize<MyUser>(userFromServer);
    }

    //
    private async Task ReceiveLocation()
    {
        string locationFromServer = await server.ReceiveString();
        List<Location> locations = JsonSerializer.Deserialize<List<Location>>(locationFromServer);

        clientVM.Locations.Clear();
        locationCopies.Clear();

        foreach (Location location in locations)
        {
            clientVM.Locations.Add(location);
            locationCopies.Add(location);
        }
    }

    //
    private async Task ReceiveDocument()
    {
        string documentFromServer = await server.ReceiveString();
        List<MyDocument> documents = JsonSerializer.Deserialize<List<MyDocument>>(documentFromServer);

        clientVM.Documents.Clear();

        foreach (MyDocument document in documents)
        {
            clientVM.Documents.Add(document);
        }

        // делаем копию документов
        documentsCopy.Clear(); 

        foreach (var document in clientVM.Documents)
        {
            documentsCopy.Add(document);
        }

    }





    private async void productListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (productListView.SelectedItem  is null) return;

        UserProduct product = (UserProduct)productListView.SelectedItem ;

        // отправка запроса - серия товара
        await server.SendInt32(UserRequest.AskProductType);
        await server.SendInt32(product.Id);
    }

    // кнопка сортировки
    private async void sortButton_Click(object sender, RoutedEventArgs e)
    {
        if (productListView.SelectedItem is null) { return; }       

        UserProduct product = (UserProduct)productListView.SelectedItem;
        string party = "";

        if (partyListView.SelectedItem != null)
        {
            party = (string)partyListView.SelectedItem;
        }

        List<UserStorage> sortedStorage = [];
        if (party != "")
        {
             sortedStorage = storageCopies
                .Where(st => st.ProductName == product.ProductName && st.Party == party)
                .ToList();                     
        }
        else
        {
            sortedStorage = storageCopies
                .Where(st => st.ProductName == product.ProductName)
                .ToList();
        }
        
        clientVM.Storages.Clear();

        foreach (UserStorage r in sortedStorage)
        {
            clientVM.Storages.Add(r);
        }
    }

    // кнопка сброса (обновить)
    private async void resetButton_Click(object sender, RoutedEventArgs e)
    {        
        await server.SendInt32(UserRequest.AskProductAndStorage);
    }

    // кнопка прием повара
    private async void acceptanceButton_Click(object sender, RoutedEventArgs e)
    {
        clientVM.ProductAcceptances.Clear();

        await server.SendInt32(UserRequest.AskProduct);

        GoodsAcceptance goodsAcceptance = new GoodsAcceptance(clientVM);
        if (goodsAcceptance.ShowDialog() == true)
        {
            // отправка на сервер clientVM.ProductAcceptances
            await server.SendInt32(UserRequest.SendAcceptance);

            List<ProductAcceptance> products = [];
            foreach(ProductAcceptance prod in clientVM.ProductAcceptances)
                products.Add(prod);

            string toSend = JsonSerializer.Serialize(products);
            await server.SendString(toSend);

            // отправка запроса - список товаров + склад (обновление информации)
            await server.SendInt32(UserRequest.AskProductAndStorage);
        }       
    }

    // кнопка - Расход
    private async void consumptionButton_Click(object sender, RoutedEventArgs e)
    {
        clientVM.ProductConsumptions.Clear();

        await server.SendInt32(UserRequest.AskProduct);

        GoodsConsumption consumption = new GoodsConsumption(clientVM);
        if (consumption.ShowDialog() == true)
        {
            await server.SendInt32(UserRequest.SendConsumption);

            List<ProductConsumption> products = [];
            foreach (ProductConsumption product in clientVM.ProductConsumptions)
                products.Add(product);

            string toSend = JsonSerializer.Serialize(products);
            await server.SendString(toSend);

            await server.SendInt32(UserRequest.AskProductAndStorage);
        }

    }

    // кнопка - Документ
    private async void documentButton_Click(object sender, RoutedEventArgs e)
    {
        // запрос на сервер (документы)
        await server.SendInt32(UserRequest.AskDocument);

        // скрыть storageGrid -> показать documentGrid
        storageGrid.Visibility = Visibility.Hidden;
        documentGrid.Visibility = Visibility.Visible;
    }

    private void searchButton_Click(object sender, RoutedEventArgs e)
    {
        if (docNumberTextBox.Text.Length < 1) return;  

        List<MyDocument> searchResults = documentsCopy
            .Where(doc => doc.DocumentNumber == docNumberTextBox.Text)
            .ToList();

        if (searchResults.Count == 0)
        {
            MessageBox.Show($"По номеру {docNumberTextBox.Text} документов не найдено");
            return;
        }

        clientVM.Documents.Clear();

        foreach (var result in searchResults)
        {
            clientVM.Documents.Add(result);
        }

        refreshButton.IsEnabled = true;
    }

    private void docNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if(docNumberTextBox.Text.Length == 6)
            searchButton.IsEnabled = true;
        else
            searchButton.IsEnabled = false;
    }

    // обновить окно склада
    private void refreshButton_Click(object sender, RoutedEventArgs e)
    {
        clientVM.Documents.Clear ();

        foreach(MyDocument document in documentsCopy)
            clientVM.Documents.Add(document);

        docNumberTextBox.Text = "";
        searchButton.IsEnabled = false;
        refreshButton.IsEnabled = false;
    }

    // назад - из документов
    private void backButton_Click(object sender, RoutedEventArgs e)
    {
        documentGrid.Visibility = Visibility.Hidden;
        storageGrid.Visibility = Visibility.Visible;
    }

    // кнопка стеллажи
    private async void rackButton_Click(object sender, RoutedEventArgs e)
    {  
        clientVM.AddDelRack.AddRacks.Clear();
        clientVM.AddDelRack.DeleteRacks.Clear();

        RackWindow rackWindow = new RackWindow(locationCopies, clientVM.AddDelRack);
        if(rackWindow.ShowDialog() == true)
        {
            await server.SendInt32(UserRequest.SendLocationChanges);

            string toSend = JsonSerializer.Serialize(clientVM.AddDelRack);
            await server.SendString(toSend);

            await server.SendInt32(UserRequest.AskLocation);

            //clientVM.Locations.Clear();
            //locationCopies.Clear();
        }
    }

    //кнопка инвентаризации
    private void inventoryButton_Click(object sender, RoutedEventArgs e)
    {
        Inventory inventory = new Inventory();
        if(inventory.ShowDialog() == true)
        {

        }
    }
}
