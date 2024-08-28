using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string OtpCode { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Invalid Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Should be same as New Password")]
        public string ConfirmPassword { get; set; }
    }
}
