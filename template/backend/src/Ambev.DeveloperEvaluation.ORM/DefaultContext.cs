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
        modelBuilder.Entity<Product>().OwnsOne(p => p.Rating);
        /*
                // relacionamento entre Cart e  CartProduct
                modelBuilder.Entity<CartProduct>()
                    .HasKey(cp => cp.Id);


                modelBuilder.Entity<CartProduct>()
                    .HasOne<Product>()
                    .WithMany()
                    .HasForeignKey(c => c.ProductId);
                */

        //cp.CartId,

        modelBuilder.Entity<CartProduct>().HasKey(cp => cp.Id);
        //modelBuilder.Entity<CartProduct>().HasNoKey();

        modelBuilder.Entity<CartProduct>()
            .HasOne(cp => cp.Cart)
            .WithMany(c => c.CartProductsList)
            .HasForeignKey(cp => cp.CartId)
            .HasConstraintName("FK_CartProduct_Cart_CartId");

        modelBuilder.Entity<CartProduct>()
            .HasOne(cp => cp.Product)
            .WithMany()
            .HasForeignKey(cp => cp.ProductId)
            .HasConstraintName("FK_CartProduct_Product_ProductId");


        /*
        // Evitar a serialização de Product
        modelBuilder.Entity<CartProduct>()
            .Navigation(cp => cp.Product)
            .AutoInclude(false);*/

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

