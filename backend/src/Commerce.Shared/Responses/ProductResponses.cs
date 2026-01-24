namespace Commerce.Shared.Responses;

public class ProductResponse
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
}