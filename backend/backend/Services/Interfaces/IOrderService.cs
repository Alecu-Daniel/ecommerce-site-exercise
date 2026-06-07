using backend.Dtos;

namespace backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(int userId, string shippingAddress);
    }
}
