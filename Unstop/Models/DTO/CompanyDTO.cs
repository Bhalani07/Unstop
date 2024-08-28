using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class CompanyDTO
    {
        public int CompanyId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Company is Required")]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9\s]{1,15}$", ErrorMessage = "Invalid Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Industry is Required")]
        public string Industry { get; set; }

        [Required(ErrorMessage = "CompanySize is Required")]
        public string CompanySize { get; set; }

        [Url]
        //[RegularExpression(@"^https?:\/\/[\w.-]+(?:\.[\w.-]+)+[\w\-\._~:/?#[\]@!$&'()*+,;=]*$", ErrorMessage = "Invalid URL format")]
        public string WebsiteUrl { get; set; }

        [RegularExpression(@"^.{2,}$", ErrorMessage = "Address must be at least 1 character long")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid City")]
        public string City { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^([0]|\+91)?[789]\d{9}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        public int FoundedYear { get; set; }

        public byte[] Logo { get; set; }

        public string LogoFileName { get; set; }

        public string LogoContentType { get; set; }

        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile LogoFile {  get; set; }

        public UserDTO User { get; set; }

        public string Email { get; set; }
    }
}
