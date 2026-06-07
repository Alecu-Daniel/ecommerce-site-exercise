using backend.DataContext;
using backend.Dtos;
using backend.Repositories;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;
        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }


        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(CartItemUpsertDto cartItem)
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            try
            {
                var sucess = await _cartService.AddToCartAsync(userId, cartItem);
                return Ok(sucess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while user {UserId} tried to add product {ProductId} to cart.", userId, cartItem.ProductId);
                return StatusCode(500, new { message = "An error occurred while updating your shopping cart." });
            }
        }


        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            try
            {
                var cart = await _cartService.GetUserCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve shopping cart details for user {UserId}.", userId);
                return StatusCode(500, new { message = "Could not fetch your shopping cart content." });
            }
        }

        [HttpGet("GetTotal")]
        public async Task<IActionResult> GetTotal()
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            try
            {
                var totalCartCost = _cartService.GetCartTotalAsync(userId);
                return Ok(totalCartCost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing total cost breakdown metrics for user {UserId}.", userId);
                return StatusCode(500, new { message = "Failed to calculate order price values." });
            }
        }

        [HttpDelete("RemoveItem/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);
            
            try
            {
                var delete = await _cartService.RemoveCartItemAsync(userId, productId);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while attempting deletion of product {ProductId} for user {UserId}.", productId, userId);
                return StatusCode(500, new { message = "Could not remove item from cart." });
            }


        }
    }
}