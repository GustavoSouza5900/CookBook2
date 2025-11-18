using CookBook.Models;
using System.Collections.Generic;

namespace CookBook.ViewModels
{
    public class ReceitaDetailsViewModel
    {
        public Receita Receita { get; set; } = new Receita();
        public string NovoComentarioTexto { get; set; } = string.Empty;
        public int ReceitaId { get; set; }
        public int TotalCurtidas { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
}