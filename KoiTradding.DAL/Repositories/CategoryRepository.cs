using KoiTradding.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace KoiTradding.DAL.Repositories;

public class CategoryRepository
{
    private readonly KoiFishTradingContext _context;

    public CategoryRepository(KoiFishTradingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Category>> GetAllAsync()
    {
        try
        {
            return await _context.Categories.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching all KoiFish: {ex.Message}");
            return new List<Category>();
        }
    }
}