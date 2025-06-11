using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class AuthorProfile
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string Bio { get; set; }

        public string ProfilePictureUrl { get; set; }

        [StringLength(100)]
        public string TwitterHandle { get; set; } //TODO social media

        [StringLength(150)]
        public string WebsiteUrl { get; set; }

    }
}