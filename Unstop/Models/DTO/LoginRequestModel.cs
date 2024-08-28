using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Paasword is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }
    }
}
