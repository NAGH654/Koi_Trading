using KoiTradding.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace KoiTradding.DAL.Repositories
{
    public class OrderRepository
    {
        private readonly KoiFishTradingContext _context;

        public OrderRepository(KoiFishTradingContext context)
        {
            _context = context;
        }

        // Create Order
        public async Task<bool> CreateOrderAsync(Order order)
        {
            try
            {
                // Validation
                if (order.AccountId == null || order.AccountId <= 0)
                    throw new ArgumentException("Account ID must be provided and greater than zero.");
                
                if (order.TotalPrice == null || order.TotalPrice <= 0)
                    throw new ArgumentException("Total price must be provided and greater than zero.");

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Read Order by ID
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // Update Order
        public async Task<bool> UpdateOrderAsync(Order order)
        {
            try
            {
                var existingOrder = await _context.Orders.FindAsync(order.OrderId);
                if (existingOrder == null)
                    throw new ArgumentException("Order not found");

                existingOrder.TotalPrice = order.TotalPrice;
                existingOrder.Status = order.Status;
                existingOrder.OrderDate = order.OrderDate;

                _context.Orders.Update(existingOrder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Delete Order
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    throw new ArgumentException("Order not found");

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // List All Orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }
    }
}
