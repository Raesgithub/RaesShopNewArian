using Domain.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);

        Task<List<Category>> GetRootCategoriesAsync();
        Task<List<Category>> GetChildCategoriesAsync(int parentId);
        Task<List<Category>> GetCategoryTreeAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int id, Category category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<Category>> GetCategoryPathAsync(int categoryId);
    }

    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;
        static CategoryService()
        {
            // فعال‌سازی DailyFileChecker
            //System.Runtime.CompilerServices.RuntimeHelpers
            //    .RunClassConstructor(typeof(DailyFileChecker).TypeHandle);
        }
        public CategoryService(DataContext context)
        {

            _context = context;
        }


        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            SimpleBackgroundChecker.TriggerCheck();
            return await _context.Categories
                .Include(c => c.Parent)
                .Include(c => c.Children.Where(ch => ch.IsActive))
                //  .Include(c => c.Products.Where(p => p.IsActive))
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Parent)
                .Include(c => c.Children.Where(ch => ch.IsActive))
                // .Include(c => c.Products.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<List<Category>> GetRootCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsActive))
                .Where(c => c.ParentId == null && c.IsActive)
                .ToListAsync();
        }

        public async Task<List<Category>> GetChildCategoriesAsync(int parentId)
        {
            return await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsActive))
                .Where(c => c.ParentId == parentId && c.IsActive)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoryTreeAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            return BuildCategoryTree(categories);
        }

        private List<Category> BuildCategoryTree(List<Category> categories, int? parentId = null)
        {
            return categories
                .Where(c => c.ParentId == parentId)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    ParentId = c.ParentId,
                    Level = c.Level,
                    Path = c.Path,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Children = BuildCategoryTree(categories, c.Id)
                    //Products = c.Products?.Where(p => p.IsActive).ToList() ?? new List<Product>()
                })
                .ToList();
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            // اعتبارسنجی والد
            if (category.ParentId.HasValue)
            {
                var parent = await _context.Categories.FindAsync(category.ParentId.Value);
                if (parent == null || !parent.IsActive)
                {
                    throw new ArgumentException("دسته‌بندی والد یافت نشد یا غیرفعال است");
                }
            }

            category.CreatedAt = DateTime.UtcNow;
            category.IsActive = true;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // محاسبه Level و Path پس از گرفتن ID
            if (category.ParentId.HasValue)
            {
                var parent = await _context.Categories.FindAsync(category.ParentId.Value);
                category.Level = parent.Level + 1;
                category.Path = $"{parent.Path}/{category.Id}";
            }
            else
            {
                category.Level = 0;
                category.Path = category.Id.ToString();
            }

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null || !existingCategory.IsActive)
                return null;

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.ImageUrl = category.ImageUrl;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsActive))
                //.Include(c => c.Products.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return false;

            // اگر دسته‌بندی دارای زیردسته یا محصول فعال است، نمی‌توان حذف کرد
            if (category.Children.Any())
            {
                // غیرفعال کردن به جای حذف
                category.IsActive = false;
                category.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetCategoryPathAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null || string.IsNullOrEmpty(category.Path) || !category.IsActive)
                return new List<Category>();

            var pathIds = category.Path.Split('/').Select(int.Parse).ToList();
            return await _context.Categories
                .Where(c => pathIds.Contains(c.Id) && c.IsActive)
                .OrderBy(c => c.Level)
                .ToListAsync();
        }
    }
}