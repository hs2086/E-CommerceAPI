using E_COMMERCEAPI.Models;

namespace E_COMMERCEAPI.Services
{
    public interface IWishlistService
    {
        Task<List<Product>> GetWishlistAsync(string userId);
        Task<bool> AddToWishlistAsync(string userId, int productId);
        Task<bool> RemoveFromWishlistAsync(string userId, int productId);
        Task<bool> ClearWishlistAsync(string userId);
    }
}
