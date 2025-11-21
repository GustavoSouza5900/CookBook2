using CookBook.Models;
using Microsoft.AspNetCore.Identity;
using CookBook.Data;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Services;

public class GamificationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public GamificationService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public static int GetExpToNextLevel(int currentLevel)
    {
        // Exemplo de curva de dificuldade: 1000 + 500 * (Level - 1)^1.5
        if (currentLevel < 1) return 1000;
        return 1000 + (int)(500 * Math.Pow(currentLevel - 1, 1.5));
    }

    public async Task AddExpAsync(ApplicationUser user, int expGain)
    {
        user.ExperiencePoints += expGain;
        int requiredExp = GetExpToNextLevel(user.Level);

        // Verifica se subiu de nível
        while (user.ExperiencePoints >= requiredExp)
        {
            user.ExperiencePoints -= requiredExp;
            user.Level++;
            requiredExp = GetExpToNextLevel(user.Level);
            // Opcional: Registrar ou notificar o Level Up
            // Console.WriteLine($"Parabéns! {user.UserName} alcançou o Nível {user.Level}!");
        }

        await _userManager.UpdateAsync(user);
    }

    public async Task CheckAndAwardBadge(ApplicationUser user, string eventType)
    {
        if (eventType == "NEW_RECIPE_PUBLISHED")
        {
            var recipeCount = await _context.Receita.CountAsync(r => r.UserId == user.Id);

            // Chef Novato
            if (recipeCount >= 1 && !await _context.UserBadge.AnyAsync(ub => ub.UserId == user.Id && ub.Badge!.Name == "Chef Novato"))
            {
                var badge = await _context.Badge.FirstOrDefaultAsync(b => b.TriggerEvent == "RECIPE_COUNT_1");
                if (badge != null)
                {
                    _context.UserBadge.Add(new UserBadge {UserId = user.Id, BadgeId = badge.Id});
                    await _context.SaveChangesAsync();
                }
            }
            
            // Mestre Culinário
            if (recipeCount >= 10 && !await _context.UserBadge.AnyAsync(ub => ub.UserId == user.Id && ub.Badge!.Name == "Mestre Culinário"))
            {
                var badge = await _context.Badge.FirstOrDefaultAsync(b => b.TriggerEvent == "RECIPE_COUNT_10");
                if (badge != null)
                {
                    _context.UserBadge.Add(new UserBadge {UserId = user.Id, BadgeId = badge.Id});
                    await _context.SaveChangesAsync();
                }
            }
        }

        else if (eventType == "LIKE_COUNT_UPDATED")
        {
            // Primeira de Muitas
            if (user.TotalLikesReceived >= 1 && !await _context.UserBadge.AnyAsync(ub => ub.UserId == user.Id && ub.Badge!.Name == "Primeira de Muitas"))
            {
                var badge = await _context.Badge.FirstOrDefaultAsync(b => b.TriggerEvent == "TOTAL_LIKES_1");
                if (badge != null)
                {
                    _context.UserBadge.Add(new UserBadge {UserId = user.Id, BadgeId = badge.Id});
                    await _context.SaveChangesAsync();
                }
            }

            // Popular
            if (user.TotalLikesReceived >= 50 && !await _context.UserBadge.AnyAsync(ub => ub.UserId == user.Id && ub.Badge!.Name == "Popular"))
            {
                var badge = await _context.Badge.FirstOrDefaultAsync(b => b.TriggerEvent == "TOTAL_LIKES_50");
                if (badge != null)
                {
                    _context.UserBadge.Add(new UserBadge {UserId = user.Id, BadgeId = badge.Id});
                    await _context.SaveChangesAsync();
                }
            }
        }
        
        else if (eventType == "NEW_COMMENT_PUBLISHED")
        {
            var commentCount = await _context.Comentario.CountAsync(c => c.UserId == user.Id);

            // Opnião Própria
            if (commentCount >= 1 && !await _context.UserBadge.AnyAsync(ub => ub.UserId == user.Id && ub.Badge!.Name == "Opnião Própria"))
            {
                var badge = await _context.Badge.FirstOrDefaultAsync(b => b.TriggerEvent == "COMMENT_COUNT_1");
                if (badge != null)
                {
                    _context.UserBadge.Add(new UserBadge {UserId = user.Id, BadgeId = badge.Id});
                    await _context.SaveChangesAsync();
                }
            }

            // Comentarista Ativo
            if (commentCount >= 5 && !await _context.UserBadge.AnyAsync(ub => ub.UserId == user.Id && ub.Badge!.Name == "Comentarista Ativo"))
            {
                var badge = await _context.Badge.FirstOrDefaultAsync(b => b.TriggerEvent == "COMMENT_COUNT_5");
                if (badge != null)
                {
                    _context.UserBadge.Add(new UserBadge {UserId = user.Id, BadgeId = badge.Id});
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}