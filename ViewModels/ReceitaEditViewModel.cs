using Microsoft.AspNetCore.Mvc.Rendering;

namespace CookBook.ViewModels
{
    public class ReceitaEditViewModel
    {
        // Campos obrigatórios para Edição
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int TempoPreparoMinutos { get; set; }
        public string Instrucoes { get; set; } = string.Empty;

        public string? ImagemUrlExistente { get; set; } // URL da imagem atual (para exibição na View)
        public IFormFile? NovaImagemArquivo { get; set; } // Novo arquivo enviado pelo usuário

        public List<int> IngredientesSelecionadosIds { get; set; } = new List<int>();
        public List<SelectListItem> IngredientesDisponiveis { get; set; } = new List<SelectListItem>();
    }
}