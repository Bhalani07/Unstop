using Unstop.Models.DTO;

namespace Unstop.Models.VM
{
    public class ApplicantsVM
    {
        public JobDTO Job { get; set; }

        public IEnumerable<ApplicationDTO> Applicants { get; set; }

        public Pagination Pagination { get; set; }
    }
}
