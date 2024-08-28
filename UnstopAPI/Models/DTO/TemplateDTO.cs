namespace UnstopAPI.Models.DTO
{
    public class TemplateDTO
    {
        public int TemplateId { get; set; }

        public int CorrectionLevel { get; set; }

        public int Margin { get; set; }

        public string Color { get; set; }

        public string DataSource { get; set; }

        public string TextField { get; set; }

        public List<ElementDTO> Elements { get; set; }
    }
}
