using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models
{
    public class UserPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PreferenceId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        public string[] JobType { get; set; }

        public string[] JobTime { get; set; }

        public string[] Industry { get; set; }

        public string[] CompanySize { get; set; }
    }
}
