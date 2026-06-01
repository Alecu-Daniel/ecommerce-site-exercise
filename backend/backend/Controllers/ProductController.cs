using backend.DataContext;
using backend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly SqlDataContext _data;
        public ProductController(SqlDataContext data)
        {
            _data = data;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductDto>> GetProducts()
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
