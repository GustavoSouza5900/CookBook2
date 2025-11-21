namespace CookBook.Models
{
    public class ReceitaSalva
    {
        public int ReceitaId { get; set; }
        public Receita? Receita { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}