using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class UserPreferencesRepository : GenericRepository<UserPreference>, IUserPreferenceRepository
    {
        public UserPreferencesRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
