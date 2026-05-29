using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BasketBag
    {

        [Key]
        public int ProductId { get; set; }
        public string Code { get; set; } = string.Empty;      // کد محصول
        public string Title { get; set; } = string.Empty;     // عنوان محصول
        public int Quantity { get; set; }                     // تعداد
        public decimal Price { get; set; }                    // قیمت فروش
        public string? ImageUrl { get; set; }                 // برای نمایش تصویر کوچک
    }
}
