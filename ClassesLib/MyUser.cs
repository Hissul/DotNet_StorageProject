namespace ClassesLib;

public class MyUser  // нужен
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";

    public MyUser(){}

    public MyUser(int id, string name, string surname, string login, string password)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Login = login;
        Password = password;
    }

    public override string ToString() => $"{Surname} {Name}";
}
