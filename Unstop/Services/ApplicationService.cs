using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Configuration.UserSecrets;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class ApplicationService : BaseService, IApplicationService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly string unstopUrl;


        public ApplicationService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> GetAllAsync<T>(string token, string userRole, int userId, int jobId, string searchApplicants, string searchApplication,  string sortOrder, List<string> status = null, int pageNumber = 1, int pageSize = 5)
        {
            string url = unstopUrl + "/api/Application?userRole=" + userRole + "&userId=" + userId + "&jobId=" + jobId + "&searchApplicants=" + searchApplicants + "&searchApplication=" + searchApplication;

            if (status != null && status.Count > 0)
            {
                foreach (var item in status)
                {
                    url += "&status=" + item;
                }
            }

            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = url + "&sortOrder=" + sortOrder + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize,
                Token = token
            });
        }

        public Task<T> GetAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Application/" + id,
                Token = token
            });
        }

        public Task<T> CheckAsync<T>(string token, int jobId, int userId)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Application/" + jobId + "/" + userId,
                Token = token
            });
        }

        public Task<T> CrateAsync<T>(string token, ApplicationDTO applicationDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = applicationDTO,
                Url = unstopUrl + "/api/Application",
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(string token, ApplicationDTO applicationDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = applicationDTO,
                Url = unstopUrl + "/api/Application/" + applicationDTO.ApplicationId,
                Token = token
            });
        }

    }
}
