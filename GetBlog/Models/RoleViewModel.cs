using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GetBlog.Models 
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        public RoleViewModel() { }
        public RoleViewModel(IdentityRole role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
        }
    }
}