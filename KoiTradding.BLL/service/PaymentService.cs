using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTradding.BLL.Services;

public class PaymentService
{
    private readonly PaymentRepository _paymentRepository;

    public PaymentService(PaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    }


    public async Task<bool> CreatePaymentAsync(Payment payment)
    {
        if (payment == null)
            throw new ArgumentNullException(nameof(payment));

        // Business-specific validations
        ValidatePayment(payment);


        return await _paymentRepository.CreatePaymentAsync(payment);
    }


    public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
    {
        if (paymentId <= 0)
            throw new ArgumentException("Payment ID must be greater than zero.", nameof(paymentId));

        return await _paymentRepository.GetPaymentByIdAsync(paymentId);
    }


    public async Task<bool> UpdatePaymentAsync(Payment payment)
    {
        if (payment == null)
            throw new ArgumentNullException(nameof(payment));

        // Business-specific validations
        ValidatePayment(payment, true);

        // Additional business rules can be added here

        return await _paymentRepository.UpdatePaymentAsync(payment);
    }


    public async Task<bool> DeletePaymentAsync(int paymentId)
    {
        if (paymentId <= 0)
            throw new ArgumentException("Payment ID must be greater than zero.", nameof(paymentId));

        // Additional business rules can be checked before deletion

        return await _paymentRepository.DeletePaymentAsync(paymentId);
    }


    public async Task<List<Payment>> GetAllPaymentsAsync()
    {
        return await _paymentRepository.GetAllPaymentsAsync();
    }


    private void ValidatePayment(Payment payment, bool isUpdate = false)
    {
        if (!isUpdate && payment.PaymentId != 0)
            throw new ArgumentException("Payment ID should not be set for a new payment.", nameof(payment.PaymentId));

        if (payment.Amount == null || payment.Amount <= 0)
            throw new ArgumentException("Amount must be provided and greater than zero.", nameof(payment.Amount));

        if (payment.PaymentDate == null)
            throw new ArgumentException("Payment date must be provided.", nameof(payment.PaymentDate));

        if (string.IsNullOrWhiteSpace(payment.Status))
            throw new ArgumentException("Payment status must be provided.", nameof(payment.Status));

        if (string.IsNullOrWhiteSpace(payment.TransactionCode))
            throw new ArgumentException("Transaction code must be provided.", nameof(payment.TransactionCode));

        if (payment.OrderId == null || payment.OrderId <= 0)
            throw new ArgumentException("Order ID must be provided and greater than zero.", nameof(payment.OrderId));
    }
}