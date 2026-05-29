using Application.Dtos;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RaesShopNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                var result = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Chekideh = p.Chekideh,  // ✅ اضافه شد
                    ImageUrls = p.ImageUrls ?? new List<string>(),
                    Quantity = p.Quantity,
                    IsActive = p.IsActive,
                    Tags = !string.IsNullOrEmpty(p.Tags) ?
                           p.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() :
                           new List<string>(),
                    CreatedAt = p.CreatedAt
                }).ToList();

                return Ok(new ApiResponse<List<ProductDto>>
                {
                    Success = true,
                    Data = result,
                    Message = "دریافت محصولات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ProductDto>>
                {
                    Success = false,
                    Message = "خطا در دریافت محصولات",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "محصول یافت نشد"
                    });

                var categoryIds = await _productService.GetProductCategoryIdsAsync(id);

                var result = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    Chekideh = product.Chekideh,  // ✅ اضافه شد
                    ImageUrls = product.ImageUrls ?? new List<string>(),  // ✅ اصلاح شد
                    Quantity = product.Quantity,
                    IsActive = product.IsActive,
                    Tags = !string.IsNullOrEmpty(product.Tags) ?
                           product.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() :
                           new List<string>(),
                    CategoryIds = categoryIds ?? new List<int>(),
                    CreatedAt = product.CreatedAt
                };

                return Ok(new ApiResponse<ProductDto>
                {
                    Success = true,
                    Data = result,
                    Message = "دریافت محصول با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = "خطا در دریافت محصول",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchProducts([FromQuery] string q)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(q);
                var result = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Chekideh = p.Chekideh,  // ✅ اضافه شد
                    ImageUrls = p.ImageUrls ?? new List<string>(),
                    Quantity = p.Quantity,
                    IsActive = p.IsActive,
                    Tags = !string.IsNullOrEmpty(p.Tags) ?
                           p.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() :
                           new List<string>(),
                    CreatedAt = p.CreatedAt
                }).ToList();

                return Ok(new ApiResponse<List<ProductDto>>
                {
                    Success = true,
                    Data = result,
                    Message = "جستجوی محصولات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ProductDto>>
                {
                    Success = false,
                    Message = "خطا در جستجوی محصولات",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "داده‌های ورودی معتبر نیستند",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description ?? string.Empty,
                    Chekideh = productDto.Chekideh ?? string.Empty,  // ✅ اضافه شد
                    ImageUrls = productDto.ImageUrls ?? new List<string>(),  // ✅ اصلاح شد
                    Quantity = productDto.Quantity,
                    Tags = productDto.Tags != null && productDto.Tags.Any() ?
                           string.Join(",", productDto.Tags) : string.Empty,
                    IsActive = productDto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _productService.CreateProductAsync(product);

                // ذخیره دسته‌بندی‌های محصول
                if (productDto.CategoryIds?.Any() == true)
                {
                    await _productService.SetProductCategoriesAsync(created.Id, productDto.CategoryIds);
                }

                // دریافت مجدد دسته‌بندی‌ها برای نمایش
                var categoryIds = await _productService.GetProductCategoryIdsAsync(created.Id);

                var result = new ProductDto
                {
                    Id = created.Id,
                    Name = created.Name,
                    Price = created.Price,
                    Description = created.Description,
                    Chekideh = created.Chekideh,  // ✅ اضافه شد
                    ImageUrls = created.ImageUrls ?? new List<string>(),
                    Quantity = created.Quantity,
                    IsActive = created.IsActive,
                    Tags = !string.IsNullOrEmpty(created.Tags) ?
                           created.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() :
                           new List<string>(),
                    CategoryIds = categoryIds ?? new List<int>(),
                    CreatedAt = created.CreatedAt
                };

                return Ok(new ApiResponse<ProductDto>
                {
                    Success = true,
                    Data = result,
                    Message = "محصول با موفقیت ایجاد شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = "خطا در ایجاد محصول",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            try
            {
                var existing = await _productService.GetProductByIdAsync(id);
                if (existing == null)
                    return NotFound(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "محصول یافت نشد"
                    });

                // به‌روزرسانی محصول
                existing.Name = productDto.Name;
                existing.Price = productDto.Price;
                existing.Description = productDto.Description ?? string.Empty;
                existing.Chekideh = productDto.Chekideh ?? string.Empty;  // ✅ اضافه شد
                existing.ImageUrls = productDto.ImageUrls ?? new List<string>();  // ✅ اصلاح شد
                existing.Quantity = productDto.Quantity;
                existing.Tags = productDto.Tags != null && productDto.Tags.Any() ?
                                string.Join(",", productDto.Tags) : string.Empty;
                existing.IsActive = productDto.IsActive;

                var updated = await _productService.UpdateProductAsync(id, existing);

                // به‌روزرسانی دسته‌بندی‌های محصول
                if (productDto.CategoryIds != null)
                {
                    await _productService.SetProductCategoriesAsync(id, productDto.CategoryIds);
                }

                if (updated == null)
                    return NotFound(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "محصول یافت نشد"
                    });

                // دریافت مجدد دسته‌بندی‌ها برای نمایش
                var categoryIds = await _productService.GetProductCategoryIdsAsync(updated.Id);

                var result = new ProductDto
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Price = updated.Price,
                    Description = updated.Description,
                    Chekideh = updated.Chekideh,  // ✅ اضافه شد
                    ImageUrls = updated.ImageUrls ?? new List<string>(),
                    Quantity = updated.Quantity,
                    IsActive = updated.IsActive,
                    Tags = !string.IsNullOrEmpty(updated.Tags) ?
                           updated.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() :
                           new List<string>(),
                    CategoryIds = categoryIds ?? new List<int>(),
                    CreatedAt = updated.CreatedAt
                };

                return Ok(new ApiResponse<ProductDto>
                {
                    Success = true,
                    Data = result,
                    Message = "محصول با موفقیت ویرایش شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = "خطا در ویرایش محصول",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "محصول یافت نشد"
                    });

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "محصول با موفقیت حذف شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "خطا در حذف محصول",
                    Error = ex.Message
                });
            }
        }

        [HttpPatch("restore/{id}")]
        public async Task<ActionResult> RestoreProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "محصول یافت نشد"
                    });

                if (!product.IsActive)
                {
                    product.IsActive = true;
                    var updated = await _productService.UpdateProductAsync(id, product);
                    if (updated != null)
                    {
                        return Ok(new ApiResponse<bool>
                        {
                            Success = true,
                            Data = true,
                            Message = "محصول با موفقیت بازیابی شد"
                        });
                    }
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "محصول قبلاً فعال است"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "خطا در بازیابی محصول",
                    Error = ex.Message
                });
            }
        }
    }

    // DTO کلاس‌ها
    public class CreateProductDto
    {
        [Required(ErrorMessage = "نام محصول الزامی است")]
        [StringLength(200, ErrorMessage = "نام محصول نمی‌تواند بیش از 200 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "قیمت الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "قیمت باید عدد مثبت باشد")]
        public decimal Price { get; set; }

        public string? Description { get; set; }
        public string? Chekideh { get; set; }  // ✅ اضافه شد
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int Quantity { get; set; } = 0;
        public List<string> Tags { get; set; } = new List<string>();
        public bool IsActive { get; set; } = true;
        public List<int> CategoryIds { get; set; } = new List<int>();
    }

    public class ProductDto : CreateProductDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? Error { get; set; }
        public List<string>? Errors { get; set; }
    }
}