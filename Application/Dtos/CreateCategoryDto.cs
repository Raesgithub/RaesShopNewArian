using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Dtos
{
    public class CreateCategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام دسته‌بندی الزامی است")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // این خط رو از Required دربیار یا ? بذار
        public string? ImageUrl { get; set; }   // اینجا قبلاً Required بوده!

        public int? ParentId { get; set; }
    }
}
