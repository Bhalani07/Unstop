using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnstopAPI.Models
{
    public class OTP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   
        public int OTPId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }  

        public string Code { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}
