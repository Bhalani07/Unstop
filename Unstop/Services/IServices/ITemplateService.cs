using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface ITemplateService
    {
        Task<T> GetAllAsync<T>(string token);

        Task<T> GetAsync<T>(string token, int id);

        Task<T> CreateAsync<T>(string token, TemplateDTO dto);
    }
}
