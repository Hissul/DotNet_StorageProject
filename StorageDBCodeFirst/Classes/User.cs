using Microsoft.EntityFrameworkCore;

namespace StorageDBCodeFirst.Classes;

[Index(nameof(Login), IsUnique = true)]
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";    
} 
