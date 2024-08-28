using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class JobDTO
    {
        public int JobId { get; set; }

        [Required(ErrorMessage = "Job Title is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9\s]{1,}$", ErrorMessage = "Invalid Job Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Company is Required")]
        public string Company { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public string Responsibilities { get; set; }

        [Required(ErrorMessage = "Job Type is Required")]
        public string JobType { get; set; }

        [Required(ErrorMessage = "Job Time is Required")]
        public string JobTiming { get; set; }

        public string Location { get; set; }

        [Required(ErrorMessage = "Occupancy is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Occupancy must be greater than 0")]
        public int Occupancy { get; set; }

        [Range(0, 10000000, ErrorMessage = "MinSalary must be between 0 and 10000000")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Maximum 2 decimal digits are allowed")]
        public decimal? MinSalary { get; set; }

        [Range(0, 10000000, ErrorMessage = "MaxSalary must be between 0 and 10000000")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Maximum 2 decimal digits are allowed")]
        public decimal? MaxSalary { get; set; }

        [Range(0, 30, ErrorMessage = "MinExperience must be between 0 and 30")]
        public int MinExperience { get; set; }

        [Range(0, 30, ErrorMessage = "MaxExperience must be between 0 and 30")]
        public int MaxExperience { get; set; }

        public int WorkingDays { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Last Date is Required")]
        [DataType(DataType.Date)]
        public DateTime? LastDate { get; set; }

        public int UserId { get; set; }

        public bool IsFavorite { get; set; }

    }
}
