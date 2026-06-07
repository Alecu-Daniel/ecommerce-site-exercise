using backend.DataContext;
using backend.Dtos;
using backend.Models;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SqlDataContext _data;
        public ProductRepository(SqlDataContext data)
        {
            _data = data;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            string sql = "SELECT ProductId, Name, Description, Price, ImageUrl FROM Products";

            return await _data.LoadDataWithParametersAsync(sql, null, reader => new ProductDto
            {
                ProductId = (int)reader["ProductId"],
                Name = reader["Name"].ToString() ?? "",
                Description = reader["Description"].ToString() ?? "",
                Price = (decimal)reader["Price"],
                ImageUrl = reader["ImageUrl"].ToString() ?? ""
            });
        }
    }
}
