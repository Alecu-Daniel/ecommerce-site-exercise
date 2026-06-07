using backend.Dtos;

namespace backend.Repositories
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderWithItemsAsync(int userId, string shippingAddress, decimal totalAmount, IEnumerable<CartItemResponseDto> cartItems);
    }
}