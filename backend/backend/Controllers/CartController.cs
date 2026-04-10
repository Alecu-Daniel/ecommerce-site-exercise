using backend.DataContext;
using backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly SqlDataContext _data;
        public CartController(SqlDataContext data)
        {
            _data = data;
        }


        [HttpPost("AddToCart")]
        public IActionResult AddToCart(CartItemUpsertDto cartItem)
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            string sqlcheckIfProductInCart = "SELECT * FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";

            List<SqlParameter> checkProductParameters = new List<SqlParameter>
            {
                new SqlParameter("UserId",System.Data.SqlDbType.Int){ Value = userId },
                new SqlParameter("ProductId",System.Data.SqlDbType.Int) { Value = cartItem.ProductId}
            };

            IEnumerable<int> existingProduct = _data.LoadDataWithParameters<int>(sqlcheckIfProductInCart, checkProductParameters, reader => (int)reader["ProductId"]);

            if (existingProduct.Count() >= 1)
            {
                string sqlUpdateProduct = @"UPDATE CartItems SET Quantity = Quantity + @Quantity WHERE ProductId = @ProductId AND UserId = @UserId;
                                            DELETE FROM CartItems WHERE Quantity <= 0 AND UserId = @UserId AND ProductId = @ProductId";

                List<SqlParameter> updateProductParameters = new List<SqlParameter>
                {
                    new SqlParameter("Quantity",System.Data.SqlDbType.Int) {Value = cartItem.Quantity},
                    new SqlParameter("ProductId",System.Data.SqlDbType.Int) {Value = cartItem.ProductId},
                    new SqlParameter("UserId",System.Data.SqlDbType.Int) {Value = userId}
                };

                _data.ExecuteSqlWithParameters(sqlUpdateProduct, updateProductParameters);

                return Ok();
            }
            else
            {
                string sqlInsertProduct = "INSERT INTO CartItems (UserId,ProductId,Quantity) VALUES (@UserId,@ProductId,@Quantity)";

                List<SqlParameter> insertProductParameters = new List<SqlParameter> {
                    new SqlParameter("UserId",System.Data.SqlDbType.Int) {Value = userId},
                    new SqlParameter("ProductId",System.Data.SqlDbType.Int) {Value = cartItem.ProductId},
                    new SqlParameter("Quantity",System.Data.SqlDbType.Int) {Value = cartItem.Quantity}
                };

                _data.ExecuteSqlWithParameters(sqlInsertProduct, insertProductParameters);
                return Ok();
            }

            throw new Exception("Failed to add item to cart");

        }


        [HttpGet("GetCart")]
        public IEnumerable<CartItemResponseDto> GetCart()
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new Exception("Failed to display cart");
            int userId = int.Parse(userIdClaim);

            string getAllCartProducts = @"SELECT ci.ProductId,ci.Quantity,p.Name,p.Price
                                          FROM CartItems AS ci 
                                          JOIN Products AS p ON p.ProductId = ci.ProductId 
                                          WHERE ci.UserId = @UserId";
            List<SqlParameter> allCartProductsParameters = new List<SqlParameter>
            {
                new SqlParameter("UserId",System.Data.SqlDbType.Int) {Value = userId}
            };

            return _data.LoadDataWithParameters(getAllCartProducts, allCartProductsParameters, reader => new CartItemResponseDto
            {
                ProductId = (int)reader["ProductId"],
                Quantity = (int)reader["Quantity"],
                Title = reader["Name"].ToString() ?? "",
                Price = (decimal)reader["Price"],
            });
        }

        [HttpGet("GetTotal")]
        public CartTotalDto GetTotal()
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new Exception("Failed to calculate cart total");
            int userId = int.Parse(userIdClaim);

            string sqlGetUsersProducts = @"SELECT ci.Quantity, p.Price
                                        FROM CartItems AS ci
                                        JOIN Products AS p ON ci.ProductId = p.ProductId
                                        WHERE ci.UserId = @UserId";
            List<SqlParameter> usersProductsParameters = new List<SqlParameter>
            {
                new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userId }
            };

            IEnumerable<(int Quantity, decimal Price)> cartData = _data.LoadDataWithParameters(sqlGetUsersProducts, usersProductsParameters, reader => (

                (int)reader["Quantity"],
                (decimal)reader["Price"]
            ));

            decimal totalCartCost = 0;

            foreach (var item in cartData)
            {
                totalCartCost += item.Price * item.Quantity;
            }

            return new CartTotalDto { Total = totalCartCost };
        }

        [HttpDelete("RemoveItem/{productId}")]
        public IActionResult RemoveItem(int productId)
        {
            string? userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            string sqlDeleteItem = "DELETE FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";
            List<SqlParameter> deleteItemsParameters = new List<SqlParameter>
            {
                new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userId },
                new SqlParameter("ProductId", System.Data.SqlDbType.Int) { Value = productId }
            };

            if (_data.ExecuteSqlWithParameters(sqlDeleteItem, deleteItemsParameters))
                return Ok();
            else throw new Exception("Failed to remove item");



        }
    }
}