// /Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace GetBlog.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "لطفا نام کاربری خود را وارد کنید")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "لطفا رمز ورود خود را وارد کنید")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز ورود")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}