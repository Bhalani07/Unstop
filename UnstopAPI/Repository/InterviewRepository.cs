using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class InterviewRepository : GenericRepository<Interview>, IInterviewRepository
    {
        public InterviewRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
