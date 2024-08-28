namespace UnstopAPI.Models.DTO
{
    public class CandidateDTO
    {
        public int CandidateId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string LinkedInProfile { get; set; }

        public string Address { get; set; }

        public bool IsProfileCompleted { get; set; } = false;

        public UserDTO User { get; set; }

    }
}
