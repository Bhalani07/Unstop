using Unstop.Models.DTO;

namespace Unstop.Models.VM
{
    public class FavoritesVM
    {
        public IEnumerable<FavoriteDTO> Favorites { get; set; }

        public Pagination Pagination { get; set; }
    }
}
