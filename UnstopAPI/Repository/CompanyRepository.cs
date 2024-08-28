using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
