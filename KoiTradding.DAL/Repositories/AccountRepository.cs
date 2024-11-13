using KoiTradding.DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace KoiTradding.DAL.Repositories
{
    public class AccountRepository
    {
        private readonly KoiFishTradingContext _context;

        public AccountRepository(KoiFishTradingContext context)
        {
            _context = context;
        }

        // Create Account
        public async Task<bool> CreateAccountAsync(Account account)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(account.Email))
                    throw new ArgumentException("Email cannot be null or empty");

                if (string.IsNullOrWhiteSpace(account.Password))
                    throw new ArgumentException("Password cannot be null or empty");

                if (_context.Accounts.Any(a => a.Email == account.Email))
                    throw new ArgumentException("An account with the given email already exists");

                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Read Account by ID
        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        // Read Account by Email
        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        }

        // Update Account
        public async Task<bool> UpdateAccountAsync(Account account)
        {
            try
            {
                var existingAccount = await _context.Accounts.FindAsync(account.AccountId);
                if (existingAccount == null)
                    throw new ArgumentException("Account not found");

                // Validation
                if (string.IsNullOrWhiteSpace(account.Email))
                    throw new ArgumentException("Email cannot be null or empty");

                existingAccount.Email = account.Email;
                existingAccount.Password = account.Password;
                existingAccount.FullName = account.FullName;
                existingAccount.Phone = account.Phone;
                existingAccount.RoleId = account.RoleId;
                existingAccount.Status = account.Status;
                existingAccount.Address = account.Address;

                _context.Accounts.Update(existingAccount);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Delete Account
        public async Task<bool> DeleteAccountAsync(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null)
                    throw new ArgumentException("Account not found");

                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Login
        public async Task<Account?> LoginAsync(string email, string password)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        // List All Accounts
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }
    }
}
