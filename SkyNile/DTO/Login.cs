using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SkyNile.DTO
{
    public class Login
    {

        [MaxLength(150), EmailAddress, RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[-+_!@#$%^&*.,?]).+$", ErrorMessage = "Invalid input! The data must contain at least one uppercase letter, one lowercase letter, one special character, and can include letters, numbers, and symbols")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [Display(Name = "Password")]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
