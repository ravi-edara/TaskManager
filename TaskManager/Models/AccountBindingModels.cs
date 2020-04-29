using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username field is required.")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password field is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}