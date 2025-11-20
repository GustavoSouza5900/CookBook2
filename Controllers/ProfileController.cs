using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CookBook.Data;
using CookBook.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CookBook.ViewModels;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using CookBook.Services;

namespace CookBook.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser>? _userManager;

        [AllowAnonymous]
        public async Task<IActionResult> Ranking()
        {
            var ranking = await _userManager!.Users
                .OfType<ApplicationUser>() // 
                .OrderByDescending(u => u.TotalLikesReceived)
                .ThenByDescending(u => u.Level)
                .Select(u => new 
                {
                    u.UserName,
                    u.Level,
                    u.TotalLikesReceived
                })
                .Take(50) 
                .ToListAsync();

            return View(ranking);
        }
    }
}