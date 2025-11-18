using CookBook.Models;
using System.Collections.Generic;

namespace CookBook.ViewModels
{
    public class ReceitaIndexViewModel
    {
        public IEnumerable<Receita> Receitas { get; set; } = new List<Receita>();
        public string? SearchQuery { get; set; } // O texto que o usuário digitou
        public string SearchType { get; set; } = "Nome"; // "Nome" ou "Ingredientes"
    }
}