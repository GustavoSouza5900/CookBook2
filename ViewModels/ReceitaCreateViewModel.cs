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

        public string IngredientesInputData { get; set; } = string.Empty;
    }

    public class ReceitaIngredienteInputModel
    {
        public int IngredienteId { get; set; }
        public string? Quantidade { get; set; }
        public bool Selecionado { get; set; }  
        public string? NomeIngrediente { get; set; } 
    }
}