using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Configuration;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class InterviewService : BaseService, IInterviewService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly string unstopUrl;

        public InterviewService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> GetAllAsync<T>(string token, int userId, string userRole)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Interview?userRole=" + userRole + "&userId=" + userId,
                Token = token
            });
        }

        public Task<T> CreateAsync<T>(string token, InterviewDTO interviewDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = interviewDTO,
                Url = unstopUrl + "/api/Interview",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Interview/" + id,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(string token, InterviewDTO interviewDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = interviewDTO,
                Url = unstopUrl + "/api/Interview/" + interviewDTO.ApplicationId,
                Token = token
            });
        }
    }
}
