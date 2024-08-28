using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class TemplateService : BaseService, ITemplateService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly string unstopUrl;

        public TemplateService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> CreateAsync<T>(string token, TemplateDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Token = token,
                Data = dto,
                Url = unstopUrl + "/api/Template"
            });
        }

        public Task<T> GetAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Template/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Token = token,
                Url = unstopUrl + "/api/Template"
            });
        }
    }
}
