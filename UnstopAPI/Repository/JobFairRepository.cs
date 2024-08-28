using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class JobFairRepository : GenericRepository<JobFair>, IJobFairRepository
    {
        public JobFairRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
