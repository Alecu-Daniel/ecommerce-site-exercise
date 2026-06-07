using backend.Dtos;

namespace backend.Services.Interfaces
{
    public interface ICartService
    {
        decimal CalculateTotal(IEnumerable<(int Quantity, decimal Price)> items);
        Task<IEnumerable<CartItemResponseDto>> GetUserCartAsync(int userId);
        Task<CartTotalDto> GetCartTotalAsync(int userId);
        Task<bool> AddToCartAsync(int userId, CartItemUpsertDto cartItem);
        Task<bool> RemoveCartItemAsync(int userId, int productId);
    }
}
