using KoiTradding.DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace KoiTradding.DAL.Repositories
{
    public class PaymentRepository
    {
        private readonly KoiFishTradingContext _context;

        public PaymentRepository(KoiFishTradingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreatePaymentAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            ValidatePayment(payment);

            try
            {
                await _context.Payments.AddAsync(payment);
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
                Console.Error.WriteLine($"An error occurred while creating the payment: {ex.Message}");
                return false;
            }
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            if (paymentId <= 0)
                throw new ArgumentException("Payment ID must be greater than zero.", nameof(paymentId));

            try
            {
                return await _context.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while retrieving the payment: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdatePaymentAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            ValidatePayment(payment, isUpdate: true);

            try
            {
                var existingPayment = await _context.Payments.FindAsync(payment.PaymentId);
                if (existingPayment == null)
                    throw new KeyNotFoundException($"Payment with ID {payment.PaymentId} not found.");

                // Update fields
                existingPayment.Amount = payment.Amount;
                existingPayment.PaymentDate = payment.PaymentDate;
                existingPayment.Status = payment.Status;
                existingPayment.TransactionCode = payment.TransactionCode;
                existingPayment.ConsignmentId = payment.ConsignmentId;
                existingPayment.OrderId = payment.OrderId;

                _context.Payments.Update(existingPayment);
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
                Console.Error.WriteLine($"An error occurred while updating the payment: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            if (paymentId <= 0)
                throw new ArgumentException("Payment ID must be greater than zero.", nameof(paymentId));

            try
            {
                var payment = await _context.Payments.FindAsync(paymentId);
                if (payment == null)
                    throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

                _context.Payments.Remove(payment);
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
                Console.Error.WriteLine($"An error occurred while deleting the payment: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            try
            {
                return await _context.Payments
                    .Include(p => p.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while retrieving all payments: {ex.Message}");
                return new List<Payment>();
            }
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
}
