namespace UnstopAPI.Models.DTO
{
    public class ApplicationDTO
    {
        public int ApplicationId { get; set; }

        public int JobId { get; set; }

        public JobDTO Job { get; set; }

        public int CandidateId { get; set; }

        public CandidateDTO Candidate { get; set; }

        public byte[] Resume { get; set; }

        public string ResumeFileName { get; set; }

        public string ResumeContentType { get; set; }

        public string Status { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    }
}
