using Microsoft.AspNetCore.JsonPatch.Operations;
using Unstop.Models.DTO;

namespace Unstop.Services.IServices
{
    public interface IInterviewService
    {
        Task<T> GetAllAsync<T>(string token, int userId, string userRole);

        Task<T> GetAsync<T>(string token, int id);

        Task<T> CreateAsync<T>(string token, InterviewDTO interviewDTO);

        Task<T> UpdateAsync<T>(string token, InterviewDTO interviewDTO);
    }
}
