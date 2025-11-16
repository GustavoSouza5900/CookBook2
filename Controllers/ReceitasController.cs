using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CookBook.Data;
using CookBook.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CookBook.ViewModels;



namespace CookBook.Controllers
{
    [Authorize]
    public class ReceitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ReceitasController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Receitas
        [AllowAnonymous] // Permite acesso sem login, mesmo com o [Authorize] no topo da classe
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Receita.Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Receitas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receita == null)
            {
                return NotFound();
            }

            return View(receita);
        }

        // GET: Receitas/Create
        public IActionResult Create()
        {
            var ingredientes = _context.Ingrediente.ToList();
            var viewModel = new ReceitaCreateViewModel
            {
                IngredientesDisponiveis = ingredientes.Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(), // O ID do ingrediente
                    Text = i.Nome,           // O nome exibido na lista
                    Selected = false         // Nenhum selecionado por padrão
                }).ToList()
            };
            // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View(viewModel);
        }

        // POST: Receitas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceitaCreateViewModel viewModel)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            ModelState.Remove("ImagemArquivo");
            
            if(ModelState.IsValid)
            {
                var receita = new Receita
                {
                    Titulo = viewModel.Titulo,
                    TempoPreparoMinutos = viewModel.TempoPreparoMinutos,
                    Instrucoes = viewModel.Instrucoes,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                if (viewModel.ImagemArquivo != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "receitas");
                    Directory.CreateDirectory(uploadsFolder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImagemArquivo.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImagemArquivo.CopyToAsync(fileStream);
                    }
                    
                    receita.ImagemUrl = "/images/receitas/" + fileName;
                }

                _context.Add(receita);
                await _context.SaveChangesAsync();

                if (viewModel.IngredientesSelecionadosIds != null)
                {
                    foreach (var IngredienteId in viewModel.IngredientesSelecionadosIds)
                    {
                        var receitaIngrediente = new ReceitaIngrediente
                        {
                            ReceitaId = receita.Id,
                            IngredienteId = IngredienteId,
                            Quantidade = ""
                        };

                        _context.Add(receitaIngrediente);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            viewModel.IngredientesDisponiveis = _context.Ingrediente
                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Nome })
                .ToList();
                
            return View(viewModel);
        }

        // GET: Receitas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita.FindAsync(id);
            if (receita == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", receita.UserId);
            return View(receita);
        }

        // POST: Receitas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,TempoPreparoMinutos,Instrucoes,ImagemUrl,UserId")] Receita receita)
        {
            if (id != receita.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceitaExists(receita.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", receita.UserId);
            return View(receita);
        }

        // GET: Receitas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receita == null)
            {
                return NotFound();
            }

            return View(receita);
        }

        // POST: Receitas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receita = await _context.Receita.FindAsync(id);
            if (receita != null)
            {
                _context.Receita.Remove(receita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceitaExists(int id)
        {
            return _context.Receita.Any(e => e.Id == id);
        }
    }
}
