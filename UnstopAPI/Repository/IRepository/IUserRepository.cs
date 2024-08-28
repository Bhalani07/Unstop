using UnstopAPI.Models;
using UnstopAPI.Models.DTO;

namespace UnstopAPI.Repository.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequestModel);

        Task<User> RegisterAsync(RegistrationModel registrationModel);

        Task<bool> UserExistAsync(string email);

        string CreatePasswordHash(string password);

        bool VerifyPasswordHash(string password, string storedHash);


    }
}
