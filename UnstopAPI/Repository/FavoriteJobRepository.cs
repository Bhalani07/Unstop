using UnstopAPI.Data;
using UnstopAPI.Models;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class FavoriteJobRepository : GenericRepository<FavoriteJob>, IFavoriteJobRepository
    {
        public FavoriteJobRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
