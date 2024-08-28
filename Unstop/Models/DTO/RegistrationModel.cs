using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Should be same as Password")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }
    }
}
