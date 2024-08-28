using System.Globalization;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Services
{
    public class JobService : BaseService, IJobService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string unstopUrl;

        public JobService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            unstopUrl = configuration.GetValue<string>("ServiceUrls:UnstopAPI");
        }


        #region Jobs 

        public Task<T> GetAllAsync<T>(string token, string userRole, int userId, string search, string jobType, string jobTime, int workingDays, string sortBy, string sortOrder, string jobStatus, int pageNumber = 1, int pageSize = 5, UserPreferenceDTO userPreferenceDTO = null)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Token = token,
                Data = userPreferenceDTO,
                Url = unstopUrl + "/api/Job?userRole=" + userRole + "&userId=" + userId + "&searchJob=" + search + "&jobType=" + jobType + "&jobTime=" + jobTime + "&workingDays=" + workingDays + "&sortBy=" + sortBy + "&sortOrder=" + sortOrder + "&jobStatus=" + jobStatus + "&pageSize=" + pageSize + "&pageNumber=" + pageNumber
            });
        }

        public Task<T> GetAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/Job/" + id,
                Token = token
            });
        }

        public Task<T> CrateAsync<T>(string token, JobDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Token = token,
                Data = dto,
                Url = unstopUrl + "/api/Job"
            });
        }

        public Task<T> DeleteAsync<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = unstopUrl + "/api/Job/" + id,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(string token, JobDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = dto,
                Url = unstopUrl + "/api/Job/" + dto.JobId,
                Token = token
            });
        }

        #endregion


        #region Favorite Job

        public Task<T> GetAllFavoriteAsync<T>(string token, int userId, string search, int pageNumber, int pageSize)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Token = token,
                Url = unstopUrl + "/api/FavoriteJob?userId=" + userId + "&searchFavorite=" + search + "&pageSize=" + pageSize + "&pageNumber=" + pageNumber
            });
        }

        public Task<T> CrateFavoriteAsync<T>(string token, FavoriteDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = dto,
                Url = unstopUrl + "/api/FavoriteJob/",
                Token = token
            });
        }

        public Task<T> CheckFavoriteAsync<T>(string token, int jobId, int userId)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = unstopUrl + "/api/FavoriteJob/" + jobId + "/" + userId,
                Token = token
            });
        }

        public Task<T> RemoveFavoriteAsync<T>(string token, int jobId, int userId)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = unstopUrl + "/api/FavoriteJob/" + jobId + "/" + userId,
                Token = token
            });
        }


        #endregion
    }
}
