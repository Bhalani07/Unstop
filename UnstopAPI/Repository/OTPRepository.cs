using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class OTPRepository : GenericRepository<OTP>, IOTPRepository
    {
        public OTPRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
