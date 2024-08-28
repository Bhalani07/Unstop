using Unstop.Models.DTO;

namespace Unstop.Models.VM
{
    public class JobsVM
    {
        public IEnumerable<JobDTO> Jobs { get; set; }

        public Pagination Pagination { get; set; }
    }
}
