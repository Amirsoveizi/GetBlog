using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsApproved { get; set; } = false;

        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; }

        public int? ParentCommentId { get; set; }
        [ForeignKey("ParentCommentId")]
        public virtual Comment ParentComment { get; set; }

        public virtual ICollection<Comment> Replies { get; set; }

        public Comment()
        {
            this.Replies = new HashSet<Comment>();
        }
    }
}