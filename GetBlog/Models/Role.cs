using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } // "Admin", "Editor", "Author", "Subscriber"

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Role()
        {
            this.Users = new HashSet<ApplicationUser>();
        }
    }
}