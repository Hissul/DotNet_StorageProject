using Microsoft.EntityFrameworkCore;
using StorageDBCodeFirst.Classes;

namespace StorageDBCodeFirst;

public class StorageDbContext : DbContext
{ 
    public DbSet<User> Users { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Storage> Storages { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Data Source=(localdb)\.;Initial Catalog=StorageDB;Integrated Security=True");
    }
}
