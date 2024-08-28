using System.Net.Mail;

namespace Unstop.Models.DTO
{
    public class EmailRequestModel
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        //public Attachment Attachment { get; set; }

        public byte[] Attachment { get; set; }

        public string AttachmentName { get; set; }

    }
}
