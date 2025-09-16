using E_COMMERCEAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_COMMERCEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService paymentService;
        private readonly OrderService orderService;

        public PaymentsController(PaymentService paymentService, OrderService orderService)
        {
            this.paymentService = paymentService;
            this.orderService = orderService;
        }

        [HttpPost("{orderId}")]
        public async Task<IActionResult> CreatePayment(int orderId)
        {
            var order = await orderService.GetByIdAsync(orderId);
            if (order == null) return NotFound(new { message = "Order not found" });

            if (order.Status != "Pending")
                return BadRequest(new { message = "Order is already paid or canceled" });

            var checkoutUrl = await paymentService.CreateCheckoutSessionAsync(order);
            return Ok(new { url = checkoutUrl });
        }
    }
}
