using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/orders", async(OrderDbContext context) => 
{
    var orders = await context.Orders.ToListAsync();
    return Results.Ok(orders);
});

app.MapGet("/orders/{id}", async (int id, OrderDbContext context) => 
{
    var order = await context.Orders.FindAsync(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

app.MapPost("/orders", async (OrderService.Models.Order order, 
OrderDbContext context) => 
{
    context.Orders.Add(order);
    await context.SaveChangesAsync();
    return Results.Created($"/orders/{order.Id}", order);
});

app.MapPut("/orders/{id}", async (int id, OrderService.Models.order updateOrder,
OrderDbContext context) =>
{
    var order = await context.Orders.FindAsync(id);
    if (order is null) return Results.NotFound();

    order.ProductId = updateOrder.ProductId;
    order.Quantity = updateOrder.Quantity;
    order.CustomerName = updateOrder.CustomerName;
    order.CustomerEmail = updateOrder.CustomerEmail;
    order.ShippingAddress = updateOrder.ShippingAddress;
    order.TotalAmount = updateOrder.TotalAmount;
    order.Status = updateOrder.Status;
    
    await context.SaveChangesAsync();
    return Results.Ok(order);
});

app.MapDelete("/orders/{id}", async(int id, OrderDbContext context) => 
{
    var order = await context.Orders.FindAsync(id);
    if(order is null) return Results.NotFound();

    context.Orders.Remove(order);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "OrderService", 
timestamp = DateTime.UtcNow }));

using(var scope = app.Services.CreateScope())
{
    var dbContext = Scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.EnsureCreated();
    console.WriteLine("OrderService database initialized!");
}

app.Run();

