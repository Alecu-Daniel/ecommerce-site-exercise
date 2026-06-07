using backend.DataContext;
using backend.Dtos;
using backend.Repositories;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        public CheckoutController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(OrderCreationDto order) 
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if(string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            int generatedOrderId = await _orderService.CreateOrderAsync(userId, order.ShippingAddress);

            return Ok(new { 
                orderId = generatedOrderId,
                message = "Order placed successfully" });
        }


    }
}
