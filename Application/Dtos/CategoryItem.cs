using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Dtos
{
    public class CategoryItem
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; } = null;

        public string? ImageUrl { get; set; } = null;

        // کلید خارجی برای والد
        public int? ParentId { get; set; }

        // ناوبری به والد
        [ForeignKey("ParentId")]
        [JsonIgnore]
        public virtual CategoryItem? Parent { get; set; }

        // لیست زیردسته‌ها
        public virtual ICollection<CategoryItem>? Children { get; set; }

        // محصولات این دسته
        //public virtual ICollection<Product>? Products { get; set; }

        // برای مدیریت سلسله مراتب
        public int Level { get; set; } = 0;
        public string Path { get; set; } = "";// مسیر کامل مانند "1/2/3"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public bool IsActive { get; set; } = true;



    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}
