using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var connectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
    var mongoClient = new MongoClient(connectionString);

    using (var context = new CustomersContext(mongoClient))
    {
        //var customer = new Customer
        //{
        //    Name = "Willow",
        //    Species = Species.Dog,
        //    ContactInfo = new()
        //    {
        //        ShippingAddress = new()
        //        {
        //            Line1 = "Barking Gate",
        //            Line2 = "Chalk Road",
        //            City = "Walpole St Peter",
        //            Country = "UK",
        //            PostalCode = "PE14 7QQ"
        //        },
        //        BillingAddress = new()
        //        {
        //            Line1 = "15a Main St",
        //            City = "Ailsworth",
        //            Country = "UK",
        //            PostalCode = "PE5 7AF"
        //        },
        //        Phones = new()
        //        {
        //            HomePhone = new() { CountryCode = 44, Number = "7877 555 555" },
        //            MobilePhone = new() { CountryCode = 1, Number = "(555) 2345-678" },
        //            WorkPhone = new() { CountryCode = 1, Number = "(555) 2345-678" }
        //        }
        //    }
        //};

        //context.Add(customer);
        //context.SaveChanges();

        var pluto = context.Customers.FirstOrDefault();
        Console.WriteLine(pluto.Name.ToString());
    }
    return true;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

#region Class model

public class Customer
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Species Species { get; set; }
    public required ContactInfo ContactInfo { get; set; }
}

public class ContactInfo
{
    public required Address ShippingAddress { get; set; }
    public Address? BillingAddress { get; set; }
    public required PhoneNumbers Phones { get; set; }
}

public class PhoneNumbers
{
    public PhoneNumber? HomePhone { get; set; }
    public PhoneNumber? WorkPhone { get; set; }
    public PhoneNumber? MobilePhone { get; set; }
}

public class PhoneNumber
{
    public required int CountryCode { get; set; }
    public required string Number { get; set; }
}

public class Address
{
    public required string Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? Line3 { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string PostalCode { get; set; }
}

public enum Species
{
    Human,
    Dog,
    Cat
}

#endregion Class model 

#region Dbcontext

public class CustomersContext : DbContext
{
    private readonly MongoClient _client;

    public CustomersContext(MongoClient client)
    {
        _client = client;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMongoDB(_client, "efsample");

    public DbSet<Customer> Customers => Set<Customer>();
}

#endregion Dbcontext
