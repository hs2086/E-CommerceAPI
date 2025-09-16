using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Models;
using E_COMMERCEAPI.Repository;

namespace E_COMMERCEAPI.Services
{
    public class ProductService
    {
        private readonly IProductRepo productRepo;

        public ProductService(IProductRepo productRepo)
        {
            this.productRepo = productRepo;
        }

        public async Task<List<ProductDto>> GetProductsAsync(int categoryId)
        {
            var products = await productRepo.GetAllAsync(categoryId);
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name
            }).ToList();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var result = await productRepo.GetByIdAsync(id);
            if (result == null)
            {
                return new ProductDto { Name = "No Name" };
            }
            else
            {
                return new ProductDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Description = result.Description,
                    Price = result.Price,
                    ImageUrl = result.ImageUrl,
                    CategoryName = result.Category.Name
                };
            }
        }

        public async Task<Product> CreateAsync(Product product) => await productRepo.CreateAsync(product);

        public async Task<bool> UpdateAsync(Product product) => await productRepo.UpdateAsync(product);

        public async Task<bool> DeleteAsync(int id) => await productRepo.DeleteAsync(id);
    }
}
