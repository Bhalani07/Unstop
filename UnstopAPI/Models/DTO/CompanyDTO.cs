namespace UnstopAPI.Models.DTO
{
    public class CompanyDTO
    {
        public int CompanyId { get; set; }

        public int UserId { get; set; }

        public string CompanyName { get; set; }

        public string Industry { get; set; }

        public string CompanySize { get; set; }

        public string WebsiteUrl { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string PhoneNumber { get; set; }

        public int FoundedYear { get; set; }

        public byte[] Logo { get; set; }

        public string LogoFileName { get; set; }

        public string LogoContentType { get; set; }

        public UserDTO User { get; set; }
    }
}
