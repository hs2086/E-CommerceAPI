using E_COMMERCEAPI.Models;

namespace E_COMMERCEAPI.Repository
{
    public interface ICartRepo
    {
        Task<List<CartItem>> GetUserCartAsync(string userId);
        Task<CartItem> GetItemAsync(int id);
        Task<CartItem> GetItemByProductAsync(string userId, int productId);
        Task<CartItem> AddItemAsync(CartItem item);
        Task<bool> UpdateItemAsync(CartItem item);
        Task<bool> RemoveItemAsync(int id);
        Task ClearCartAsync(string userId);
    }
}
