using E_COMMERCEAPI.Data;
using E_COMMERCEAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_COMMERCEAPI.Repository
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ECOMMERCEDbContext context;

        public CategoryRepo(ECOMMERCEDbContext _context)
        {
            context = _context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await context.Categories.FindAsync(id);
        }

        public async Task<Category?> CreateAsync(Category category)
        {
            if (category == null)
            {
                return null;
            }

            var entry = await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            if (category == null) return false;

            var existingCategory = await context.Categories.FindAsync(category.Id);
            if (existingCategory == null)
                return false;

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            context.Categories.Update(existingCategory);
            await context.SaveChangesAsync();

            return true;
        } 
        public async Task<bool> DeleteAsync(int id)
        {
            var existingCategory = await context.Categories.FindAsync(id);
            if (existingCategory == null) return false;
            context.Categories.Remove(existingCategory);
            await context.SaveChangesAsync();
            return true;
        } 
    }
}
