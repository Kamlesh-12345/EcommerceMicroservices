using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Contracts;
using OrderService.Models;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<OrderService.Clients.ProductClient>(c =>
{
    c.BaseAddress = new Uri("http://productservice:8080");
});

var app = builder.Build();

app.UseExceptionHandler();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/orders", async(OrderDbContext context) => 
{
    var orders = await context.Orders.ToListAsync();
    return Results.Ok(orders);
});

app.MapGet("/orders/{id:int}", async (int id, OrderDbContext context) => 
{
    var order = await context.Orders.FindAsync(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

app.MapPost("/orders", async (CreateOrderRequest req, 
OrderDbContext context,OrderService.Clients.ProductClient productClient,
    CancellationToken ct) => 
{
    var errors = new Dictionary<string, string[]>();

    if (req.ProductId <= 0) errors["productId"] = new[] {"ProductId must be > 0"};
    if (req.Quantity < 0) errors["quantity"] = new[] {"Quantity must be > 0"};

    if (string.IsNullOrWhiteSpace(req.CustomerName)) errors["customerName"] = new[] {"CustomerName is required"};
    if  (string.IsNullOrWhiteSpace(req.CustomerEmail)) errors["customerEmail"] = new[] {"CustomerEmail is required"};
    else if (!req.CustomerEmail.Contains('@')) errors["customerEmail"] = new[] {"CustomerEmail must contain @"};

    if  (string.IsNullOrWhiteSpace(req.ShippingAddress)) errors["shippingAddress"] = new[] {"ShippingAddress is required"};

    if(errors.Count > 0) return Results.ValidationProblem(errors);

     // Reserve stock FIRST
    var (ok, status) = await productClient.ReserveStockAsync(req.ProductId, req.Quantity, ct);
    if (!ok)
    {
        if (status == System.Net.HttpStatusCode.Conflict)
            return Results.Conflict(new { message = "Insufficient stock" });

        if (status == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound(new { message = "Product not found" });

        return Results.StatusCode(502);
    }

    var order = new Order
    {
        ProductId = req.ProductId,
        Quantity = req.Quantity,
        CustomerName = req.CustomerName.Trim(),
        CustomerEmail = req.CustomerEmail.Trim(),
        ShippingAddress = req.ShippingAddress.Trim(),
        Status = "Pending",
        OrderDate = DateTime.UtcNow,
        TotalAmount = 0m
    };

    context.Orders.Add(order);
    await context.SaveChangesAsync();

    var resp = new OrderResponse
    {
        Id = order.Id,
        ProductId = order.ProductId,
        Quantity = order.Quantity,
        CustomerName = order.CustomerName,
        CustomerEmail = order.CustomerEmail,
        ShippingAddress = order.ShippingAddress,
        TotalAmount = order.TotalAmount,
        Status = order.Status,
        OrderDate = order.OrderDate
    };

    return Results.Created($"/orders/{order.Id}", resp);
});

app.MapPut("/orders/{id:int}", async (int id, UpdateOrderRequest req,
OrderDbContext context) =>
{

    var errors = new Dictionary<string, string[]>();

    if (req.ProductId <= 0) errors["productId"] = new[] {"ProductId must be > 0"};
    if (req.Quantity < 0) errors["quantity"] = new[] {"Quantity must be > 0"};

    if (string.IsNullOrWhiteSpace(req.CustomerName)) errors["customerName"] = new[] {"CustomerName is required"};
    if  (string.IsNullOrWhiteSpace(req.CustomerEmail)) errors["customerEmail"] = new[] {"CustomerEmail is required"};
    else if (!req.CustomerEmail.Contains('@')) errors["customerEmail"] = new[] {"CustomerEmail must contain @"};

    if  (string.IsNullOrWhiteSpace(req.ShippingAddress)) errors["shippingAddress"] = new[] {"ShippingAddress is required"};

    if(errors.Count > 0) return Results.ValidationProblem(errors);

    var order = await context.Orders.FindAsync(id);
    if (order is null) return Results.NotFound();

    order.ProductId = req.ProductId;
    order.Quantity = req.Quantity;
    order.CustomerName = req.CustomerName;
    order.CustomerEmail = req.CustomerEmail;
    order.ShippingAddress = req.ShippingAddress;
    
    await context.SaveChangesAsync();

    var resp = new OrderResponse
    {
        Id = order.Id,
        ProductId = order.ProductId,
        Quantity = order.Quantity,
        CustomerName = order.CustomerName,
        CustomerEmail = order.CustomerEmail,
        ShippingAddress = order.ShippingAddress,
        TotalAmount = order.TotalAmount,
        Status = order.Status,
        OrderDate = order.OrderDate
    };

    return Results.Ok(resp);
});

app.MapDelete("/orders/{id:int}", async(int id, OrderDbContext context) => 
{
    var order = await context.Orders.FindAsync(id);
    if(order is null) return Results.NotFound();

    context.Orders.Remove(order);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "OrderService", 
timestamp = DateTime.UtcNow }));

app.MapGet("/orders/boom", () => Results.Problem("test", statusCode: 500));

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.Migrate();
    Console.WriteLine("OrderService database migrated!");
}

app.Run();

