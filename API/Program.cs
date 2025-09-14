using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Application;
using Domain;
using Infrastructure;
using System.IO; // Add this for Path

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

// DI
builder.Services.AddSingleton<IBinWidthCalculator, BinWidthCalculator>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<CreateOrder>();
builder.Services.AddScoped<GetOrder>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Bin Width API v1");
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
