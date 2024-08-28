using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnstopAPI.Models
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobId { get; set; }

        public string Title { get; set; }

        public string Company {  get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public string Responsibilities { get; set; }

        public string JobType { get; set; }

        public string JobTiming { get; set; }

        public string Location { get; set; }

        public int Occupancy { get; set; }

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public int MinExperience { get; set; }

        public int MaxExperience { get; set; }

        public int WorkingDays { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }  


    }
}
