namespace CookBook.Models
{
    public class UserBadge
    {
        public int BadgeId { get; set; }
        public Badge? Badge { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        
        public DateTime DateAchieved { get; set; } = DateTime.Now;
    }
}