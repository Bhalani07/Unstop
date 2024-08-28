using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IProfileService
    {
        Task<T> GetAllCandidateAsync<T>(string token);

        Task<T> GetCandidateAsync<T>(string token, int id);

        Task<T> UpdateCandidateAsync<T>(string token, CandidateDTO candidateDTO);

        Task<T> GetCompanyAsync<T>(int id);

        Task<T> UpdateCompanyAsync<T>(string token, CompanyDTO companyDTO);

        Task<T> GetUserAsync<T>(string token, int id);

        Task<T> CreateUserAsync<T>(string token, CompanyDTO companyDTO);  
    }
}
