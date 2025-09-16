using E_COMMERCEAPI.Data;
using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_COMMERCEAPI.Services
{
    public class ReviewService: IReviewService
    {
        private readonly ECOMMERCEDbContext context;

        public ReviewService(ECOMMERCEDbContext context)
        {
            this.context = context;
        }

        public async Task<Review?> CreateAsync(int productId, string userId, ReviewDto dto)
        {
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();
            return review;
        }

        public async Task<List<Review>> GetByProductIdAsync(int productId, bool includeUnapproved = false)
        {
            var query = context.Reviews.Where(r => r.ProductId == productId);
            if (!includeUnapproved)
                query = query.Where(r => r.IsApproved);

            return await query.ToListAsync();
        }

        public async Task<bool> ApproveAsync(int reviewId)
        {
            var review = await context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            review.IsApproved = true;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int reviewId, string? requestingUserId = null)
        {
            var review = await context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            if (requestingUserId != null && review.UserId != requestingUserId)
                return false;

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Review?> GetByIdAsync(int reviewId)
        {
            return await context.Reviews.FindAsync(reviewId);
        }
    }
}
