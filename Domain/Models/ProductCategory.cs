
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }


        public int CategoryId { get; set; }


    }
}
