using E_COMMERCEAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_COMMERCEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService orderService;

        public OrdersController(OrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst("uid")?.Value;
            try
            {
                var order = await orderService.CreateOrderFromCartAsync(userId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirst("uid")?.Value;
            var orders = await orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] string status)
        {
            var updated = await orderService.UpdateOrderStatusAsync(orderId, status);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpPut("{id}/status-admin")]
        public async Task<IActionResult> UpdateStatusAdmin(int id, [FromBody] string status)
        {
            var updated = await orderService.UpdateStatusAsync(id, status);
            if (!updated) return NotFound(new { message = "Order not found" });

            return NoContent(); 
        }

        [HttpPut("{id}/ship")]
        public async Task<IActionResult> MarkAsShipped(int id, [FromBody] string trackingNumber)
        {
            var updated = await orderService.MarkAsShippedAsync(id, trackingNumber);
            if (!updated) return NotFound(new { message = "Order not found" });

            return NoContent();
        }

        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orders = await orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
    }
}
