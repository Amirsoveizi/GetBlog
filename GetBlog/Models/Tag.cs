using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Slug { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public Tag()
        {
            this.Articles = new HashSet<Article>();
        }
    }
}