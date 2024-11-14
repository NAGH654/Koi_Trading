using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTradding.BLL.Services;

public class OrderDetailService
{
    private readonly OrderDetailRepository _orderDetailRepository;

    public OrderDetailService(OrderDetailRepository orderDetailRepository)
    {
        _orderDetailRepository =
            orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
    }


    public async Task<bool> CreateOrderDetailAsync(OrderDetail orderDetail)
    {
        if (orderDetail == null)
            throw new ArgumentNullException(nameof(orderDetail));

        // Business-specific validations
        ValidateOrderDetail(orderDetail);

        // Additional business rules can be added here

        return await _orderDetailRepository.CreateOrderDetailAsync(orderDetail);
    }


    public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId)
    {
        if (orderDetailId <= 0)
            throw new ArgumentException("OrderDetail ID must be greater than zero.", nameof(orderDetailId));

        return await _orderDetailRepository.GetOrderDetailByIdAsync(orderDetailId);
    }


    public async Task<bool> UpdateOrderDetailAsync(OrderDetail orderDetail)
    {
        if (orderDetail == null)
            throw new ArgumentNullException(nameof(orderDetail));

        // Business-specific validations
        ValidateOrderDetail(orderDetail, true);

        // Additional business rules can be added here

        return await _orderDetailRepository.UpdateOrderDetailAsync(orderDetail);
    }


    public async Task<bool> DeleteOrderDetailAsync(int orderDetailId)
    {
        if (orderDetailId <= 0)
            throw new ArgumentException("OrderDetail ID must be greater than zero.", nameof(orderDetailId));

        // Additional business rules can be checked before deletion

        return await _orderDetailRepository.DeleteOrderDetailAsync(orderDetailId);
    }


    public async Task<List<OrderDetail>> GetAllOrderDetailsAsync()
    {
        return await _orderDetailRepository.GetAllOrderDetailsAsync();
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