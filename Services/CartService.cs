using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Models;
using E_COMMERCEAPI.Repository;

namespace E_COMMERCEAPI.Services
{
    public class CartService
    {
        private readonly ICartRepo cartRepo;
        private readonly IProductRepo productRepo;

        public CartService(ICartRepo cartRepo, IProductRepo productRepo)
        {
            this.cartRepo = cartRepo;
            this.productRepo = productRepo;
        }

        public async Task<List<CartItemDto>> GetUserCartAsync(string userId)
        {
            var items = await cartRepo.GetUserCartAsync(userId);
            return items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ImageUrl = i.Product.ImageUrl,
                Price = i.Product.Price,
                Quantity = i.Quantity
            }).ToList();
        }

        public async Task<CartItem> AddOrUpdateAsync(string userId, int productId, int quantity)
        {
            var product = await productRepo.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            var existingItem = await cartRepo.GetItemByProductAsync(userId, productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await cartRepo.UpdateItemAsync(existingItem);
                return existingItem;
            }

            var newItem = new CartItem { UserId = userId, ProductId = productId, Quantity = quantity };
            return await cartRepo.AddItemAsync(newItem);
        }

        public async Task<bool> RemoveItemAsync(int id) => await cartRepo.RemoveItemAsync(id);

        public async Task ClearCartAsync(string userId) => await cartRepo.ClearCartAsync(userId);
    }
}
