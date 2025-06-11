using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Slug { get; set; }

        public int? ParentCategoryId { get; set; }
        [ForeignKey("ParentCategoryId")]
        public virtual Category ParentCategory { get; set; }

        public virtual ICollection<Category> ChildCategories { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public Category()
        {
            this.ChildCategories = new HashSet<Category>();
            this.Articles = new HashSet<Article>();
        }
    }
}