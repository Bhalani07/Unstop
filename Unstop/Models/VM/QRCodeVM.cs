using Unstop.Models.DTO;

namespace Unstop.Models.VM
{
    public class QRCodeVM
    {
        public CandidateDTO Candidate { get; set; }

        public List<CandidateDTO> Candidates { get; set; }

        public JobDTO Job { get; set; }

        public List<JobDTO> Jobs { get; set; }

        public List<TemplateDTO> Templates {  get; set; }

        public class FilteredData
        {
            public object SelectedValue { get; set; }
        }

    }
}
