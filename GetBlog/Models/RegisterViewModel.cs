// /Models/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace GetBlog.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفا نام کامل خود را وارد کنید")]
        [Display(Name = "نام کامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "لطفا نام کاربری خود را وارد کنید")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "لطفا ایمیل خود را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا رمز ورود خود را وارد کنید")]
        [StringLength(100, ErrorMessage = "رمز ورود باید حداقل {2} کاراکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز ورود")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز ورود")]
        [Compare("Password", ErrorMessage = "رمز ورود و تکرار آن یکسان نیستند.")]
        public string ConfirmPassword { get; set; }
    }
}