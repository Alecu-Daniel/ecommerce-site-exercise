using backend.Dtos;
using backend.Repositories;
using backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;

        private readonly ICartService _cartService;
        
        
        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _cartService = cartService;
        }

        public async Task<int> CreateOrderAsync(int userId, string shippingAddress)
        {
            var cartItems = await _cartRepository.GetUserCartAsync(userId);
            if(cartItems == null || !cartItems.Any())
            {
                throw new Exception("Cannot place an order with an empty cart.");
            }

            decimal totalCartCost = _cartService.CalculateTotal(cartItems.Select(item => (item.Quantity, item.Price)));

            var orderId = await _orderRepository.CreateOrderWithItemsAsync(userId, shippingAddress, totalCartCost, cartItems);
            return orderId;
        }
    }
}
