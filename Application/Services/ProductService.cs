using Domain.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Application.Services
{
    public interface IProductService
    {
        Task<List<int>> GetProductCategoryIdsAsync(int productId);
        Task<bool> SetProductCategoriesAsync(int productId, List<int> categoryIds);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> SearchProductsAsync(string searchTerm);

    }

    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<int>> GetProductCategoryIdsAsync(int productId)
        {
            // دریافت IDs دسته‌بندی‌های یک محصول
            return await _context.ProductCategories
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.CategoryId)
                .ToListAsync();
        }
        public async Task<bool> SetProductCategoriesAsync(int productId, List<int> categoryIds)
        {
            try
            {
                // حذف دسته‌بندی‌های قبلی
                var existingCategories = await _context.ProductCategories
                    .Where(pc => pc.ProductId == productId)
                    .ToListAsync();

                _context.ProductCategories.RemoveRange(existingCategories);

                // اضافه کردن دسته‌بندی‌های جدید
                if (categoryIds != null && categoryIds.Any())
                {
                    var newCategories = categoryIds.Select(categoryId => new ProductCategory
                    {
                        ProductId = productId,
                        CategoryId = categoryId
                    });

                    await _context.ProductCategories.AddRangeAsync(newCategories);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                //.Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id); // ✅ همه محصولات، حتی غیرفعال
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                product.IsActive = true;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception x)
            {

                throw;
            }

        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null || !existingProduct.IsActive)
                return null;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description ?? string.Empty;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.IsActive = product.IsActive;
            existingProduct.Tags = product.Tags; // ✅ این خط را اضافه کنید
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllProductsAsync();

            return await _context.Products
                .Where(p => p.IsActive &&
                    (p.Name.Contains(searchTerm) ||
                     (p.Description != null && p.Description.Contains(searchTerm))))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}