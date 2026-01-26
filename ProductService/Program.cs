using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
app.UseAuthorization();
app.MapControllers();

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

app.MapPost("/products", async (ProductService.Models.Product product, 
ProductDbContext context) => 
{
    context.Products.Add(product);
    await context.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", async (int id, ProductService.Models.Product updateProduct,
ProductDbContext context) =>
{
    var product = await context.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = updateProduct.Name;
    product.Description = updateProduct.Description;
    product.Price = updateProduct.Price;
    product.Stock = updateProduct.Stock;
    product.UpdatedAt = DateTime.UtcNow;

    await context.SaveChangesAsync();
    return Results.Ok(product);
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
    dbContext.Database.EnsureCreated();
    Console.WriteLine("ProductService database initialized!");
}

app.Run();

