using Syncfusion.DocIO.DLS;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class JobFairService : BaseService, IJobFairService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string unstopUrl;

        public JobFairService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClientFactory = httpClient;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> CrateAsync<T>(string token, JobFairDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Token = token,
                Data = dto,
                Url = unstopUrl + "/api/JobFair"
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Token = token,
                Url = unstopUrl + "/api/JobFair"
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/JobFair/" + id,
            });
        }
    }
}
