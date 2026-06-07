using backend.Dtos;

namespace backend.Repositories
{
    public interface ICartRepository
    {     
        Task<IEnumerable<CartItemResponseDto>> GetUserCartAsync(int userId);
        Task<IEnumerable<(int Quantity, decimal Price)>> GetCartItemsWithPricesAsync(int userId);
        Task<bool> UpsertCartItemAsync(int userId, CartItemUpsertDto cartItem);
        Task<bool> RemoveCartItemAsync(int userId, int productId);
    }
}
