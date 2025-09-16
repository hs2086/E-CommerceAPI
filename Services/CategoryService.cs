using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Models;
using E_COMMERCEAPI.Repository;

namespace E_COMMERCEAPI.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepo categoryRepo;

        public CategoryService(ICategoryRepo categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }

        public async Task<List<CategoryDto>> GetCategories() 
        {
            var result = await categoryRepo.GetAllAsync();

            return result.Select(
                item => new CategoryDto
                {
                    Name = item.Name,
                    Description = item.Description
                }
            ).ToList();
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var result = await categoryRepo.GetByIdAsync(id);
            if(result == null)
            {
                return new CategoryDto { Name = "No Name", Description = "No Description" };
            }
            return new CategoryDto { Name = result.Name, Description =  result.Description };
        }

        public async Task<Category> CreateAsync(Category category)
        {
            var result = await categoryRepo.CreateAsync(category);
            return result;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existing = await categoryRepo.GetByIdAsync(category.Id);
            if (existing == null)
                return false;

            existing.Name = category.Name;
            existing.Description = category.Description;

            return await categoryRepo.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await categoryRepo.GetByIdAsync(id);
            if (existing == null)
                return false;

            return await categoryRepo.DeleteAsync(id);
        }
    }
}
