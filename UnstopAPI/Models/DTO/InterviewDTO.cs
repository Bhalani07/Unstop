using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models.DTO
{
    public class InterviewDTO
    {
        public int InterviewId { get; set; }

        public int ApplicationId { get; set; }

        public ApplicationDTO Application { get; set; }

        [Required]
        public DateTime InterviewDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        [Required]
        public string Location { get; set; }

        public bool Complete { get; set; } = false;

        public string InterviewTitle { get; set; }

    }
}
