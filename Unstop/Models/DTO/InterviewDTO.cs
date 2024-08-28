using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class InterviewDTO
    {
        public int InterviewId { get; set; }

        public int ApplicationId { get; set; }

        public ApplicationDTO Application { get; set; }

        [Required(ErrorMessage = "Interview Date is Required")]
        public DateTime InterviewDate { get; set; }

        [Required(ErrorMessage = "Starting Time is Required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Ending Time is Required")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Location is Required")]
        public string Location { get; set; }

        public bool Complete { get; set; } = false;

        public int JobId { get; set; }

        public string InterviewTitle { get; set; }

    }
}
