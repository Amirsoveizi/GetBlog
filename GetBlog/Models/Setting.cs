using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetBlog.Models
{
    public class Setting
    {
        [Key]
        [StringLength(100)]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }
    }
}