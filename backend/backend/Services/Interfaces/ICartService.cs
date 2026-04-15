namespace backend.Services.Interfaces
{
    public interface ICartService
    {
        decimal CalculateTotal(IEnumerable<(int Quantity, decimal Price)> items);
    }
}
