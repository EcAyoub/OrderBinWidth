using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration;

public static class TestDb
{
    public static AppDbContext Create()
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                 ?? "Host=localhost;Port=5432;Database=orderbin;Username=postgres;Password=postgres";

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs)
            .Options;

        var context = new AppDbContext(opts);

        context.Database.Migrate();

        return context;
    }

    public static void Clear(AppDbContext db)
    {
        db.OrderItems.RemoveRange(db.OrderItems);
        db.Orders.RemoveRange(db.Orders);
        db.SaveChanges();
    }
}
