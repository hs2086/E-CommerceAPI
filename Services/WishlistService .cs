using E_COMMERCEAPI.Data;
using E_COMMERCEAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_COMMERCEAPI.Services
{
    public class WishlistService: IWishlistService
    {
        private readonly ECOMMERCEDbContext context;

        public WishlistService(ECOMMERCEDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Product>> GetWishlistAsync(string userId)
        {
            return await context.WishlistItems
                .Where(w => w.UserId == userId)
                .Select(w => w.Product)
                .ToListAsync();
        }

        public async Task<bool> AddToWishlistAsync(string userId, int productId)
        {
            var productExists = await context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists) return false;

            bool alreadyExists = await context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
            if (alreadyExists) return false;

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            };

            await context.WishlistItems.AddAsync(wishlistItem);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlistItem = await context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null) return false;

            context.WishlistItems.Remove(wishlistItem);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearWishlistAsync(string userId)
        {
            var wishlistItems = await context.WishlistItems
                .Where(w => w.UserId == userId)
                .ToListAsync();

            if (!wishlistItems.Any()) return false;

            context.WishlistItems.RemoveRange(wishlistItems);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
