using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class TemplateRepository : GenericRepository<Template>, ITemplateRepository
    {
        public TemplateRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
