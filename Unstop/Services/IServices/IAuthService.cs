using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestModel model);

        Task<T> RegisterAsync<T>(RegistrationModel model);

        Task<T> ForgotAsync<T>(string email);

        Task<T> ValidateOTPAsync<T>(ForgotPasswordModel model);

        Task<T> ResetPasswordAsync<T>(ForgotPasswordModel model);
    }
}
