using Unstop.Models;

namespace Unstop.Services.IServices
{
    public interface IBaseService
    {
        APIResponse ResponseModel { get; set; }

        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
