using E_COMMERCEAPI.Models;
using Stripe.Checkout;

namespace E_COMMERCEAPI.Services
{
    public class PaymentService
    {
        public async Task<string> CreateCheckoutSessionAsync(Order order)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = "https://your-frontend.com/payment-success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://your-frontend.com/payment-cancel",
                LineItems = order.Items.Select(i => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(i.UnitPrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = i.Product.Name,
                        }
                    },
                    Quantity = i.Quantity
                }).ToList()
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }
    }
}
