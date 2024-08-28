namespace UnstopAPI.Models.DTO
{
    public class JobFairDTO
    {
        public int JobFairId { get; set; }

        public int CompanyId { get; set; }

        public CompanyDTO Company { get; set; }

        public string Organizer { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Location { get; set; }

        public List<string> JobLevel { get; set; }
    }
}
