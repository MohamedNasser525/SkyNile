using System.ComponentModel.DataAnnotations;

namespace SkyNile.DTO
{
    public class AddRoleModel
    {
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Only letters and numbers are allowed.")]
        [MaxLength(20, ErrorMessage = "UserId must be no more 20 characters long")]
        public string UserId { get; set; }


        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Only letters are allowed.")]
        [MaxLength(8, ErrorMessage = "Password must be no more 8 characters long")]
        public string Role { get; set; }
    }
}
