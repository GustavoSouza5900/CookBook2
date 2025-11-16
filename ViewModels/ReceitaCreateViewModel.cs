using Microsoft.AspNetCore.Mvc.Rendering; // Necessário para SelectList e SelectListItem
using CookBook.Models;
using Microsoft.AspNetCore.Http; // Necessário para IFormFile
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookBook.ViewModels
{
    public class ReceitaCreateViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public int TempoPreparoMinutos { get; set; }
        public string Instrucoes { get; set; } = string.Empty;

        public IFormFile? ImagemArquivo { get; set; }

        public List<int> IngredientesSelecionadosIds { get; set; } = new List<int>();

        public List<SelectListItem> IngredientesDisponiveis { get; set; } = new List<SelectListItem>();
    }
}