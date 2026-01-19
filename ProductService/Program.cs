using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if(app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}

var products = new List<Product>
{
    new(1, "Laptop", 50000, 10),
    new(2, "Mouse", 500, 50),
    new(3, "Keyboard", 1500, 30)
};

app.MapGet("/products", () => products);
app.MapGet("/products/{id}", (int id) =>
products.FirstOrDefault(p => p.Id == id) 
    is Product product 
    ? Results.Ok(product) 
    : Results.NotFound());

app.MapPost("/products", (Product product) =>
{
    products.Add(product);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapGet("/health", () => Results.Ok("Healthy"));

record Product(int Id, string Name, decimal Price, int Stock);

app.Run();