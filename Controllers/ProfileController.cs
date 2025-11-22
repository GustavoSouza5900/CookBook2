using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CookBook.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CookBook.ViewModels;
using Microsoft.AspNetCore.Identity;
using CookBook.Services;
using CookBook.Data;

namespace CookBook.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Ranking()
        {
            var ranking = await _userManager.Users
                .OfType<ApplicationUser>() // 
                .OrderByDescending(u => u.TotalLikesReceived)
                .ThenByDescending(u => u.Level)
                .Select(u => new 
                {
                    u.UserName,
                    u.Level,
                    u.TotalLikesReceived,
                    u.Id,
                    ProfilePictureUrl = u.ProfilePictureUrl
                })
                .Take(50) 
                .ToListAsync();

            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(ranking);
        }

        // GET: Profile/Badges
        [Authorize]
        public async Task<IActionResult> Badges()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // 1. Carregar o ApplicationUser (com EXP e Level)
            var user = await _userManager!.Users
                .OfType<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            
            // 2. Carregar os Badges Conquistados
            var achievedBadges = await _context.UserBadge
                .Include(ub => ub.Badge) // Inclui os detalhes do Badge
                .Where(ub => ub.UserId == userId)
                .OrderByDescending(ub => ub.DateAchieved)
                .ToListAsync();

            // 3. Preparar o ViewModel
            var expRequired = GamificationService.GetExpToNextLevel(user.Level);
            
            var viewModel = new UserBadgesViewModel
            {
                UserName = user.UserName ?? "Usuário",
                Level = user.Level,
                ExperiencePoints = user.ExperiencePoints,
                ExpToNextLevel = expRequired,
                AchievedBadges = achievedBadges,
                ProfilePictureUrl = user.ProfilePictureUrl // Assumindo que ApplicationUser tem esta propriedade
            };

            return View(viewModel);
        }
    }
}