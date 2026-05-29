using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(9000, ErrorMessage = "توضیحات نمی‌تواند بیش از 9000 کاراکتر باشد")]
        public string? Description { get; set; } = null;

        public string? ImageUrl { get; set; }   // قبلاً [Required] بوده!

        // کلید خارجی برای والد
        public int? ParentId { get; set; }

        // ناوبری به والد
        [ForeignKey("ParentId")]
        [JsonIgnore]
        public virtual Category? Parent { get; set; }
        [NotMapped]
        // لیست زیردسته‌ها
        public virtual ICollection<Category>? Children { get; set; }

        // محصولات این دسته
        //public virtual ICollection<Product>? Products { get; set; }

        // برای مدیریت سلسله مراتب
        public int Level { get; set; } = 0;
        public string Path { get; set; } = "";// مسیر کامل مانند "1/2/3"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public bool IsActive { get; set; } = true;
       
        public Category()
        {
            Children = new HashSet<Category>();
           
        }
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } = null;

        public string Chekideh { get; set;  }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Quantity { get; set; }
        public string Tags { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public bool IsActive { get; set; } = true;

    }
}