using Microsoft.AspNetCore.Mvc.Rendering; 
using CookBook.Models;
using Microsoft.AspNetCore.Http; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // ESSENCIAL: Adicionar using para as anotações
using System.ComponentModel.DataAnnotations.Schema;

namespace CookBook.ViewModels
{
    public class ReceitaCreateViewModel
    {
        [Required(ErrorMessage = "O título da receita é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tempo de preparo é obrigatório.")]
        [Range(1, 1000, ErrorMessage = "O tempo de preparo deve ser um número válido em minutos (mínimo 1).")]
        public int TempoPreparoMinutos { get; set; }

        [Required(ErrorMessage = "As instruções (Modo de Preparo) são obrigatórias.")]
        public string Instrucoes { get; set; } = string.Empty;

        // Opcional: O arquivo de imagem não é obrigatório para o modelo de BD
        public IFormFile? ImagemArquivo { get; set; }

        // 🎯 CORREÇÃO: ADICIONAR [REQUIRED] para obrigar o preenchimento dos ingredientes
        [Required(ErrorMessage = "A lista de ingredientes é obrigatória.")]
        public string IngredientesInputData { get; set; } = string.Empty;
    }

    // Mantido como estava, pois é um modelo auxiliar
    public class ReceitaIngredienteInputModel
    {
        public int IngredienteId { get; set; }
        public string? Quantidade { get; set; }
        public bool Selecionado { get; set; }  
        public string? NomeIngrediente { get; set; } 
    }
}