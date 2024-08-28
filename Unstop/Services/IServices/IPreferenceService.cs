using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IPreferenceService
    {
        Task<T> GetAllAsync<T>(string token, UserPreferenceDTO userPreferenceDTO);

        Task<T> GetAsync<T>(string token, int id);

        Task<T> CreateAsync<T>(string token, UserPreferenceDTO userPreferenceDTO);

        Task<T> UpdateAsync<T>(string token, UserPreferenceDTO userPreferenceDTO); 

    }
}
