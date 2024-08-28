using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class PreferenceService : BaseService, IPreferenceService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly string unstopUrl;

        public PreferenceService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }

        public Task<T> GetAllAsync<T>(string token, UserPreferenceDTO userPreferenceDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Preference/",
                Token = token,
                Data = userPreferenceDTO
            });
        }

        public Task<T> GetAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Preference/" + id,
                Token = token
            });
        }

        public Task<T> CreateAsync<T>(string token, UserPreferenceDTO userPreferenceDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = userPreferenceDTO,
                Url = unstopUrl + "/api/Preference",
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(string token, UserPreferenceDTO userPreferenceDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = userPreferenceDTO,
                Url = unstopUrl + "/api/Preference/" + userPreferenceDTO.PreferenceId,
                Token = token
            });
        }


    }
}
