using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IJobFairService
    {
        Task<T> GetAllAsync<T>(string token);

        Task<T> GetAsync<T>(int id);

        Task<T> CrateAsync<T>(string token, JobFairDTO dto);
    }
}
