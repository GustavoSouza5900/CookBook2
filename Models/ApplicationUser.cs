using Microsoft.AspNetCore.Identity;

namespace CookBook.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int ExperiencePoints { get; set; } = 0;
        public int Level { get; set; } = 1;
        
        public int TotalLikesReceived { get; set; } = 0;
        public int TotalSavedRecipes { get; set; } = 0;

        public string? ProfilePictureUrl { get; set; }

    }
}