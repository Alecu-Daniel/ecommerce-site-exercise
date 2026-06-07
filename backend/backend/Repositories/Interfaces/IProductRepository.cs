using backend.Dtos;

namespace backend.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}