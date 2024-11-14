using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTradding.BLL.Services;

public class CategoryService
{
    private readonly CategoryRepository _categoryRepository;

    public CategoryService(CategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<List<Category>> GetAllCategories()
    {
        return await _categoryRepository.GetAllAsync();
    }
}