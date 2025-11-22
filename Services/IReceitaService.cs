// Em Services/IReceitaService.cs

using System.Threading.Tasks; // Se você usar métodos assíncronos

namespace CookBook.Services
{
    public interface IReceitaService
    {
        // Define o método de contagem. O valor de retorno é um 'int'.
        Task<int> ContarTodasReceitasAsync(); 
        Task<int> ContarTodosComentariosAsync();
        Task<int> ContarTodasCurtidasAsync();
        
        // Adicione aqui outros métodos que o Controller precise, como BuscarReceitaPorId
    }
}