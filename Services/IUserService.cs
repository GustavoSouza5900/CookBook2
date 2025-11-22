// Em Services/IUserService.cs

using System.Threading.Tasks;

namespace CookBook.Services
{
    public interface IUserService
    {
        // Define o método de contagem para usuários/chefs
        Task<int> ContarTodosUsuariosAsync(); 
        
        // Adicione aqui outros métodos relacionados a usuários, se necessário
    }
}