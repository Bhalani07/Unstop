namespace Unstop.Models.DTO
{
    public class UserPreferenceDTO
    {
        public int PreferenceId { get; set; }

        public int UserId { get; set; }

        public UserDTO User { get; set; }

        public List<string> JobType { get; set; }

        public List<string> JobTime { get; set; }

        public List<string> Industry { get; set; }

        public List<string> CompanySize { get; set; }
    }
}
