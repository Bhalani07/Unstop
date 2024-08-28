using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models
{
    public class Template
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TemplateId { get; set; }

        public int CorrectionLevel { get; set; }

        public int Margin { get; set; }

        public string Color { get; set; }

        public string DataSource { get; set; }

        public string TextField { get; set; }

    }
}
