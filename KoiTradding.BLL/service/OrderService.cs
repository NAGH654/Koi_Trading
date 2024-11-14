using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;


namespace KoiTradding.BLL.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;

        public OrderService(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Create Order
        public async Task<bool> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "Order cannot be null");

            // Validate business rules, e.g., order status
            if (string.IsNullOrWhiteSpace(order.Status))
                order.Status = "Pending"; // Default status

            return await _orderRepository.CreateOrderAsync(order);
        }

        // Get Order by ID
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero");

            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        // Update Order
        public async Task<bool> UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "Order cannot be null");

            // Validate fields if necessary
            return await _orderRepository.UpdateOrderAsync(order);
        }

        // Delete Order
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero");

            return await _orderRepository.DeleteOrderAsync(orderId);
        }

        // List All Orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }
    }
}