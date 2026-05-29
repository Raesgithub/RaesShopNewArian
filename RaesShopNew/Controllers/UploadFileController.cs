using Application.Helpers.Cpanel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RaesShopNew.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)] // از sawagger حذف میکند
    //[IgnoreAntiforgeryToken]
    //[AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class UploadFileController : ControllerBase
    {
        [HttpGet("images/shop/{imageName}")]
        public IActionResult GetShopImage(string imageName)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "shop", imageName);

            if (!System.IO.File.Exists(imagePath))
                return NotFound();

            var image = System.IO.File.OpenRead(imagePath);
            var contentType = GetContentType(imageName);

            return File(image, contentType);
        }
    
        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        [HttpPost("uploadimage")]
        
        public async Task<IActionResult> UploadImage([FromForm] IFormFile upload) // تغییر به "upload"
        {
            if (upload == null || upload.Length == 0)
                return BadRequest(new { error = new { message = "فایل یافت نشد" } });

            FileUpload fileUpload = new FileUpload();

            var check = fileUpload.CheckBeforeUpload(upload, "images/shop",
                new List<string> { ".jpg", ".png", ".jpeg" }, 4000);

            if (!check.IsSucces)
                return BadRequest(new { error = new { message = check.Message } });

            var res = await fileUpload.Upload(upload, "images/shop", xlSize992px: true);

            if (!res.IsSucces)
                return BadRequest(new { error = new { message = res.Message } });

            // پاسخ مورد نیاز CKEditor
            return Ok(new
            {
                uploaded = true,
                url = $"/images/shop/xl-{res.Filename}"
            });
        }
        [HttpPost("uploadimagefile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file) // تغییر از IBrowserFile به IFormFile
        {
            if (file == null || file.Length == 0)
                return Ok(new { uploaded = false, url = "فایل یافت نشد" });

            FileUpload fileUpload = new FileUpload();

            // بررسی فایل قبل از آپلود
            // توجه: متد CheckBeforeUpload باید پارامتر IFormFile قبول کند
            var check = fileUpload.CheckBeforeUpload(file, "images/shop",
                                                     new List<string> { ".jpg", ".png", ".jpeg" }, 4000);
            if (!check.IsSucces)
                return Ok(new { uploaded = false, url = check.Message });

            // آپلود فایل
            var res = await fileUpload.Upload(file, "images/shop", xlSize992px: true);
            if (!res.IsSucces)
                return Ok(new { uploaded = false, url = res.Message });

            return Ok(new { uploaded = true, url = $"/images/shop/xl-{res.Filename}" });
        }

    }
}
