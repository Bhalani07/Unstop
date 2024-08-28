namespace UnstopAPI.Models.DTO
{
    public class FavoriteDTO
    {
        public int FavoriteId { get; set; }

        public int JobId { get; set; }

        public JobDTO Job { get; set; }

        public int UserId { get; set; }

        public UserDTO User { get; set; }
    }
}
