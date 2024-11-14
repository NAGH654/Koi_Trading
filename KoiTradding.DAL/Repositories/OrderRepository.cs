using KoiTradding.DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace KoiTradding.DAL.Repositories
{
    public class OrderRepository
    {
        private readonly KoiFishTradingContext _context;

        public OrderRepository(KoiFishTradingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        
        public async Task<bool> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            ValidateOrder(order);

            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                // Log exception (consider using a logging framework)
                Console.Error.WriteLine($"Database Update Error: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.Error.WriteLine($"An error occurred while creating the order: {ex.Message}");
                return false;
            }
        }

       
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));

            try
            {
                return await _context.Orders
                    .Include(o => o.Account)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Koi)
                    .Include(o => o.Payments)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.Error.WriteLine($"An error occurred while retrieving the order: {ex.Message}");
                return null;
            }
        }

       
        public async Task<bool> UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            ValidateOrder(order, isUpdate: true);

            try
            {
                var existingOrder = await _context.Orders.FindAsync(order.OrderId);
                if (existingOrder == null)
                    throw new KeyNotFoundException($"Order with ID {order.OrderId} not found.");

                // Update fields
                existingOrder.AccountId = order.AccountId;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.TotalPrice = order.TotalPrice;
                existingOrder.Status = order.Status;

                _context.Orders.Update(existingOrder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                // Handle concurrency issues
                Console.Error.WriteLine($"Concurrency error: {concEx.Message}");
                return false;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update issues
                Console.Error.WriteLine($"Database Update Error: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.Error.WriteLine($"An error occurred while updating the order: {ex.Message}");
                return false;
            }
        }

       
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Payments)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");

                // Remove related entities if necessary
                _context.OrderDetails.RemoveRange(order.OrderDetails);
                _context.Payments.RemoveRange(order.Payments);
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update issues
                Console.Error.WriteLine($"Database Update Error: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.Error.WriteLine($"An error occurred while deleting the order: {ex.Message}");
                return false;
            }
        }

       
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Account)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Koi)
                    .Include(o => o.Payments)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                Console.Error.WriteLine($"An error occurred while retrieving all orders: {ex.Message}");
                return new List<Order>();
            }
        }

      
        private void ValidateOrder(Order order, bool isUpdate = false)
        {
            if (!isUpdate && order.OrderId != 0)
                throw new ArgumentException("Order ID should not be set for a new order.", nameof(order.OrderId));

            if (order.AccountId == null || order.AccountId <= 0)
                throw new ArgumentException("Account ID must be provided and greater than zero.", nameof(order.AccountId));

            if (order.TotalPrice == null || order.TotalPrice <= 0)
                throw new ArgumentException("Total price must be provided and greater than zero.", nameof(order.TotalPrice));

            if (string.IsNullOrWhiteSpace(order.Status))
                throw new ArgumentException("Order status must be provided.", nameof(order.Status));

            
        }
    }
}
