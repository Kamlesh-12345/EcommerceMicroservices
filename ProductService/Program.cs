using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Contracts;
using ProductService.Models;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/products", async(ProductDbContext context) => 
{
    var products = await context.Products.ToListAsync();
    return Results.Ok(products);
});

app.MapGet("/products/{id}", async (int id, ProductDbContext context) => 
{
    var product = await context.Products.FindAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products", async (CreateProductRequest req, 
ProductDbContext context) => 
{
    var errors = new Dictionary<string, string[]>();

    if(string.IsNullOrWhiteSpace(req.Name)) errors["name"] = new[] {"Name is required"}; 
    if(req.Price <= 0) errors["price"] = new[] {"Price must be > 0"};
    if(req.Stock < 0) errors["stock"] = new[] {"Stock must be >= 0"};

    if (errors.Count > 0) return Results.ValidationProblem(errors);

    var entity = new Product
    {
        Name = req.Name.Trim(),
        Description = req.Description,
        Price = req.Price,
        Stock = req.Stock,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    context.Products.Add(entity);
    await context.SaveChangesAsync();

    var resp = new ProductResponse
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Price = entity.Price,
        Stock = entity.Stock
    };

    return Results.Created($"/products/{entity.Id}", resp);
});

app.MapPut("/products/{id}", async (int id, UpdateProductRequest req,
ProductDbContext context) =>
{
    var errors = new Dictionary<string, string[]>();

    if(string.IsNullOrWhiteSpace(req.Name)) errors["name"] = new[] {"Name is required"}; 
    if(req.Price <= 0) errors["price"] = new[] {"Price must be > 0"};
    if(req.Stock < 0) errors["stock"] = new[] {"Stock must be >= 0"};

    if (errors.Count > 0) return Results.ValidationProblem(errors);

    var product = await context.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = req.Name;
    product.Description = req.Description;
    product.Price = req.Price;
    product.Stock = req.Stock;
    product.UpdatedAt = DateTime.UtcNow;

    await context.SaveChangesAsync();

var resp = new ProductResponse
{
    Id = product.Id,
    Name = product.Name,
    Description = product.Description,
    Price = product.Price,
    Stock = product.Stock
};

    return Results.Ok(resp);
});

app.MapDelete("/products/{id}", async(int id, ProductDbContext context) => 
{
    var product = await context.Products.FindAsync(id);
    if(product is null) return Results.NotFound();

    context.Products.Remove(product);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "ProductService", 
timestamp = DateTime.UtcNow }));

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    dbContext.Database.Migrate();
    Console.WriteLine("ProductService database migrated!");
}

app.Run();

