using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class ElementRepository : GenericRepository<Element>, IElementRepository
    {
        public ElementRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
