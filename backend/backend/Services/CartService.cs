using backend.Services.Interfaces;

namespace backend.Services
{
    public class CartService : ICartService
    {
        public decimal CalculateTotal(IEnumerable<(int Quantity, decimal Price)> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                total += item.Quantity * item.Price;
            }
            return total;
        }
    }
}
