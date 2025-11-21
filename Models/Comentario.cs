namespace CookBook.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Conteudo { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relação com a Receita (Chave Estrangeira)
        public int ReceitaId { get; set; }
        public Receita Receita { get; set; } = null!;

        // Relação com o Usuário (Quem comentou)
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!; 
    }
}