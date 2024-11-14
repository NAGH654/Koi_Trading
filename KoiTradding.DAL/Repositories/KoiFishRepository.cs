using KoiTradding.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiTradding.DAL.Repositories
{
    public class KoiFishRepository
    {
        private readonly KoiFishTradingContext _context;

        public KoiFishRepository(KoiFishTradingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get all KoiFish asynchronously
        public async Task<List<KoiFish>> GetAllAsync()
        {
            try
            {
                return await _context.KoiFishes.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all KoiFish: {ex.Message}");
                return new List<KoiFish>();
            }
        }

        // Get a KoiFish by ID asynchronously
        public async Task<KoiFish?> GetByIdAsync(int koiId)
        {
            try
            {
                return await _context.KoiFishes
                    .Include(k => k.Category)
                    .Include(k => k.Certificate)
                    .FirstOrDefaultAsync(k => k.KoiId == koiId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching KoiFish with ID {koiId}: {ex.Message}");
                return null;
            }
        }

        // Add a new KoiFish asynchronously
        public async Task<bool> AddAsync(KoiFish koiFish)
        {
            try
            {
                if (koiFish == null) throw new ArgumentNullException(nameof(koiFish));

                await _context.KoiFishes.AddAsync(koiFish);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding KoiFish: {ex.Message}");
                return false;
            }
        }

        // Update an existing KoiFish asynchronously
        public async Task<bool> UpdateAsync(KoiFish koiFish)
        {
            try
            {
                var existingKoiFish = await _context.KoiFishes.FindAsync(koiFish.KoiId);
                if (existingKoiFish == null) throw new ArgumentException("KoiFish not found");

                // Update properties
                existingKoiFish.Origin = koiFish.Origin;
                existingKoiFish.Gender = koiFish.Gender;
                existingKoiFish.Age = koiFish.Age;
                existingKoiFish.Size = koiFish.Size;
                existingKoiFish.Status = koiFish.Status;
                existingKoiFish.Price = koiFish.Price;
                existingKoiFish.CategoryId = koiFish.CategoryId;
                existingKoiFish.Health = koiFish.Health;
                existingKoiFish.KoiImage = koiFish.KoiImage;

                _context.KoiFishes.Update(existingKoiFish);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating KoiFish: {ex.Message}");
                return false;
            }
        }

        // Delete a KoiFish by ID asynchronously
        public async Task<bool> DeleteAsync(int koiId)
        {
            try
            {
                var koiFish = await _context.KoiFishes.FindAsync(koiId);
                if (koiFish == null) throw new ArgumentException("KoiFish not found");

                _context.KoiFishes.Remove(koiFish);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting KoiFish with ID {koiId}: {ex.Message}");
                return false;
            }
        }

        // Check if a KoiFish exists by ID
        public async Task<bool> ExistsAsync(int koiId)
        {
            try
            {
                return await _context.KoiFishes.AnyAsync(k => k.KoiId == koiId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if KoiFish exists with ID {koiId}: {ex.Message}");
                return false;
            }
        }
    }
}
