using Unstop.Models.DTO;

namespace Unstop.Models.VM
{
    public class ApplicationsVM
    {
        public IEnumerable<ApplicationDTO> Applications { get; set; }

        public Pagination Pagination { get; set; }
    }
}
