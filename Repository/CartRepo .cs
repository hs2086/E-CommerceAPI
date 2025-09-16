using E_COMMERCEAPI.Data;
using E_COMMERCEAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_COMMERCEAPI.Repository
{
    public class CartRepo : ICartRepo
    {
        private readonly ECOMMERCEDbContext context;

        public CartRepo(ECOMMERCEDbContext _context)
        {
            context = _context;
        }

        public async Task<List<CartItem>> GetUserCartAsync(string userId)
        {
            return await context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();
        }

        public async Task<CartItem> GetItemAsync(int id)
            => await context.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<CartItem> GetItemByProductAsync(string userId, int productId)
            => await context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        public async Task<CartItem> AddItemAsync(CartItem item)
        {
            await context.CartItems.AddAsync(item);
            await context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> UpdateItemAsync(CartItem item)
        {
            context.CartItems.Update(item);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItemAsync(int id)
        {
            var item = await context.CartItems.FindAsync(id);
            if (item == null) return false;

            context.CartItems.Remove(item);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task ClearCartAsync(string userId)
        {
            var items = context.CartItems.Where(c => c.UserId == userId);
            context.CartItems.RemoveRange(items);
            await context.SaveChangesAsync();
        }
    }
}
