using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped] 
        public IFormFile? ImagemArquivo { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public ICollection<Comentario>? Comentarios { get; set; }
        
        public ICollection<ReceitaIngrediente>? ReceitaIngredientes { get; set; }
        public DateTime DataCriacao { get; set; }

        public ICollection<ReceitaCurtida>? ReceitaCurtidas { get; set; }
        public ICollection<ReceitaSalva>? ReceitaSalvas { get; set; }
        
        public Receita() 
        {
            DataCriacao = DateTime.Now; // Define a data/hora atual por padrão
        }
    }
}