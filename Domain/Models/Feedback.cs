using System;

namespace Domain.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime Cdate { get; set; } = DateTime.UtcNow;
    }
}
