using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IEmailService
    {
        Task<T> SendEmailAsync<T>(EmailRequestModel emailRequest); 
    }
}
