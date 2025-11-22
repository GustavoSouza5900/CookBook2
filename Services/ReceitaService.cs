// Em Services/ReceitaService.cs

using CookBook.Data; // Assumindo que seu DbContext está aqui
using Microsoft.EntityFrameworkCore; // Para o método CountAsync

namespace CookBook.Services
{
    // A classe implementa o contrato IReceitaService
    public class ReceitaService : IReceitaService
    {
        private readonly ApplicationDbContext _context; // Seu contexto de banco de dados

        // Construtor para injeção do DbContext
        public ReceitaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> ContarTodasReceitasAsync()
        {
            // Lógica de negócio: Conta todas as entradas na tabela de Receitas
            return await _context.Receitas.CountAsync();
            
            // Nota: Substitua "_context.Receitas" pela sua DbSet de Receitas
        }
        // NOVA LÓGICA DE COMENTÁRIOS
        public async Task<int> ContarTodosComentariosAsync()
        {
            // Assume que você tem um DbSet chamado 'Comentarios' no seu DbContext
            return await _context.Comentario.CountAsync(); 
        }

        // NOVA LÓGICA DE CURTIDAS
        public async Task<int> ContarTodasCurtidasAsync()
        {
            // Assume que você tem um DbSet chamado 'Curtidas' no seu DbContext
            return await _context.ReceitaCurtida.CountAsync(); 
        }
    }
}