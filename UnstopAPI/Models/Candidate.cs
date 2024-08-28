using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models
{
    public class Candidate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CandidateId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string LinkedInProfile { get; set; }

        public string Address { get; set; }

        public bool IsProfileCompleted { get; set; }
    }
}
