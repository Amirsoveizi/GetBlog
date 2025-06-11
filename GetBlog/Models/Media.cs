using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class Media
    {
        [Key]
        public int MediaId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        public string Url { get; set; }

        [StringLength(50)]
        public string MimeType { get; set; }

        [StringLength(255)]
        public string AltText { get; set; }

        public DateTime UploadedDate { get; set; }

        public string UploaderId { get; set; }
        [ForeignKey("UploaderId")]
        public virtual ApplicationUser Uploader { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public Media()
        {
            this.Articles = new HashSet<Article>();
        }
    }
}