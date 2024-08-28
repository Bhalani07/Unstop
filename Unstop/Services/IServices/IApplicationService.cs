using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IApplicationService
    {
        Task<T> GetAllAsync<T>(string token, string userRole, int userId, int jobId, string searchApplicants, string searchApplication,  string sortOrder, List<string> status = null, int pageNumber = 1, int pageSize = 5);

        Task<T> GetAsync<T>(string token, int id);

        Task<T> CheckAsync<T>(string token, int jobId, int userId);

        Task<T> CrateAsync<T>(string token, ApplicationDTO applicationDTO);

        Task<T> UpdateAsync<T>(string token, ApplicationDTO applicationDTO);
    }
}
