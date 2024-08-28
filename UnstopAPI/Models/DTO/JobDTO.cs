using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models.DTO
{
    public class JobDTO
    {
        public int JobId { get; set; }

        [Required(ErrorMessage = "Job Title is Required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Company is Required")]
        public string Company { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public string Responsibilities { get; set; }

        public string JobType { get; set; }

        public string JobTiming { get; set; }

        public string Location { get; set; }

        [Required(ErrorMessage = "Occupancy is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Occupancy must be greater than 0")]
        public int Occupancy { get; set; }

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public int MinExperience { get; set; }

        public int MaxExperience { get; set; }

        public int WorkingDays { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Last Date is Required")]
        public DateTime? LastDate { get; set; }

        public int UserId { get; set; }

    }
}
