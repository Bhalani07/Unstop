using System.ComponentModel.DataAnnotations;

namespace Unstop.Models.DTO
{
    public class OfferDTO
    {
        public int ApplicationId { get; set; }

        public int JobId { get; set; }  

        public int CandidateId { get; set; }

        [Required(ErrorMessage = "JobTitle is Required")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "JobType is Required")]
        public string JobType { get; set; }

        [Required(ErrorMessage = "JobLocations is Required")]
        public string JobLocation { get; set; }

        [Required(ErrorMessage = "JoiningDate is Required")]
        public string JoiningDate { get; set; }

        [Range(100000, 10000000, ErrorMessage = "Should be > 100000 & < 10000000")]
        public int Salary {  get; set; }

        public string CompanyName { get; set; }

        public string SenderName { get; set; }

        public string CandidateName { get; set; }
    }
}
