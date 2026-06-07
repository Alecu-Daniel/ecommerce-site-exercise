using backend.Dtos;
using backend.Repositories;
using backend.Services.Interfaces;
using Microsoft.OpenApi.Models;

namespace backend.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public decimal CalculateTotal(IEnumerable<(int Quantity, decimal Price)> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                total += item.Quantity * item.Price;
            }
            return total;
        }

        public async Task<IEnumerable<CartItemResponseDto>> GetUserCartAsync(int userId)
        {
            return await _cartRepository.GetUserCartAsync(userId);
        }

        public async Task<CartTotalDto> GetCartTotalAsync(int userId)
        {
            var cartData = await _cartRepository.GetCartItemsWithPricesAsync(userId);
            decimal totalCartCost = CalculateTotal(cartData);

            return new CartTotalDto { Total = totalCartCost };
        }

        public async Task<bool> AddToCartAsync(int userId, CartItemUpsertDto cartItem)
        {
            if (cartItem.Quantity == 0) return false;
            return await _cartRepository.UpsertCartItemAsync(userId, cartItem);
        }

        public async Task<bool> RemoveCartItemAsync(int userId, int productId)
        {
            return await _cartRepository.RemoveCartItemAsync(userId, productId);
        }
    }

}
