using E_COMMERCEAPI.Data;
using E_COMMERCEAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_COMMERCEAPI.Repository
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ECOMMERCEDbContext context;

        public OrderRepo(ECOMMERCEDbContext _context)
        {
            context = _context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var order = await context.Orders.FindAsync(id);
            if (order == null) return false;

            order.Status = status;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Order>> GetAll()
        {
            return await context.Orders.ToListAsync();
        }

        public async Task<bool> MarkShipped(int orderId, string trackingNumber)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Paid") return false;

            order.Status = "Shipped";
            order.ShippedAt = DateTime.UtcNow;
            order.TrackingNumber = trackingNumber;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
