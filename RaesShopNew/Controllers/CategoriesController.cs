using Application.Dtos;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RaesShopNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateCategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var model = categories.Select(s => new CreateCategoryDto
                {
                    Description = s.Description,
                    Id = s.Id,
                    ImageUrl = s.ImageUrl,
                    Name = s.Name,
                    ParentId = s.ParentId
                }).ToList();
                return Ok(new
                {
                    success = true,
                    data = model,
                    message = "دریافت اطلاعات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "دسته‌بندی یافت نشد"
                    });

                return Ok(new
                {
                    success = true,
                    data = category,
                    message = "دریافت اطلاعات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات",
                    error = ex.Message
                });
            }
        }

        [HttpGet("root")]
        public async Task<ActionResult<IEnumerable<CategoryItem>>> GetRootCategories()
        {
            try
            {
                var categories = await _categoryService.GetRootCategoriesAsync();
                return Ok(new
                {
                    success = true,
                    data = categories,
                    message = "دریافت اطلاعات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{parentId}/children")]
        public async Task<ActionResult<IEnumerable<Category>>> GetChildCategories(int parentId)
        {
            try
            {
                var categories = await _categoryService.GetChildCategoriesAsync(parentId);
                return Ok(new
                {
                    success = true,
                    data = categories,
                    message = "دریافت اطلاعات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات",
                    error = ex.Message
                });
            }
        }

        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoryTree()
        {
            try
            {
                var tree = await _categoryService.GetCategoryTreeAsync();
                return Ok(new
                {
                    success = true,
                    data = tree,
                    message = "دریافت اطلاعات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}/path")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoryPath(int id)
        {
            try
            {
                var path = await _categoryService.GetCategoryPathAsync(id);
                return Ok(new
                {
                    success = true,
                    data = path,
                    message = "دریافت اطلاعات با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات",
                    error = ex.Message
                });
            }
        }

        // ADD - ایجاد دسته‌بندی جدید

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            try
            {
                var category = new Category
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    ImageUrl = categoryDto.ImageUrl,
                    ParentId = categoryDto.ParentId
                };

                var created = await _categoryService.CreateCategoryAsync(category);

                var resultDto = new CreateCategoryDto
                {
                    Id = created.Id,
                    Name = created.Name,
                    Description = created.Description,
                    ImageUrl = created.ImageUrl,
                    ParentId = created.ParentId
                };

                return Ok(new
                {
                    success = true,
                    data = resultDto,
                    message = "دسته‌بندی با موفقیت ایجاد شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        // EDIT - ویرایش دسته‌بندی
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto categoryDto)
        {
            try
            {
                var existing = await _categoryService.GetCategoryByIdAsync(id);
                if (existing == null)
                    return NotFound(new { success = false, message = "دسته‌بندی یافت نشد" });

                existing.Name = categoryDto.Name;
                existing.Description = categoryDto.Description;
                existing.ImageUrl = categoryDto.ImageUrl; // اگر null باشه، پاک میشه

                await _categoryService.UpdateCategoryAsync(id, existing);

                return Ok(new { success = true, message = "دسته‌بندی با موفقیت ویرایش شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // DELETE - حذف دسته‌بندی
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                // اول چک کن فرزند داره یا نه
                var hasChildren = await _categoryService.GetChildCategoriesAsync(id);
                if (hasChildren.Any())
                    return BadRequest(new { success = false, message = "این دسته‌بندی دارای زیرمجموعه است و قابل حذف نیست." });

                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "دسته‌بندی یافت نشد" });

                return Ok(new { success = true, message = "دسته‌بندی با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // RESTORE - بازیابی دسته‌بندی غیرفعال
        [HttpPatch("restore/{id}")]
        public async Task<ActionResult> RestoreCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "دسته‌بندی یافت نشد"
                    });

                // اگر دسته‌بندی غیرفعال است، آن را فعال کنیم
                if (!category.IsActive)
                {
                    category.IsActive = true;
                    category.UpdatedAt = DateTime.UtcNow;

                    var updated = await _categoryService.UpdateCategoryAsync(id, category);
                    if (updated != null)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "دسته‌بندی با موفقیت بازیابی شد"
                        });
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = "دسته‌بندی قبلاً فعال است"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در بازیابی دسته‌بندی",
                    error = ex.Message
                });
            }
        }
    }

    // DTO برای ایجاد دسته‌بندی


    // DTO برای ویرایش دسته‌بندی
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "نام دسته‌بندی الزامی است")]
        [StringLength(100, ErrorMessage = "نام نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")]
        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}
