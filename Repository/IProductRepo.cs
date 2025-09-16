using E_COMMERCEAPI.Models;

namespace E_COMMERCEAPI.Repository
{
    public interface IProductRepo
    {
        Task<List<Product>> GetAllAsync(int categoryId);
        Task<Product> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
