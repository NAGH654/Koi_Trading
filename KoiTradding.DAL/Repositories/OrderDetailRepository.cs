using KoiTradding.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace KoiTradding.DAL.Repositories;

public class OrderDetailRepository
{
    private readonly KoiFishTradingContext _context;

    public OrderDetailRepository(KoiFishTradingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> CreateOrderDetailAsync(OrderDetail orderDetail)
    {
        if (orderDetail == null)
            throw new ArgumentNullException(nameof(orderDetail));

        ValidateOrderDetail(orderDetail);

        try
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException dbEx)
        {
            Console.Error.WriteLine($"Database Update Error: {dbEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while creating the order detail: {ex.Message}");
            return false;
        }
    }

    public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId)
    {
        if (orderDetailId <= 0)
            throw new ArgumentException("OrderDetail ID must be greater than zero.", nameof(orderDetailId));

        try
        {
            return await _context.OrderDetails
                .Include(od => od.Koi)
                .Include(od => od.Order)
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while retrieving the order detail: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateOrderDetailAsync(OrderDetail orderDetail)
    {
        if (orderDetail == null)
            throw new ArgumentNullException(nameof(orderDetail));

        ValidateOrderDetail(orderDetail, true);

        try
        {
            var existingOrderDetail = await _context.OrderDetails.FindAsync(orderDetail.OrderDetailId);
            if (existingOrderDetail == null)
                throw new KeyNotFoundException($"OrderDetail with ID {orderDetail.OrderDetailId} not found.");

            // Update fields
            existingOrderDetail.OrderId = orderDetail.OrderId;
            existingOrderDetail.KoiId = orderDetail.KoiId;
            existingOrderDetail.BatchId = orderDetail.BatchId;
            existingOrderDetail.Type = orderDetail.Type;
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.Price = orderDetail.Price;

            _context.OrderDetails.Update(existingOrderDetail);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException concEx)
        {
            Console.Error.WriteLine($"Concurrency error: {concEx.Message}");
            return false;
        }
        catch (DbUpdateException dbEx)
        {
            Console.Error.WriteLine($"Database Update Error: {dbEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while updating the order detail: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteOrderDetailAsync(int orderDetailId)
    {
        if (orderDetailId <= 0)
            throw new ArgumentException("OrderDetail ID must be greater than zero.", nameof(orderDetailId));

        try
        {
            var orderDetail = await _context.OrderDetails.FindAsync(orderDetailId);
            if (orderDetail == null)
                throw new KeyNotFoundException($"OrderDetail with ID {orderDetailId} not found.");

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException dbEx)
        {
            Console.Error.WriteLine($"Database Update Error: {dbEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while deleting the order detail: {ex.Message}");
            return false;
        }
    }

    public async Task<List<OrderDetail>> GetAllOrderDetailsAsync()
    {
        try
        {
            return await _context.OrderDetails
                .Include(od => od.Koi)
                .Include(od => od.Order)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while retrieving all order details: {ex.Message}");
            return new List<OrderDetail>();
        }
    }

    private void ValidateOrderDetail(OrderDetail orderDetail, bool isUpdate = false)
    {
        if (!isUpdate && orderDetail.OrderDetailId != 0)
            throw new ArgumentException("OrderDetail ID should not be set for a new order detail.",
                nameof(orderDetail.OrderDetailId));

        if (orderDetail.OrderId == null || orderDetail.OrderId <= 0)
            throw new ArgumentException("Order ID must be provided and greater than zero.",
                nameof(orderDetail.OrderId));

        if (orderDetail.KoiId == null || orderDetail.KoiId <= 0)
            throw new ArgumentException("Koi ID must be provided and greater than zero.", nameof(orderDetail.KoiId));

        if (orderDetail.Quantity == null || orderDetail.Quantity <= 0)
            throw new ArgumentException("Quantity must be provided and greater than zero.",
                nameof(orderDetail.Quantity));

        if (orderDetail.Price == null || orderDetail.Price <= 0)
            throw new ArgumentException("Price must be provided and greater than zero.", nameof(orderDetail.Price));
    }
}