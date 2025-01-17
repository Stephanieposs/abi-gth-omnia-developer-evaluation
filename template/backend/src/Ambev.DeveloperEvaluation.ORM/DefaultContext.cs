using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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

    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SalesItems { get; set; }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().OwnsOne(p => p.Rating);

        modelBuilder.Entity<CartProduct>().HasKey(cp => cp.Id);

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

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(s => s.SaleNumber);
            entity.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(s => s.IsCancelled).HasDefaultValue(false);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(si => si.Id);
            entity.Property(si => si.Quantity).IsRequired();
            entity.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(si => si.Discount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            entity.Property(si => si.Total).HasColumnType("decimal(18,2)");
            entity.Property(si => si.IsCancelled).HasDefaultValue(false);

            entity.HasOne(si => si.Sale)
                  .WithMany(s => s.Items)
                  .HasForeignKey(si => si.SaleId);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        string dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"); 
        if (!string.IsNullOrEmpty(dbConnectionString)) { optionsBuilder.UseNpgsql(dbConnectionString, b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.WebApi")); }
        else { throw new InvalidOperationException("Environment variable 'DB_CONNECTION_STRING' is not set."); }
    } */
}

public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        

        builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.WebApi"));

        return new DefaultContext(builder.Options);
    }
}

