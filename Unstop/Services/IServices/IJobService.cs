using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IJobService
    {
        #region Jobs

        Task<T> GetAllAsync<T>(string token, string userRole, int userId, string search, string jobType, string jobTime, int workingDays, string sortBy, string sortOrder, string jobStatus, int pageNumber, int pageSize, UserPreferenceDTO userPreferenceDTO = null);

        Task<T> GetAsync<T>(string token, int id);

        Task<T> CrateAsync<T>(string token, JobDTO dto);

        Task<T> UpdateAsync<T>(string token, JobDTO dto);

        Task<T> DeleteAsync<T>(string token, int id);

        #endregion


        #region Favorite Jobs

        Task<T> GetAllFavoriteAsync<T>(string token, int userId, string search, int pageNumber, int pageSize);

        Task<T> CrateFavoriteAsync<T>(string token, FavoriteDTO dto);

        Task<T> CheckFavoriteAsync<T>(string token, int jobId, int userId);

        Task<T> RemoveFavoriteAsync<T>(string token, int jobId, int userId);

        #endregion
    }
}
