using E_COMMERCEAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_COMMERCEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            this.wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var wishlist = await wishlistService.GetWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await wishlistService.AddToWishlistAsync(userId, productId);
            if (!result) return BadRequest(new { message = "Product already in wishlist or does not exist" });

            return Ok(new { message = "Product added to wishlist" });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await wishlistService.RemoveFromWishlistAsync(userId, productId);
            if (!result) return NotFound(new { message = "Product not found in wishlist" });

            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await wishlistService.ClearWishlistAsync(userId);
            return NoContent();
        }
    }
}
