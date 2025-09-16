using E_COMMERCEAPI.Models;

namespace E_COMMERCEAPI.Repository
{
    public interface IOrderRepo
    {
        Task<Order> CreateAsync(Order order);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<Order> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<List<Order>> GetAll();
        Task<bool> MarkShipped(int orderId, string trackingNumber);
    }
}
