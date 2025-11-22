// Em Services/UserService.cs

using CookBook.Data; // Seu contexto de banco de dados
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Para acessar a tabela de usuários

namespace CookBook.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context; // Seu contexto de banco de dados
        
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> ContarTodosUsuariosAsync()
        {
            // Nota: Se estiver usando Identity, a tabela de usuários é Users
            return await _context.Users.CountAsync();
            
            // Se você tiver uma classe de modelo Chef, use:
            // return await _context.Chefs.CountAsync();
        }
    }
}