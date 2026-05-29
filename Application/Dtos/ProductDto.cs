using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام محصول الزامی است")]
        [StringLength(100, ErrorMessage = "نام محصول نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        // حذف CategoryId و اضافه کردن CategoryIds
        public List<int> CategoryIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "قیمت الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "قیمت باید عدد مثبت باشد")]
        public decimal Price { get; set; }
        [Required]
        public string Chekideh{ get; set; }
        public int Quantity { get; set; } = 0;

        [StringLength(9000, ErrorMessage = "توضیحات نمی‌تواند بیش از 9000 کاراکتر باشد")]
        public string Description { get; set; } = string.Empty;

        public List<string> ImageUrls { get; set; } = new List<string>();
        public string  ImageUrl  { get; set; }
        // لیست تگ‌ها (برچسب‌ها)
        public List<string> Tags { get; set; } = new List<string>();

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
