using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Models;

namespace E_COMMERCEAPI.Services
{
    public interface IReviewService
    {
        Task<Review?> CreateAsync(int productId, string userId, ReviewDto dto);
        Task<List<Review>> GetByProductIdAsync(int productId, bool includeUnapproved = false);
        Task<bool> ApproveAsync(int reviewId);
        Task<bool> DeleteAsync(int reviewId, string? requestingUserId = null);
        Task<Review?> GetByIdAsync(int reviewId);
    }
}
