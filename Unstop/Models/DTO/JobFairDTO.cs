using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class JobFairDTO
    {
        public int JobFairId { get; set; }

        public int CompanyId { get; set; }

        public CompanyDTO Company { get; set; }

        public string Organizer {  get; set; }

        [Required(ErrorMessage = "Title is Required")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Start Date is Required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is Required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Start Time is Required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End Time is Required")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Job Location is Required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Job Level is Required")]
        public List<string> JobLevel { get; set; }
    }
}
