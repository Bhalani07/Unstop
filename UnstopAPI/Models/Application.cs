using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models
{
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }

        [ForeignKey("Job")]
        public int JobId { get; set; }

        public Job Job { get; set; }

        [ForeignKey("Candidate")]
        public int CandidateId { get; set; }

        public Candidate Candidate { get; set; }

        public byte[] Resume { get; set; }

        public string ResumeFileName { get; set; }

        public string ResumeContentType { get; set; }

        public string Status { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    }
}