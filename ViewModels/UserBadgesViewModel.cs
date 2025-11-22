using CookBook.Models;

namespace CookBook.ViewModels
{
    public class UserBadgesViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        
        public int ExpToNextLevel { get; set; }
        
        public IEnumerable<UserBadge> AchievedBadges { get; set; } = new List<UserBadge>();
        
        public IEnumerable<Badge> AllAvailableBadges { get; set; } = new List<Badge>();
        public string? ProfilePictureUrl { get; set; }
    }
}