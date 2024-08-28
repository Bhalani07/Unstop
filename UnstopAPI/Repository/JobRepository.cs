using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        //private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context) : base(context)
        {
            //_context = context;
        }
    }
}
