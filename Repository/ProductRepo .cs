using E_COMMERCEAPI.Data;
using E_COMMERCEAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_COMMERCEAPI.Repository
{
    public class ProductRepo : IProductRepo
    {
        private readonly ECOMMERCEDbContext context;

        public ProductRepo(ECOMMERCEDbContext _context)
        {
            context = _context;
        }

        public async Task<List<Product>> GetAllAsync(int categoryId)
        {
            return await context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
            => await context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product> CreateAsync(Product product)
        {
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return false;

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
