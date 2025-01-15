using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Xml;
using static Ambev.DeveloperEvaluation.Domain.Entities.CartProduct;

namespace Ambev.DeveloperEvaluation.ORM;

public class DefaultContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().OwnsOne(p => p.Rating);  //
                                                                //
        //modelBuilder.Entity<Cart>().Property(e => e.Id).ValueGeneratedOnAdd();

        //modelBuilder.Entity<Cart>().HasKey(c => c.Id);
        //modelBuilder.Entity<CartProduct>().HasKey(c => c.ProductId);

        //modelBuilder.Entity<Cart>().HasMany(g => g.CartProductsList).WithOne(c => c.Cart);

        // relacionamento entre Cart e  CartProduct
        modelBuilder.Entity<CartProduct>()
           .HasKey(cp => new { cp.ProductId }); 


        modelBuilder.Entity<CartProduct>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(c => c.ProductId);

        // quantitiy é obrigatorio
        modelBuilder.Entity<CartProduct>()
            .Property(cp => cp.Quantity)
            .IsRequired();


        /*
        // Relacionamento entre Cart e CartProduct
        modelBuilder.Entity<CartProduct>()
            .HasOne(cp => cp.Cart) // Cart tem muitos CartProducts
                .WithMany(c => c.CartProductsList) // Um CartProduct tem um Cart
            .HasForeignKey(cp => cp.CartId); // A chave estrangeira em CartProduct para Cart
        
        // Relacionamento entre Product e CartProduct
        modelBuilder.Entity<CartProduct>()
            .HasOne(cp => cp.Product) // CartProduct tem um Product
            .WithMany() // Product não precisa de coleção de CartProducts
            .HasForeignKey(cp => cp.ProductId); // A chave estrangeira em CartProduct para Product
        */


        // Evitar a serialização de CartProduct.Product diretamente (se desejado)
        modelBuilder.Entity<CartProduct>()
            .Navigation(cp => cp.Product)
            .AutoInclude(false);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(
            connectionString,
            b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.WebApi")
        );

        return new DefaultContext(builder.Options);
    }
}

