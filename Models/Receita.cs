using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CookBook.Models
{
    public class Receita
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;
        public int TempoPreparoMinutos { get; set; }
        public string Instrucoes { get; set; } = string.Empty;
        
        public string? ImagemUrl { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        public IdentityUser User { get; set; } = null!;

        public ICollection<Comentario>? Comentarios { get; set; }
        
        public ICollection<ReceitaIngrediente>? ReceitaIngredientes { get; set; }
    }
}