using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace E_COMMERCEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpPost("product/{productId}")]
        public async Task<IActionResult> AddReview(int productId, [FromBody] ReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var created = await reviewService.CreateAsync(productId, userId, dto);
            if (created == null) return BadRequest(new { message = "Failed to create review" });

            return Ok(created);
        }

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var reviews = await reviewService.GetByProductIdAsync(productId);
            return Ok(reviews);
        }
    }
}
