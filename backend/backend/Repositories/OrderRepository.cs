using backend.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace backend.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _config;
        public OrderRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> CreateOrderWithItemsAsync(int userId,string shippingAddress,decimal totalAmount,IEnumerable<CartItemResponseDto> cartItems)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                string sqlInsertOrder = @"
                    INSERT INTO Orders (UserId, ShippingAddress, TotalAmount) 
                    VALUES (@UserId, @ShippingAddress, @TotalAmount);
                    SELECT SCOPE_IDENTITY();";

                using var orderCommand = new SqlCommand(sqlInsertOrder, connection, (SqlTransaction)transaction);
                orderCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
                orderCommand.Parameters.Add(new SqlParameter("@ShippingAddress", SqlDbType.NVarChar) { Value = shippingAddress });
                orderCommand.Parameters.Add(new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Value = totalAmount });

                object result = await orderCommand.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                {
                    throw new Exception("Order creation failed; could not retrieve generated OrderId.");
                }

                int newOrderId = Convert.ToInt32(result);

                string sqlInsertOrderItem = @"
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtPurchase)
                    VALUES (@OrderId, @ProductId, @Quantity, @Price)";

                foreach (var item in cartItems)
                {
                    using var itemCommand = new SqlCommand(sqlInsertOrderItem, connection, (SqlTransaction)transaction);
                    itemCommand.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = newOrderId });
                    itemCommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = item.ProductId });
                    itemCommand.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int) { Value = item.Quantity });
                    itemCommand.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Value = item.Price });

                    await itemCommand.ExecuteNonQueryAsync();
                }

                string sqlClearCart = "DELETE FROM CartItems WHERE UserId = @UserId";
                using var clearCartCommand = new SqlCommand(sqlClearCart, connection, (SqlTransaction)transaction);
                clearCartCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

                await clearCartCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return newOrderId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}