using ClassesLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PrefixLib;
using Request;
using StorageDBCodeFirst;
using StorageDBCodeFirst.Classes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace StorageServer;

internal class Server
{
    private StorageDbContext db = new();    
    private TcpListener listener;
    private List<MyUser> userInfos = [];
    
    
    private ProductAndStorage test = new ProductAndStorage();// ПЕРЕИМЕНОВАТЬ
    private MyUser user;
    private List<string> productParties = [];    
    private List<Product> products = [];
    private List<Location> locations = [];
    private List<ProductAcceptance> productsAcceptances = [];
    private List<MyDocument> documents = [];
    private List<ProductConsumption> productConsumptions = [];
    private AddDelRack addDelRack = new AddDelRack();
    



    static async Task Main(string[] args) => await new Server().Run();

    private async Task Run()
    {
        //список пользователей(логин - пароль)
        await GetUserInfo();
        // стеллажи
        await GetLocation();

        string[] connect = await File.ReadAllLinesAsync("Config.txt");
        listener = new TcpListener(IPAddress.Any, int.Parse(connect[1]));

        try
        {
            listener.Start();
            await Console.Out.WriteLineAsync("Server is started");

            await AcceptLoop();
        }
        catch(Exception ex)
        {
            await Console.Out.WriteLineAsync("функция Run");
        }
        finally { listener.Stop(); }
    }

    // ПОЛУЧАЕМ список пользователей(логин - пароль)
    private async Task GetUserInfo()
    {
        User[] users = db.Users.ToArray();
        foreach (var user in users)
        {
            userInfos.Add(new(user.Id , user.Name, user.Surname, user.Login, user.Password));
        }
    }

    // ПОЛУЧАЕМ стеллажи
    private async Task GetLocation()
    {
        locations.Clear();
        locations = await db.Locations.ToListAsync();
    }

    // ПОЛУЧАЕМ все продукты 
    private async Task GetProduct()
    {
        test.ProductTypes = await db.ProductTypes.ToListAsync();
    }

    // ПОЛУЧАЕМ весь склад
    private async Task GetStorage()
    {       
        test.Storages = await db.Storages
            .Include(pr => pr.Product)
            .ThenInclude(prT => prT.ProductType)
            .Include(loc => loc.Location).ToListAsync();      
    }

    // ПОЛУЧАЕМ сери определенного продукта
    private async Task GetCertainProductParty(int productId)
    {
        productParties = await db.Products.Where(pr => pr.ProductTypeId == productId)
            .Select(pt => pt.Party).ToListAsync();
    }

    // ПОЛУЧАЕМ продукты + серии
    private async Task GetProductParty()
    {
        products = await db.Products.Include(pr => pr.ProductType).ToListAsync();
    }

    // //////////////////////////////////
    // ПОЛУЧАЕМ документы
    private async Task GetDocument()
    {
        List<Document> docs = await db.Documents
            .Include(doc => doc.DocumentType)
            .Include(doc => doc.User)
            .Include(prod => prod.ProductType)
            .ToListAsync();

        foreach (Document doc in docs)
        {
            documents.Add(new MyDocument(
                doc.DocumentType.DocumentName,
                doc.DocumentNumber,
                doc.ProductType.ProductName,
                doc.Amount,
                doc.Date,
                doc.User.Surname,
                doc.Party));
        }
    }   

    private async Task AcceptLoop()
    {
        while (true)
        {
            try
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                await Console.Out.WriteLineAsync($"user connected");

                // принять - логин и пароль от клиента
                await ReceiveLogin(client);

                await SendUserInfo(client);

                // отправить - список продуктов , продукты + серию, склад, документы

                // слушать клиента
                await ListenClient(client);
            }catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("функция AcceptLoop");
            }
        }
    }

    private async Task ReceiveLogin(TcpClient client)
    {
        while (true)
        {
            string fromClient = await client.ReceiveString();

            string[] loginPassword = fromClient.Split(',');
            bool isFinded = false;

            await Console.Out.WriteLineAsync("Login and password was received");

            foreach (MyUser userInfo in userInfos)
            {
                if (userInfo.Login == loginPassword[0] && userInfo.Password == loginPassword[1])
                {
                    // ответ клиенту
                    await client.SendString(ServerAction.Allow.ToString());
                    user = new MyUser(
                        userInfo.Id, userInfo.Name, userInfo.Surname, userInfo.Login, userInfo.Password);
                    isFinded = true;
                    break;
                }
            }

            if (!isFinded)
            {
                // ответ клиенту
                await client.SendString(ServerAction.Prohibit.ToString());                
            }

            await Console.Out.WriteLineAsync("Confirmation is sent");
            

            if ( isFinded == true)
            {
                return;
            }
        }        
    }

    private async Task SendUserInfo(TcpClient client)
    {
        await client.SendInt32(ServerResponse.SendUserInfo);

        string toSend = JsonSerializer.Serialize(user); // объединить
        await client.SendString(toSend);

    }

    // СЛУШАЕМ КЛИЕНТА
    private async Task ListenClient(TcpClient client)
    {     
        while(true)
        {
            try
            {
                int signal = await client.ReceiveInt32();
                await Console.Out.WriteLineAsync("Request was received");

                switch (signal)
                {
                    case UserRequest.AskProductAndStorage:
                        await GetProduct();
                        await GetStorage();

                        await client.SendInt32(ServerResponse.SendProductAndStorage);                        
                        await client.SendString(JsonSerializer.Serialize(test));

                        await Console.Out.WriteLineAsync("ProductAndStorage are sent");
                        break;

                    case UserRequest.AskProductType:
                        int productId = await client.ReceiveInt32();
                        await GetCertainProductParty(productId);

                        await client.SendInt32(ServerResponse.SendProductParty);                        
                        await client.SendString(JsonSerializer.Serialize(productParties));

                        await Console.Out.WriteLineAsync("ProductParty are sent");
                        break;

                    case UserRequest.AskProduct:
                        await GetProductParty();

                        await client.SendInt32(ServerResponse.SendProduct);                        
                        await client.SendString(JsonSerializer.Serialize(products));

                        await Console.Out.WriteLineAsync("Product are sent");
                        break;

                    case UserRequest.AskLocation:
                        await GetLocation();

                        await client.SendInt32(ServerResponse.SendLocation);                        
                        await client.SendString(JsonSerializer.Serialize(locations));

                        await Console.Out.WriteLineAsync("Location are sent");
                        break;

                    case UserRequest.SendAcceptance:
                        string acceptanceFromClient = await client.ReceiveString();
                        productsAcceptances = JsonSerializer.Deserialize<List<ProductAcceptance>>(acceptanceFromClient);

                        foreach (ProductAcceptance product in productsAcceptances)
                        {
                            if (product.IsNew == true)
                            {
                                Product prod = new();
                                prod.Party = product.Party;
                                prod.ProductTypeId = product.ProductTypeId;
                                db.Products.Add(prod);
                                await db.SaveChangesAsync();

                                await SaveAcceptanceToDB(product, prod, client);
                            }
                            else
                            {
                                await SaveAcceptanceToDB(product, client);
                            }
                        }
                        await Console.Out.WriteLineAsync("Acceptances were saved");
                        break;

                    case UserRequest.AskDocument:
                        await GetDocument();

                        await client.SendInt32(ServerResponse.SendDocument);                        
                        await client.SendString(JsonSerializer.Serialize(documents));

                        await Console.Out.WriteLineAsync("Document are sent");
                        break;

                    case UserRequest.SendConsumption:
                        string consumptionFromClient = await client.ReceiveString();
                        productConsumptions = JsonSerializer.Deserialize<List<ProductConsumption>>(consumptionFromClient);

                        foreach (ProductConsumption product in productConsumptions)
                        {
                            await SaveConsumptionToDB(product, client);
                        }
                        await Console.Out.WriteLineAsync("Consumption were saved");
                        break;

                    case UserRequest.SendLocationChanges:
                        string changesFromClient = await client.ReceiveString();

                        addDelRack = JsonSerializer.Deserialize<AddDelRack>(changesFromClient);

                        await SaveLocationChanges(client);
                        break;
                }
            }catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("функция ListenClient");
                break;
            }
        }
    }

    // //////////////////////////////////
    // сохранение в БД (изменение склада)
    private async Task SaveLocationChanges(TcpClient client)
    {
        using IDbContextTransaction transaction = db.Database.BeginTransaction();
        try {
            if (addDelRack.AddRacks.Count != 0)
            {
                foreach (int rack in addDelRack.AddRacks)
                {   
                    foreach(Location loc in locations)
                    {
                        if(loc.RackNumber == rack)
                        {
                            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
                            await client.SendString(ServerAction.NotSaved.ToString());
                            return;
                        }
                    }
                    Location location = new Location();
                    location.RackNumber = rack;
                    db.Locations.Add(location);
                }
            }

            bool isDeleted = false;
            if (addDelRack.DeleteRacks.Count != 0)
            {
                List<int> numb = test.Storages.Select(st => st.Location.RackNumber).ToList();

                foreach (int rack in addDelRack.DeleteRacks)
                {
                    // нельзя удалить если стеллаж не пустой
                    if (!numb.Contains(rack))
                    {
                        Location location = db.Locations.Where(loc => loc.RackNumber == rack).First();
                        db.Locations.Remove(location);

                        isDeleted = true;
                    }                 
                }
            }

            await db.SaveChangesAsync();
            transaction.Commit();

            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            if((addDelRack.AddRacks.Count == 0 && addDelRack.DeleteRacks.Count != 0  && isDeleted == false))
            {
                await client.SendString(ServerAction.NotDeleted.ToString());
            }
            else
            {
                await client.SendString(ServerAction.Saved.ToString());
            }
            
        }
        catch (Exception ex)
        {
            // отправка результата (удачно или нет) 
            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            await client.SendString(ServerAction.NotSaved.ToString());

            transaction.Rollback();
        }
    }

    // //////////////////////////////////
    // сохранение в БД (списание на склад)
    private async Task SaveConsumptionToDB(ProductConsumption product, TcpClient client)
    {
        Product prod = db.Products
                    .Where(pr => pr.Party == product.Party && pr.ProductTypeId == product.ProductTypeId)
                    .First();

        Storage storage = db.Storages
                      .Where(st => st.ProductId == prod.Id && st.LocationId == product.LocationId)
                      .First();

        using IDbContextTransaction transaction = db.Database.BeginTransaction();
        try
        {

            if (storage.Amount == product.Amount) // 
            {                    
                db.Storages.Remove(storage);

                var ddd = db.Storages.Where(st => st.ProductId == prod.Id).ToList();
                if(ddd.Count == 1)
                {
                    db.Products.Remove(prod);
                }
            }
            else
            {
                storage.Amount -= product.Amount;
            }

            Document document = new Document();
            document.DocumentTypeId = ProductConsumption.DocumentTypeId;
            document.ProductTypeID = prod.ProductTypeId;
            document.UserId = user.Id;
            document.DocumentNumber = product.DocumentNumber;
            document.Amount = product.Amount;
            document.Date = product.Date;
            document.Party = product.Party;

            db.Documents.Add(document);

            await db.SaveChangesAsync();

            transaction.Commit();

            // отправка результата (удачно или нет) 
            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            await client.SendString(ServerAction.Saved.ToString());
        }
        catch (Exception ex)
        {
            // отправка результата (удачно или нет) 
            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            await client.SendString(ServerAction.NotSaved.ToString());

            transaction.Rollback();
        }
    }

    // //////////////////////////////////
    // сохранение в БД (поступление на склад)
    private async Task SaveAcceptanceToDB(ProductAcceptance product, Product prod, TcpClient client)
    {
        int prodId = db.Products.Where(pr => pr.Party == prod.Party && pr.ProductTypeId == prod.ProductTypeId).Select(pr => pr.Id).First();

        using IDbContextTransaction transaction = db.Database.BeginTransaction();
        try 
        {
            Storage storage = new();
            storage.ProductId = prodId;
            storage.LocationId = product.LocationId;
            storage.Amount = product.Amount;
            db.Storages.Add(storage);
            //db.SaveChanges();

            Document document = new Document();
            document.DocumentTypeId = ProductAcceptance.DocumentTypeId;
            document.ProductTypeID = product.ProductTypeId;
            document.UserId = user.Id;
            document.DocumentNumber = product.DocumentNumber;
            document.Amount = product.Amount;
            document.Date = product.Date;
            document.Party = product.Party;

            db.Documents.Add(document);
            await db.SaveChangesAsync();

            transaction.Commit();

            // отправка результата (удачно или нет) 
            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            await client.SendString(ServerAction.Saved.ToString());
        }
        catch (Exception ex)
        {
            // отправка результата (удачно или нет) 
            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            await client.SendString(ServerAction.NotSaved.ToString());

            transaction.Rollback();
        }


    }


    // //////////////////////////////////
    // сохранение в БД (поступление на склад)
    private async Task SaveAcceptanceToDB(ProductAcceptance product, TcpClient client)
    {
        Product prod = db.Products
                            .Where(pr => pr.Party == product.Party && pr.ProductTypeId == product.ProductTypeId)
                            .First();

        using IDbContextTransaction transaction = db.Database.BeginTransaction();
        try
        {
            Storage storage = db.Storages.Where(pr => pr.ProductId == prod.Id).First();

            if (storage.LocationId == product.LocationId)
            {
                storage.Amount += product.Amount;
                //await db.SaveChangesAsync();
            }
            else
            {
                Storage stor = new Storage();
                stor.ProductId = prod.Id;
                stor.LocationId = product.LocationId;
                stor.Amount = product.Amount;
                db.Storages.Add(stor);
                //await db.SaveChangesAsync();
            }


            Document document = new Document();
            document.DocumentTypeId = ProductAcceptance.DocumentTypeId;
            document.ProductTypeID = prod.ProductTypeId;
            document.UserId = user.Id;
            document.DocumentNumber = product.DocumentNumber;
            document.Amount = product.Amount;
            document.Date = product.Date;
            document.Party = product.Party;

            db.Documents.Add(document);
            await db.SaveChangesAsync();

            transaction.Commit();

            // отправка результата (удачно или нет) 
            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            await client.SendString(ServerAction.Saved.ToString());
        }
        catch (Exception ex)
        {
            // отправка результата (удачно или нет) 
            await client.SendInt32(ServerResponse.SendMessageAboutChanges);
            await client.SendString(ServerAction.NotSaved.ToString());

            transaction.Rollback();
        }
      
    }







}
