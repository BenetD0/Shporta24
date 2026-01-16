using System;
using System.ComponentModel.DataAnnotations;

namespace Shporta24.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;

        public string? PerformedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
