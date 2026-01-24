var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
var app = builder.Build();
if(app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}

var orders = new List<Order>
{
    new(1, 101, new List<OrderItem> { new(1,2), new(2,1)}, 101500),
    new(2, 102, new List<OrderItem> { new(3,1)}, 1500)
};

app.MapGet("/orders", () => orders);
app.MapGet("/orders/{id}", (int id) => orders.FirstOrDefault(o => o.Id == id)
        is Order order
        ? Results.Ok(order)
        : Results.NotFound());

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();

record Order(int Id, int CustomerId, List<OrderItem> Items, decimal TotalAmount);

record OrderItem(int ProductId, int Quantity);