using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<OrderEntity>(e =>
        {
            e.HasKey(x => x.OrderId);
            e.Property(x => x.RequiredBinWidthMm)
             .HasColumnType("numeric(9,2)");                 
            e.HasMany(x => x.Items)
             .WithOne(x => x.Order)
             .HasForeignKey(x => x.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
            e.ToTable("Orders");
        });

        b.Entity<OrderItemEntity>(e =>
        {
            e.HasKey(x => new { x.OrderId, x.ProductType });  
            e.Property(x => x.ProductType)
             .HasConversion<int>();                           
            e.ToTable("OrderItems");
        });
    }
}
