using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Models;
using E_COMMERCEAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace E_COMMERCEAPI.Services
{
    public class OrderService
    {
        private readonly IOrderRepo orderRepo;
        private readonly ICartRepo cartRepo;
        private readonly IProductRepo productRepo;

        public OrderService(IOrderRepo orderRepo, ICartRepo cartRepo, IProductRepo productRepo)
        {
            this.orderRepo = orderRepo;
            this.cartRepo = cartRepo;
            this.productRepo = productRepo;
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await orderRepo.GetByIdAsync(id);
        }

        public async Task<Order> CreateOrderFromCartAsync(string userId)
        {
            var cartItems = await cartRepo.GetUserCartAsync(userId);
            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            var order = new Order { UserId = userId };
            decimal total = 0;

            foreach (var item in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
                total += item.Product.Price * item.Quantity;

                // Decrease stock
                item.Product.Stock -= item.Quantity;
                await productRepo.UpdateAsync(item.Product);
            }

            order.TotalAmount = total;
            var createdOrder = await orderRepo.CreateAsync(order);

            await cartRepo.ClearCartAsync(userId);

            return createdOrder;
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await orderRepo.GetUserOrdersAsync(userId);
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.CreatedAt,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
            => await orderRepo.UpdateStatusAsync(orderId, status);

        public async Task<List<Order>> GetAllAsync()
        {
            return await orderRepo.GetAll();
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            return await orderRepo.UpdateStatusAsync(id, status);
        }

        public async Task<bool> MarkAsShippedAsync(int orderId, string trackingNumber)
        {
            return await orderRepo.MarkShipped(orderId, trackingNumber);
        }
    }
}
