using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        private readonly IHttpClientFactory _httpClient;

        private readonly string unstopUrl;

        public ProfileService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> GetAllCandidateAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Profile/",
                Token = token
            });
        }

        public Task<T> GetCandidateAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Profile/candidate/" + id,
                Token = token
            });
        }

        public Task<T> UpdateCandidateAsync<T>(string token, CandidateDTO candidateDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = candidateDTO,
                Url = unstopUrl + "/api/Profile/candidate/" + candidateDTO.CandidateId,
                Token = token
            });
        }

        public Task<T> GetCompanyAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Profile/company/" + id,
            });
        }

        public Task<T> UpdateCompanyAsync<T>(string token, CompanyDTO companyDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = companyDTO,
                Url = unstopUrl + "/api/Profile/company/" + companyDTO.CompanyId,
                Token = token
            });
        }


        public Task<T> GetUserAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Profile/user/" + id,
                Token = token
            });
        }

        public Task<T> CreateUserAsync<T>(string token, CompanyDTO companyDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = companyDTO,
                Url = unstopUrl + "/api/Profile/user",
                Token = token
            });
        }

    }
}
