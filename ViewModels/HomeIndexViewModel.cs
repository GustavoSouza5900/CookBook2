// Em ViewModels/HomeIndexViewModel.cs
namespace CookBook.ViewModels
{
    public class HomeIndexViewModel
    {
        public int TotalReceitas { get; set; }
        public int TotalChefs { get; set; }
        public int TotalComentarios { get; set; }
        public int TotalCurtidas { get; set; }
        // Se precisar de uma propriedade de Ranking global
        // public List<ChefRanking> Ranking { get; set; } 
    }
}