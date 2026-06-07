using backend.DataContext;
using backend.Dtos;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backend.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly SqlDataContext _data;

        public CartRepository(SqlDataContext data)
        {
            _data = data;
        }

        public async Task<IEnumerable<CartItemResponseDto>> GetUserCartAsync(int userId)
        {
            string sql = @"
                SELECT ci.ProductId, ci.Quantity, p.Name, p.Price
                FROM CartItems AS ci
                JOIN Products AS p ON p.ProductId = ci.ProductId
                WHERE ci.UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };

            return await _data.LoadDataWithParametersAsync(sql, parameters, reader => new CartItemResponseDto
            {
                ProductId = (int)reader["ProductId"],
                Quantity = (int)reader["Quantity"],
                Title = reader["Name"].ToString() ?? "",
                Price = (decimal)reader["Price"],
            });
        }

        public async Task<IEnumerable<(int Quantity, decimal Price)>> GetCartItemsWithPricesAsync(int userId)
        {
            const string sql = @"
                SELECT ci.Quantity, p.Price
                FROM CartItems AS ci
                JOIN Products AS p ON ci.ProductId = p.ProductId
                WHERE ci.UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };

            return await _data.LoadDataWithParametersAsync(sql, parameters, reader => (
                (int)reader["Quantity"],
                (decimal)reader["Price"]
            ));
        }

        public async Task<bool> UpsertCartItemAsync(int userId, CartItemUpsertDto cartItem)
        {
            
            const string sqlCheck = "SELECT ProductId FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";

            var checkParameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
                new SqlParameter("@ProductId", SqlDbType.Int) { Value = cartItem.ProductId }
            };

            var existingProduct = await _data.LoadDataWithParametersAsync<int>(sqlCheck, checkParameters, reader => (int)reader["ProductId"]);

            if (existingProduct.Any())
            {
                
                const string sqlUpdate = @"
                    UPDATE CartItems SET Quantity = Quantity + @Quantity WHERE ProductId = @ProductId AND UserId = @UserId;
                    DELETE FROM CartItems WHERE Quantity <= 0 AND UserId = @UserId AND ProductId = @ProductId";

                var updateParameters = new List<SqlParameter>
                {
                    new SqlParameter("@Quantity", SqlDbType.Int) { Value = cartItem.Quantity },
                    new SqlParameter("@ProductId", SqlDbType.Int) { Value = cartItem.ProductId },
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
                };

                return await _data.ExecuteSqlWithParametersAsync(sqlUpdate, updateParameters);
            }
            else
            {
                const string sqlInsert = "INSERT INTO CartItems (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";

                var insertParameters = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
                    new SqlParameter("@ProductId", SqlDbType.Int) { Value = cartItem.ProductId },
                    new SqlParameter("@Quantity", SqlDbType.Int) { Value = cartItem.Quantity }
                };

                return await _data.ExecuteSqlWithParametersAsync(sqlInsert, insertParameters);
            }
        }

        public async Task<bool> RemoveCartItemAsync(int userId, int productId)
        {
            const string sql = "DELETE FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
                new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId }
            };

            return await _data.ExecuteSqlWithParametersAsync(sql, parameters);
        }

    }
}
