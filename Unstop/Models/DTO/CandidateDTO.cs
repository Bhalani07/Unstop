using System.ComponentModel.DataAnnotations;
using Unstop.Models.DTO;

namespace Unstop.Models.DTO
{
    public class CandidateDTO
    {
        public int CandidateId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "FullName is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "DateOfBirth is Required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^([0]|\+91)?[789]\d{9}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Url]
        public string LinkedInProfile { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        [RegularExpression(@"^.{2,}$", ErrorMessage = "Address must be at least 1 character long")]
        public string Address { get; set; }

        public bool IsProfileCompleted { get; set; } = false;

        public UserDTO User { get; set; }
    }
}
