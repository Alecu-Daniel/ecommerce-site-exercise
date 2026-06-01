using backend.DataContext;
using backend.Dtos;
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
        private readonly SqlDataContext _data;
        private readonly ICartService _cartService;
        public CheckoutController(SqlDataContext data, ICartService cartService)
        {
            _data = data;
            _cartService = cartService;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(OrderCreationDto order)
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if(string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            string sqlGetCart = @"SELECT ci.ProductId, ci.Quantity, p.Price
                                  FROM CartItems AS ci
                                  JOIN Products AS p ON ci.ProductId = p.ProductId
                                  WHERE ci.UserId = @UserId";

            List<SqlParameter> getCartParameters = new List<SqlParameter>() { new SqlParameter("UserId", System.Data.SqlDbType.Int) {Value = userId} };
            var cartItems = await _data.LoadDataWithParametersAsync(sqlGetCart, getCartParameters, reader => new
            {

                ProductId = (int)reader["ProductId"],
                Quantity = (int)reader["Quantity"],
                Price = (decimal)reader["Price"]
            });

            if (cartItems.Count == 0) return BadRequest("Cart is empty");

            decimal totalAmount = _cartService.CalculateTotal(cartItems.Select(item => (item.Quantity, item.Price)));

            string sqlInsertOrder = @"INSERT INTO Orders (UserId, ShippingAddress, TotalAmount) 
                             VALUES (@UserId, @Address, @Total)";

            List<SqlParameter> insertOrderParameters = new List<SqlParameter>
            {
                new SqlParameter("UserId",System.Data.SqlDbType.Int) {Value = userId},
                new SqlParameter("Address",System.Data.SqlDbType.NVarChar) {Value = order.ShippingAddress},
                new SqlParameter("Total",System.Data.SqlDbType.Decimal) {Value = totalAmount},
            };

            await _data.ExecuteSqlWithParametersAsync(sqlInsertOrder, insertOrderParameters);

            string sqlGetOrderId = "SELECT MAX(OrderId) as OrderId FROM Orders WHERE UserId = @UserId";
            List<SqlParameter> getOrderIdParameters = new List<SqlParameter> { new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userId } };
            int orderId = await _data.LoadDataSingleWithParametersAsync(sqlGetOrderId, getOrderIdParameters, reader => (int)reader["OrderId"]);



            foreach (var item in cartItems)
            {
                string sqlInsertOrderItem = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtPurchase)
                                          VALUES (@OrderId, @ProductId, @Quantity, @Price)";
                var insertOrderItemParameters = new List<SqlParameter> {
                    new SqlParameter("OrderId", System.Data.SqlDbType.Int) { Value = orderId},
                    new SqlParameter("ProductId", System.Data.SqlDbType.Int) { Value = item.ProductId},
                    new SqlParameter("Quantity", System.Data.SqlDbType.Int) { Value = item.Quantity},
                    new SqlParameter("Price", System.Data.SqlDbType.Decimal) { Value = item.Price}
                };
                await _data.ExecuteSqlWithParametersAsync(sqlInsertOrderItem, insertOrderItemParameters);
            }

            string sqlClearCart = "DELETE FROM CartItems WHERE UserId = @UserId";
            List<SqlParameter> clearCartParameters = new List<SqlParameter> {new SqlParameter("UserId",System.Data.SqlDbType.Int) {Value = userId} };
            await _data.ExecuteSqlWithParametersAsync(sqlClearCart, clearCartParameters);

            return Ok(new { message = "Order placed successfully" });
        }


    }
}
