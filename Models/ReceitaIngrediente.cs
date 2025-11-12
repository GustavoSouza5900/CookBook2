namespace CookBook.Models
{
    public class ReceitaIngrediente
    {
        public int ReceitaId { get; set; }
        public int IngredienteId { get; set; }

        public Receita Receita { get; set; } = null!;
        public Ingrediente Ingrediente { get; set; } = null!;
        
        public string Quantidade { get; set; } = string.Empty;
    }
}