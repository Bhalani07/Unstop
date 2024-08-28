using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly string unstopUrl;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestModel model)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = model,
                Url = unstopUrl + "/api/UserAuth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegistrationModel model)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = model,
                Url = unstopUrl + "/api/UserAuth/register"
            });
        }

        public Task<T> ForgotAsync<T>(string email)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = email,
                Url = unstopUrl + "/api/UserAuth/forgot"
            });
        }

        public Task<T> ValidateOTPAsync<T>(ForgotPasswordModel model)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = model,
                Url = unstopUrl + "/api/UserAuth/verify-otp"
            });
        }

        public Task<T> ResetPasswordAsync<T>(ForgotPasswordModel model)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = model,
                Url = unstopUrl + "/api/UserAuth/reset-password"
            });
        }


    }
}
