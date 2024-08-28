using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string secretKey;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }


        #region Login Logic

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequestModel)
        {
            User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginRequestModel.Email);

            if (user == null || !VerifyPasswordHash(loginRequestModel.Password, user.PasswordHash))
            {
                return new LoginResponseModel()
                {
                    Token = "",
                    User = null
                };
            }

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.UTF8.GetBytes(secretKey);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseModel loginResponseModel = new()
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };

            return loginResponseModel;
        }

        #endregion


        #region Registraion Logic

        public async Task<User> RegisterAsync(RegistrationModel registrationModel)
        {
            User user = new()
            {
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName,
                Email = registrationModel.Email,
                PasswordHash = CreatePasswordHash(registrationModel.Password),
                Role = registrationModel.Role,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if(registrationModel.Role == "Candidate")
            {
                Candidate candidate = new()
                {
                    UserId = user.Id,
                    FullName = registrationModel.FirstName + " " + registrationModel.LastName,
                };

                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();
            }            

            return user;
        }

        #endregion


        #region Check User Existence

        public async Task<bool> UserExistAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        #endregion


        #region HashPassword Logic

        public string CreatePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        #endregion

    }
}
