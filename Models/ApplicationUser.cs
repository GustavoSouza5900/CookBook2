using Microsoft.AspNetCore.Identity;

namespace CookBook.Models
{
    public class ApplicationUser : IdentityUser
    {
        // 🚨 Certifique-se de que seu DbContext herde de IdentityDbContext<ApplicationUser>
        public int ExperiencePoints { get; set; } = 0;
        public int Level { get; set; } = 1;
        
        public int TotalLikesReceived { get; set; } = 0;
        public int TotalSavedRecipes { get; set; } = 0;
    }
}