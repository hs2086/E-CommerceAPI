﻿using E_COMMERCEAPI.Models;

namespace E_COMMERCEAPI.Repository
{
    public interface ICategoryRepo
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<bool> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
    }
}
