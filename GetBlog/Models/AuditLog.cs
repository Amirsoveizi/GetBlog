using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class AuditLog
    {
        [Key]
        public long AuditId { get; set; } 

        public string UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        [StringLength(100)]
        public string EntityName { get; set; }

        public int? EntityId { get; set; } 

        public DateTime Timestamp { get; set; }

        public string Details { get; set; }
    }
}