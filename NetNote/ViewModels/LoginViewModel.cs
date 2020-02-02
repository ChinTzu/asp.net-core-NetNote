using System.ComponentModel.DataAnnotations;

namespace NetNote.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} can't be empty")]
        [Display(Name = "user name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} can't be empty")]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }

        [Display(Name = "remember login status")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "user name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "{0} must greater than {2}", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "confirm password")]
        [Compare("Password", ErrorMessage = "inconsistent password ")]
        public string ConfirmPassword { get; set; }
    }

}