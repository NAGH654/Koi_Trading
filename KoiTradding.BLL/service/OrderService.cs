using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTradding.BLL.Services;

public class OrderService
{
    private readonly OrderRepository _orderRepository;

    public OrderService(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }


    public async Task<bool> CreateOrderAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        // Business-specific validations
        if (order.OrderDetails == null || order.OrderDetails.Count == 0)
            throw new ArgumentException("An order must have at least one order detail.", nameof(order.OrderDetails));

        // Calculate TotalPrice if not set
        if (!order.TotalPrice.HasValue || order.TotalPrice <= 0)
        {
            decimal calculatedTotal = 0;
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Price == null || detail.Quantity == null)
                    throw new ArgumentException("Each order detail must have a valid price and quantity.");

                calculatedTotal += detail.Price.Value * detail.Quantity.Value;
            }

            order.TotalPrice = calculatedTotal;
        }

        // Set OrderDate if not set
        if (!order.OrderDate.HasValue)
            order.OrderDate = DateTime.UtcNow;


        // Proceed to create the order using the repository
        return await _orderRepository.CreateOrderAsync(order);
    }


    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        if (orderId <= 0)
            throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));

        return await _orderRepository.GetOrderByIdAsync(orderId);
    }


    public async Task<bool> UpdateOrderAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        // Business-specific validations
        if (order.OrderDetails == null || order.OrderDetails.Count == 0)
            throw new ArgumentException("An order must have at least one order detail.", nameof(order.OrderDetails));

        // Recalculate TotalPrice if necessary
        decimal calculatedTotal = 0;
        foreach (var detail in order.OrderDetails)
        {
            if (detail.Price == null || detail.Quantity == null)
                throw new ArgumentException("Each order detail must have a valid price and quantity.");

            calculatedTotal += detail.Price.Value * detail.Quantity.Value;
        }

        order.TotalPrice = calculatedTotal;

        // Additional business rules can be added here

        // Proceed to update the order using the repository
        return await _orderRepository.UpdateOrderAsync(order);
    }


    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        if (orderId <= 0)
            throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));

        // Additional business rules can be checked before deletion

        return await _orderRepository.DeleteOrderAsync(orderId);
    }


    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllOrdersAsync();
    }


    private void ValidateOrder(Order order, bool isUpdate = false)
    {
        if (!isUpdate && order.OrderId != 0)
            throw new ArgumentException("Order ID should not be set for a new order.", nameof(order.OrderId));

        if (order.AccountId == null || order.AccountId <= 0)
            throw new ArgumentException("Account ID must be provided and greater than zero.", nameof(order.AccountId));

        if (order.TotalPrice == null || order.TotalPrice <= 0)
            throw new ArgumentException("Total price must be provided and greater than zero.",
                nameof(order.TotalPrice));

        if (string.IsNullOrWhiteSpace(order.Status))
            throw new ArgumentException("Order status must be provided.", nameof(order.Status));
    }
}