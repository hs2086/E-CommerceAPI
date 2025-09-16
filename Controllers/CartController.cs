using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_COMMERCEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService cartService;

        public CartController(CartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirst("uid")?.Value;
            var cart = await cartService.GetUserCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDTO request)
        {
            var userId = User.FindFirst("uid")?.Value;
            var item = await cartService.AddOrUpdateAsync(userId, request.ProductId, request.Quantity);
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var deleted = await cartService.RemoveItemAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirst("uid")?.Value;
            await cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}
