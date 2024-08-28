using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UnstopAPI.Models
{
    public class Element
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ElementId { get; set; }

        [ForeignKey("Template")]
        public int TemplateId { get; set; }

        public Template Template { get; set; }

        public string Name { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}
