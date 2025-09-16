using Application;
using Domain;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Controllers + enums en texte
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// OpenAPI v1
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Bin Width API",
        Version = "v1",
        Description = "Create orders and compute required bin width (mm)."
    });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "API.xml")); 
});

// EF Core + PostgreSQL
var cs = builder.Configuration.GetConnectionString("Default")
         ?? builder.Configuration["ConnectionStrings:Default"];
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();

// DI
builder.Services.AddSingleton<IBinWidthCalculator, BinWidthCalculator>();
builder.Services.AddScoped<CreateOrder>();
builder.Services.AddScoped<GetOrder>();
builder.Services.AddTransient<API.Middleware.ErrorHandlingMiddleware>();

var app = builder.Build();

// Middleware
app.UseMiddleware<API.Middleware.ErrorHandlingMiddleware>();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Bin Width API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
app.Run();
