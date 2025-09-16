using E_COMMERCEAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace E_COMMERCEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminReviewsController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public AdminReviewsController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveReview(int id)
        {
            var result = await reviewService.ApproveAsync(id);
            if (!result) return NotFound(new { message = "Review not found" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await reviewService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Review not found" });

            return NoContent();
        }
    }
}
