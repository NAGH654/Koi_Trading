using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTradding.BLL.Services
{
    public class KoiFishService
    {
        private readonly KoiFishRepository _koiFishRepository;

        public KoiFishService(KoiFishRepository koiFishRepository)
        {
            _koiFishRepository = koiFishRepository ?? throw new ArgumentNullException(nameof(koiFishRepository));
        }

        // Get all KoiFish
        public async Task<List<KoiFish>> GetAllKoiFishAsync()
        {
            return await _koiFishRepository.GetAllAsync();
        }

        // Get a KoiFish by ID
        public async Task<KoiFish?> GetKoiFishByIdAsync(int koiId)
        {
            if (koiId <= 0)
                throw new ArgumentException("Koi ID must be greater than zero");

            return await _koiFishRepository.GetByIdAsync(koiId);
        }

        // Add a new KoiFish
        public async Task<bool> AddKoiFishAsync(KoiFish koiFish)
        {
            if (koiFish == null)
                throw new ArgumentNullException(nameof(koiFish), "KoiFish cannot be null");

            // Business rule validation, e.g., check required fields
            if (string.IsNullOrWhiteSpace(koiFish.Origin))
                throw new ArgumentException("Origin is required.");
            
            if (koiFish.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            return await _koiFishRepository.AddAsync(koiFish);
        }

        // Update an existing KoiFish
        public async Task<bool> UpdateKoiFishAsync(KoiFish koiFish)
        {
            if (koiFish == null)
                throw new ArgumentNullException(nameof(koiFish), "KoiFish cannot be null");

            if (koiFish.KoiId <= 0)
                throw new ArgumentException("Koi ID must be greater than zero");

            // Additional validation or business rules
            return await _koiFishRepository.UpdateAsync(koiFish);
        }

        // Delete a KoiFish by ID
        public async Task<bool> DeleteKoiFishAsync(int koiId)
        {
            if (koiId <= 0)
                throw new ArgumentException("Koi ID must be greater than zero");

            return await _koiFishRepository.DeleteAsync(koiId);
        }

        // Check if a KoiFish exists by ID
        public async Task<bool> KoiFishExistsAsync(int koiId)
        {
            if (koiId <= 0)
                throw new ArgumentException("Koi ID must be greater than zero");

            return await _koiFishRepository.ExistsAsync(koiId);
        }
    }
}
