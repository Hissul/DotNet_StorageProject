namespace StorageServer;

public class UserInfo // удалить
{
    public int Id { get;}
    public string Name { get;}
    public string Surname { get;}
    public string Login { get;}
    public string Password { get;}

    public UserInfo(int id, string name, string surname, string login, string password)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Login = login;
        Password = password;
    }
}
