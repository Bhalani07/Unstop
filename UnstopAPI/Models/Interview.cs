using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models
{
    public class Interview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterviewId { get; set; }

        [ForeignKey("Application")]
        public int ApplicationId { get; set; }

        public Application Application { get; set; }

        public DateTime InterviewDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set;}

        public string Location { get; set;}

        public bool Complete { get; set; } = false;

        public string InterviewTitle { get; set; }
    }
}
