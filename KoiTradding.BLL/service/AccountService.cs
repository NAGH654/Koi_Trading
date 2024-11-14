using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KoiTradding.BLL.Services
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;

        public AccountService(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Create Account
        public async Task<bool> CreateAccountAsync(Account account)
        {
            try
            {
                // Business Logic Validation
                if (string.IsNullOrWhiteSpace(account.Email))
                    throw new ArgumentException("Email cannot be null or empty");

                if (string.IsNullOrWhiteSpace(account.Password))
                    throw new ArgumentException("Password cannot be null or empty");

                if (account.Password.Length < 6)
                    throw new ArgumentException("Password must be at least 6 characters long");

                // Set default role to "Customer" (RoleId = 3) if not provided
                account.RoleId ??= 3;

                return await _accountRepository.CreateAccountAsync(account);
            }
            catch (Exception ex)
            {
                // Log exception or handle it
                Console.WriteLine($"Error creating account: {ex.Message}");
                return false;
            }
        }

        // Get Account by ID
        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            return await _accountRepository.GetAccountByIdAsync(id);
        }

        // Get Account by Email
        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _accountRepository.GetAccountByEmailAsync(email);
        }

        // Update Account
        public async Task<bool> UpdateAccountAsync(Account account)
        {
            try
            {
                // Business Logic Validation
                if (string.IsNullOrWhiteSpace(account.Email))
                    throw new ArgumentException("Email cannot be null or empty");

                if (string.IsNullOrWhiteSpace(account.Password))
                    throw new ArgumentException("Password cannot be null or empty");

                // Ensure role is set if updating account role or defaults to 3 if not set
                account.RoleId ??= 3;

                return await _accountRepository.UpdateAccountAsync(account);
            }
            catch (Exception ex)
            {
                // Log exception or handle it
                Console.WriteLine($"Error updating account: {ex.Message}");
                return false;
            }
        }

        // Delete Account
        public async Task<bool> DeleteAccountAsync(int id)
        {
            try
            {
                return await _accountRepository.DeleteAccountAsync(id);
            }
            catch (Exception ex)
            {
                // Log exception or handle it
                Console.WriteLine($"Error deleting account: {ex.Message}");
                return false;
            }
        }

        // Login
        public async Task<Account?> LoginAsync(string email, string password)
        {
            try
            {
                // Business Logic Validation
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be null or empty");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password cannot be null or empty");

                return await _accountRepository.LoginAsync(email, password);
            }
            catch (Exception ex)
            {
                // Log exception or handle it
                Console.WriteLine($"Error logging in: {ex.Message}");
                return null;
            }
        }

        // Get All Accounts
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAccountsAsync();
        }
       
        // Check if an email is already registered
        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _accountRepository.IsEmailExistsAsync(email);
        }
    }
}
