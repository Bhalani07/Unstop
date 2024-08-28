using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models
{
    public class JobFair
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobFairId { get; set; }

        public string Title { get; set; }

        public string Organizer { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Location { get; set; }

        public List<string> JobLevel { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        public Company Company { get; set; }
    }
}
