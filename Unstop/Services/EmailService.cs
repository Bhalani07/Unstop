using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class EmailService : BaseService, IEmailService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly string unstopUrl;

        public EmailService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> SendEmailAsync<T>(EmailRequestModel emailRequest)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = unstopUrl + "/api/Email",
                Data = emailRequest
            });
        }
    }
}
