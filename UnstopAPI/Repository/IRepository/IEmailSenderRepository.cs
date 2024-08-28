using UnstopAPI.Models.DTO;

namespace UnstopAPI.Repository.IRepository
{
    public interface IEmailSenderRepository
    {
        Task<bool> SendEmailAsync(EmailRequestModel emailRequest);
    }
}
