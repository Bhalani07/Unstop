using System.ComponentModel.DataAnnotations.Schema;

namespace UnstopAPI.Models.DTO
{
    public class ElementDTO
    {
        public int ElementId { get; set; }

        public int TemplateId { get; set; }

        public string Name { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}
