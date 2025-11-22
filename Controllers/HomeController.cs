using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CookBook.Models;
using CookBook.ViewModels;
using CookBook.Services;
using System.Threading.Tasks; // Importante para IReceitaService e IUserService

namespace CookBook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    // PASSO 1: DECLARAÇÃO DOS CAMPOS PRIVADOS
    private readonly IReceitaService _receitaService; 
    private readonly IUserService _userService; 

    // PASSO 2: CONSTRUTOR ATUALIZADO (Recebendo Logger e Serviços)
    public HomeController(ILogger<HomeController> logger, IReceitaService receitaService, IUserService userService)
    {
        _logger = logger;
        _receitaService = receitaService;
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        // Crie o pacote (ViewModel) e preencha com dados reais dos seus serviços
        var viewModel = new HomeIndexViewModel
        {
            TotalReceitas = await _receitaService.ContarTodasReceitasAsync(), // Seu método de contagem real
            TotalChefs = await _userService.ContarTodosUsuariosAsync(),       // Seu método de contagem real
            TotalComentarios = await _receitaService.ContarTodosComentariosAsync(),
            TotalCurtidas = await  _receitaService.ContarTodasCurtidasAsync()
            //TotalReceitas = 10,  // Valor falso
            //TotalChefs = 5,      // Valor falso
            //TotalComentarios = 50,
            //TotalCurtidas = 100
        };
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
