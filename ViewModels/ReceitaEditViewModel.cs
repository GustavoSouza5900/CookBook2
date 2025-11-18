using Microsoft.AspNetCore.Mvc.Rendering;

namespace CookBook.ViewModels
{
    public class ReceitaEditViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int TempoPreparoMinutos { get; set; }
        public string Instrucoes { get; set; } = string.Empty;

        public string? ImagemUrlExistente { get; set; } 
        public IFormFile? NovaImagemArquivo { get; set; } 
        public string IngredientesInputData { get; set; } = string.Empty;
        public string IngredientesExistentesJson { get; set; } = string.Empty;
    }
}