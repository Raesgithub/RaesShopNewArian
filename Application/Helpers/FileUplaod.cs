using Application.Dtos;
using Application.Dtos.Cpenel;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Application.Helpers.Cpanel
{
    public class FileUpload
    {
        // بررسی فایل برای IBrowserFile
        public ResultUploadDto CheckBeforeUpload(IBrowserFile file, string pathFolder, List<string> validExt, int size)
        {
            if (file == null)
            {
                return new ResultUploadDto { IsSucces = false, Message = "لطفا فایلی را انتخاب کنید" };
            }

            var ext = Path.GetExtension(file.Name).ToLower();
            if (!validExt.Any(item => item == ext))
            {
                return new ResultUploadDto { IsSucces = false, Message = "فرمت فایل نامعتبر است" };
            }

            if (file.Size / 1024 > size)
            {
                return new ResultUploadDto { IsSucces = false, Message = "حجم فایل بیش از حد مجاز است" };
            }

            pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathFolder);
            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }

            return new ResultUploadDto { IsSucces = true };
        }

        // بررسی فایل برای IFormFile
        public ResultUploadDto CheckBeforeUpload(IFormFile file, string pathFolder, List<string> validExt, int size)
        {
            if (file == null)
            {
                return new ResultUploadDto { IsSucces = false, Message = "لطفا فایلی را انتخاب کنید" };
            }

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!validExt.Any(item => item == ext))
            {
                return new ResultUploadDto { IsSucces = false, Message = "فرمت فایل نامعتبر است" };
            }

            if (file.Length / 1024 > size)
            {
                return new ResultUploadDto { IsSucces = false, Message = "حجم فایل بیش از حد مجاز است" };
            }

            pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathFolder);
            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }

            return new ResultUploadDto { IsSucces = true };
        }

        // فشرده سازی تصویر با کیفیت بالا برای IBrowserFile
        private async Task CompressImage(IBrowserFile file, int width, string path, int quality = 100)
        {
            await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            using var image = await Image.LoadAsync(stream);

            int newHeight = (int)((double)width / image.Width * image.Height);

            // فقط اگر سایز جدید کوچکتر از سایز اصلی است، ریسایز کن
            if (width < image.Width)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, newHeight),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Lanczos3 // بهترین کیفیت
                }));
            }

            // تشخیص فرمت اصلی فایل
            var extension = Path.GetExtension(file.Name).ToLower();

            if (extension == ".png")
            {
                // برای PNG از فرمت PNG با کیفیت lossless استفاده کن
                var pngEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder
                {
                    CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.BestCompression,
                    BitDepth = SixLabors.ImageSharp.Formats.Png.PngBitDepth.Bit8,
                    ColorType = SixLabors.ImageSharp.Formats.Png.PngColorType.RgbWithAlpha
                };
                await using var outputStream = new FileStream(path, FileMode.Create);
                await image.SaveAsync(outputStream, pngEncoder);
            }
            else
            {
                // برای JPEG با کیفیت بالا
                var encoder = new JpegEncoder
                {
                    Quality = quality,
                    ColorType = JpegEncodingColor.YCbCrRatio444 // ✅ Use ColorType instead
                };
                await using var outputStream = new FileStream(path, FileMode.Create);
                await image.SaveAsync(outputStream, encoder);
            }
        }
        // فشرده سازی تصویر با کیفیت بالا برای IFormFile
        private async Task CompressImage(IFormFile file, int width, string path, int quality = 100)
        {
            await using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);

            int newHeight = (int)((double)width / image.Width * image.Height);

            if (width < image.Width)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, newHeight),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Lanczos3
                }));
            }

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".png")
            {
                var pngEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder
                {
                    CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.BestCompression,
                    BitDepth = SixLabors.ImageSharp.Formats.Png.PngBitDepth.Bit8,
                    ColorType = SixLabors.ImageSharp.Formats.Png.PngColorType.RgbWithAlpha
                };
                await using var outputStream = new FileStream(path, FileMode.Create);
                await image.SaveAsync(outputStream, pngEncoder);
            }
            else
            {
                var encoder = new JpegEncoder
                {
                    Quality = quality,
                    ColorType = JpegEncodingColor.YCbCrRatio444 // ✅ Use ColorType instead
                };
                await using var outputStream = new FileStream(path, FileMode.Create);
                await image.SaveAsync(outputStream, encoder);
            }
        }

        // فشرده سازی از استریم
        private async Task CompressImageFromStream(Stream inputStream, int width, string path, int quality = 100)
        {
            inputStream.Position = 0;
            using var image = await Image.LoadAsync(inputStream);

            int newHeight = (int)((double)width / image.Width * image.Height);

            if (width < image.Width)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, newHeight),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Lanczos3
                }));
            }

            var extension = Path.GetExtension(path).ToLower();

            if (extension == ".png")
            {
                var pngEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder
                {
                    CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.BestCompression,
                    BitDepth = SixLabors.ImageSharp.Formats.Png.PngBitDepth.Bit8,
                    ColorType = SixLabors.ImageSharp.Formats.Png.PngColorType.RgbWithAlpha
                };
                await using var outputStream = new FileStream(path, FileMode.Create);
                await image.SaveAsync(outputStream, pngEncoder);
            }
            else
            {
                var encoder = new JpegEncoder
                {
                    Quality = quality,
                    ColorType = JpegEncodingColor.YCbCrRatio444 // ✅ Use ColorType instead
                };
                await using var outputStream = new FileStream(path, FileMode.Create);
                await image.SaveAsync(outputStream, encoder);
            }
        }
        // آپلود چند فایل
        public async Task<List<ResultUploadDto>> UploadMultiple(IReadOnlyList<IBrowserFile> files, string pathFolder)
        {
            var results = new List<ResultUploadDto>();
            foreach (var file in files)
            {
                var result = await UploadAllSize(file, pathFolder);
                results.Add(result);
            }
            return results;
        }

        // آپلود از بایت
        public async Task<ResultUploadDto> UploadFromBytes(byte[] fileData, string fileName, string contentType,
            string pathFolder, bool xxsSize32px = false, bool xsSize64px = false,
            bool smSize320px = false, bool mdSize576px = false, bool lgSize768px = false,
            bool xlSize992px = false, bool xxlSize1200px = false, bool xxxlSize1400px = false,
            int quality = 90)
        {
            var ext = Path.GetExtension(fileName);
            var dt = new PersianDateTime(DateTime.UtcNow.AddHours(3.5));
            var newFilename = $"{new Random().Next(10000)}{dt.Year}{dt.Month}{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{ext}";

            pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathFolder);

            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }

            using var memoryStream = new MemoryStream(fileData);

            if (xxsSize32px)
                await CompressImageFromStream(memoryStream, 32, Path.Combine(pathFolder, "xxs-" + newFilename), quality);
            if (xsSize64px)
                await CompressImageFromStream(memoryStream, 64, Path.Combine(pathFolder, "xs-" + newFilename), quality);
            if (smSize320px)
                await CompressImageFromStream(memoryStream, 320, Path.Combine(pathFolder, "sm-" + newFilename), quality);
            if (mdSize576px)
                await CompressImageFromStream(memoryStream, 576, Path.Combine(pathFolder, "md-" + newFilename), quality);
            if (lgSize768px)
                await CompressImageFromStream(memoryStream, 768, Path.Combine(pathFolder, "lg-" + newFilename), quality);
            if (xlSize992px)
                await CompressImageFromStream(memoryStream, 992, Path.Combine(pathFolder, "xl-" + newFilename), quality);
            if (xxlSize1200px)
                await CompressImageFromStream(memoryStream, 1200, Path.Combine(pathFolder, "xxl-" + newFilename), quality);
            if (xxxlSize1400px)
                await CompressImageFromStream(memoryStream, 1400, Path.Combine(pathFolder, "xxxl-" + newFilename), quality);

            return new ResultUploadDto
            {
                IsSucces = true,
                Message = "فایل با موفقیت آپلود گردید",
                Filename = Path.GetFileName(newFilename)
            };
        }

        // آپلود برای IBrowserFile با کیفیت پیشفرض
        public async Task<ResultUploadDto> UploadBrowserFile(IBrowserFile file, string pathFolder,
            bool xxsSize32px = false, bool xsSize64px = false,
            bool smSize320px = false, bool mdSize576px = false, bool lgSize768px = false,
            bool xlSize992px = false, bool xxlSize1200px = false, bool xxxlSize1400px = false)
        {
            return await UploadBrowserFile(file, pathFolder, xxsSize32px, xsSize64px, smSize320px, mdSize576px,
                lgSize768px, xlSize992px, xxlSize1200px, xxxlSize1400px, 90);
        }

        // آپلود برای IBrowserFile با کیفیت دلخواه
        public async Task<ResultUploadDto> UploadBrowserFile(IBrowserFile file, string pathFolder,
            bool xxsSize32px, bool xsSize64px, bool smSize320px, bool mdSize576px,
            bool lgSize768px, bool xlSize992px, bool xxlSize1200px, bool xxxlSize1400px,
            int quality)
        {
            var ext = Path.GetExtension(file.Name);
            var dt = new PersianDateTime(DateTime.UtcNow.AddHours(3.5));
            var newFilename = $"{new Random().Next(10000)}{dt.Year}{dt.Month}{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{ext}";

            pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathFolder);

            if (xxsSize32px)
                await CompressImage(file, 32, Path.Combine(pathFolder, "xxs-" + newFilename), quality);
            if (xsSize64px)
                await CompressImage(file, 64, Path.Combine(pathFolder, "xs-" + newFilename), quality);
            if (smSize320px)
                await CompressImage(file, 320, Path.Combine(pathFolder, "sm-" + newFilename), quality);
            if (mdSize576px)
                await CompressImage(file, 576, Path.Combine(pathFolder, "md-" + newFilename), quality);
            if (lgSize768px)
                await CompressImage(file, 768, Path.Combine(pathFolder, "lg-" + newFilename), quality);
            if (xlSize992px)
                await CompressImage(file, 992, Path.Combine(pathFolder, "xl-" + newFilename), quality);
            if (xxlSize1200px)
                await CompressImage(file, 1200, Path.Combine(pathFolder, "xxl-" + newFilename), quality);
            if (xxxlSize1400px)
                await CompressImage(file, 1400, Path.Combine(pathFolder, "xxxl-" + newFilename), quality);

            return new ResultUploadDto
            {
                IsSucces = true,
                Message = "فایل با موفقیت آپلود گردید",
                Filename = Path.GetFileName(newFilename)
            };
        }

        // آپلود برای IFormFile با کیفیت پیشفرض
        public async Task<ResultUploadDto> UploadFormFile(IFormFile file, string pathFolder,
            bool xxsSize32px = false, bool xsSize64px = false,
            bool smSize320px = false, bool mdSize576px = false, bool lgSize768px = false,
            bool xlSize992px = false, bool xxlSize1200px = false, bool xxxlSize1400px = false)
        {
            return await UploadFormFile(file, pathFolder, xxsSize32px, xsSize64px, smSize320px, mdSize576px,
                lgSize768px, xlSize992px, xxlSize1200px, xxxlSize1400px, 90);
        }

        // آپلود برای IFormFile با کیفیت دلخواه
        public async Task<ResultUploadDto> UploadFormFile(IFormFile file, string pathFolder,
            bool xxsSize32px, bool xsSize64px, bool smSize320px, bool mdSize576px,
            bool lgSize768px, bool xlSize992px, bool xxlSize1200px, bool xxxlSize1400px,
            int quality)
        {
            var ext = Path.GetExtension(file.FileName);
            var dt = new PersianDateTime(DateTime.UtcNow.AddHours(3.5));
            var newFilename = $"{new Random().Next(10000)}{dt.Year}{dt.Month}{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{ext}";

            pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathFolder);

            if (xxsSize32px)
                await CompressImage(file, 32, Path.Combine(pathFolder, "xxs-" + newFilename), quality);
            if (xsSize64px)
                await CompressImage(file, 64, Path.Combine(pathFolder, "xs-" + newFilename), quality);
            if (smSize320px)
                await CompressImage(file, 320, Path.Combine(pathFolder, "sm-" + newFilename), quality);
            if (mdSize576px)
                await CompressImage(file, 576, Path.Combine(pathFolder, "md-" + newFilename), quality);
            if (lgSize768px)
                await CompressImage(file, 768, Path.Combine(pathFolder, "lg-" + newFilename), quality);
            if (xlSize992px)
                await CompressImage(file, 992, Path.Combine(pathFolder, "xl-" + newFilename), quality);
            if (xxlSize1200px)
                await CompressImage(file, 1200, Path.Combine(pathFolder, "xxl-" + newFilename), quality);
            if (xxxlSize1400px)
                await CompressImage(file, 1400, Path.Combine(pathFolder, "xxxl-" + newFilename), quality);

            return new ResultUploadDto
            {
                IsSucces = true,
                Message = "فایل با موفقیت آپلود گردید",
                Filename = Path.GetFileName(newFilename)
            };
        }

        // حذف فایل
        public ResultUploadDto Remove(string pathFolder, string filename,
            bool xxsSize32px = false, bool xsSize64px = false,
            bool smSize320px = false, bool mdSize576px = false, bool lgSize768px = false,
            bool xlSize992px = false, bool xxlSize1200px = false, bool xxxlSize1400px = false)
        {
            pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathFolder);

            var sizes = new List<(bool ShouldDelete, string Prefix)>
            {
                (xxsSize32px, "xxs-"),
                (xsSize64px, "xs-"),
                (smSize320px, "sm-"),
                (mdSize576px, "md-"),
                (lgSize768px, "lg-"),
                (xlSize992px, "xl-"),
                (xxlSize1200px, "xxl-"),
                (xxxlSize1400px, "xxxl-")
            };

            foreach (var size in sizes.Where(s => s.ShouldDelete))
            {
                try
                {
                    var filePath = Path.Combine(pathFolder, size.Prefix + filename);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                catch (Exception)
                {
                    // در صورت بروز خطا، عملیات حذف برای این سایز ادامه پیدا می‌کند
                }
            }

            return new ResultUploadDto
            {
                IsSucces = true,
                Message = "فایل حذف گردید"
            };
        }

        // آپلود با تمام سایزها برای IBrowserFile
        // آپلود با تمام سایزها برای IBrowserFile
        public async Task<ResultUploadDto> Upload(IBrowserFile file, string pathFolder,
       bool xxsSize32px = false, bool xsSize64px = false,
       bool smSize320px = false, bool mdSize576px = false, bool lgSize768px = false,
       bool xlSize992px = false, bool xxlSize1200px = false, bool xxxlSize1400px = false)
        {
            return await UploadBrowserFile(file, pathFolder, xxsSize32px, xsSize64px,
                smSize320px, mdSize576px, lgSize768px, xlSize992px, xxlSize1200px, xxxlSize1400px, 90);
        }

        public async Task<ResultUploadDto> Upload(IFormFile file, string pathFolder,
            bool xxsSize32px = false, bool xsSize64px = false,
            bool smSize320px = false, bool mdSize576px = false, bool lgSize768px = false,
            bool xlSize992px = false, bool xxlSize1200px = false, bool xxxlSize1400px = false)
        {
            return await UploadFormFile(file, pathFolder, xxsSize32px, xsSize64px,
                smSize320px, mdSize576px, lgSize768px, xlSize992px, xxlSize1200px, xxxlSize1400px, 90);
        }

        public async Task<ResultUploadDto> UploadAllSize(IBrowserFile file, string pathFolder)
        {
            return await Upload(file, pathFolder, true, true, true, true, true, true, true, true);
        }

        public async Task<ResultUploadDto> UploadAllSize(IFormFile file, string pathFolder)
        {
            return await Upload(file, pathFolder, true, true, true, true, true, true, true, true);
        }
    }
}