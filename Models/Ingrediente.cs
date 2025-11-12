namespace CookBook.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        
        public ICollection<ReceitaIngrediente>? ReceitaIngredientes { get; set; }
    }
}