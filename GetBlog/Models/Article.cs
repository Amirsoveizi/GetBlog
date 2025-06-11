using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetBlog.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(500)]
        public string Excerpt { get; set; }

        public string FeaturedImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsPublished { get; set; } = false;

        [StringLength(250)]
        public string Slug { get; set; }

        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Media> MediaItems { get; set; }

        public Article()
        {
            this.Categories = new HashSet<Category>();
            this.Tags = new HashSet<Tag>();
            this.Comments = new HashSet<Comment>();
            this.MediaItems = new HashSet<Media>();
        }
    }
}