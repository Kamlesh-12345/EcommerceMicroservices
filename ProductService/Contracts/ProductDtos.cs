namespace ProductService.Contracts;

public sealed class CreateProductRequest
{
    public string Name { get;set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public sealed class UpdateProductRequest
{
    public string Name { get;set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public sealed class ProductResponse
{
    public int Id { get; set; }
    public string Name { get;set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}