using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CookBook.Data;
using CookBook.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CookBook.ViewModels;
using System.Text.Json;


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
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.User) // Criador da Receita
                .Include(r => r.ReceitaIngredientes!) // Relação N:N
                    .ThenInclude(ri => ri.Ingrediente)
                .Include(r => r.Comentarios!) // Os comentários da receita
                    .ThenInclude(c => c.User) // O autor de cada comentário
                .FirstOrDefaultAsync(m => m.Id == id);

            if (receita == null)
            {
                return NotFound();
            }

            var viewModel = new ReceitaDetailsViewModel
            {
                Receita = receita,
                ReceitaId = receita.Id
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AdicionarComentario(ReceitaDetailsViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.NovoComentarioTexto))
            {
                // Redireciona de volta para Details se o texto estiver vazio
                return RedirectToAction(nameof(Details), new { id = viewModel.ReceitaId });
            }

            var comentario = new Comentario
            {
                Conteudo = viewModel.NovoComentarioTexto,
                ReceitaId = viewModel.ReceitaId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!, // ID do usuário logado
                DataCriacao = DateTime.Now
            };

            _context.Comentario.Add(comentario);
            await _context.SaveChangesAsync();

            // Redireciona para a mesma página de detalhes (Details)
            return RedirectToAction(nameof(Details), new { id = viewModel.ReceitaId });
        }

        // GET: Receitas/Create
        public IActionResult Create()
        {
            ViewBag.IngredientesDisponiveis = _context.Ingrediente.ToList();
            
            return View(new ReceitaCreateViewModel());
        }

        // POST: Receitas/Create
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
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
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
                
                if (!string.IsNullOrEmpty(viewModel.IngredientesInputData))
                {
                    var inputIngredientes = viewModel.IngredientesInputData.Split(";", StringSplitOptions.RemoveEmptyEntries);

                    var ingredientesExistentes = await _context.Ingrediente.ToListAsync();
                    var ingredientesMap = ingredientesExistentes.ToDictionary(i => i.Nome.ToLower());

                    foreach (var input in inputIngredientes)
                    {
                        var partes = input.Split("|", 2);
                        if (partes.Length != 2)
                        {
                            continue;
                        }

                        var nome = partes[0].Trim();
                        var quantidade = partes[1].Trim();
                        var nomeNormalizado = nome.ToLower();

                        Ingrediente? ingrediente = null;

                        if (ingredientesMap.ContainsKey(nomeNormalizado))
                        {
                            ingrediente = ingredientesMap[nomeNormalizado];
                        } else
                        {
                            //Ingrediente não existe no banco
                            ingrediente = new Ingrediente()
                            {
                                Nome = nome,
                            };

                            _context.Ingrediente.Add(ingrediente);
                            await _context.SaveChangesAsync(); //necessario para obter Id

                            ingredientesMap.Add(nomeNormalizado, ingrediente); //adiciona no map
                        }
                        
                        if (ingrediente != null)
                        {
                            var receitaIngrediente = new ReceitaIngrediente()
                            {
                                ReceitaId = receita.Id,
                                IngredienteId = ingrediente.Id,
                                Quantidade = quantidade
                            };

                            _context.ReceitaIngrediente.Add(receitaIngrediente);
                        }

                    }
                    await _context.SaveChangesAsync(); // salva os ReceitaIngredientes
                }
                
            }  
            return RedirectToAction(nameof(Index));
        }

        // GET: Receitas/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.ReceitaIngredientes)!
                    .ThenInclude(ri => ri.Ingrediente) // Traz a tabela de junção
                .FirstOrDefaultAsync(m => m.Id == id);

            if (receita == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (receita.UserId != userId)
            {
                return Forbid(); 
            }

            var ingredientesExistentes = receita.ReceitaIngredientes!
                .Select(ri => new
                {
                    nome = ri.Ingrediente.Nome,
                    quantidade = ri.Quantidade
                }).ToList();

            var viewModel = new ReceitaEditViewModel
            {
                Id = receita.Id,
                Titulo = receita.Titulo,
                TempoPreparoMinutos = receita.TempoPreparoMinutos,
                Instrucoes = receita.Instrucoes,
                ImagemUrlExistente = receita.ImagemUrl,
                
                IngredientesExistentesJson = JsonSerializer.Serialize(ingredientesExistentes)
            };

            ViewBag.IngredientesDisponiveis = await _context.Ingrediente.ToListAsync();

            return View(viewModel);
        }

        // POST: Receitas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReceitaEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                var receitaDB = await _context.Receita
                    .Include(r => r.ReceitaIngredientes)
                    .FirstOrDefaultAsync(r => r.Id == viewModel.Id);

                if (receitaDB == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (receitaDB.UserId != userId)
                {
                    return Forbid();
                } 

                if (viewModel.NovaImagemArquivo != null)
                {
                    if (!string.IsNullOrEmpty(receitaDB.ImagemUrl))
                    {
                        string oldPath = Path.Combine(_hostEnvironment.WebRootPath,
                                                        receitaDB.ImagemUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "receitas");
                    Directory.CreateDirectory(uploadsFolder);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NovaImagemArquivo.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.NovaImagemArquivo.CopyToAsync(fileStream);
                    }
                    receitaDB.ImagemUrl = "/images/receitas/" + fileName;
                }

                receitaDB.Titulo = viewModel.Titulo;
                receitaDB.TempoPreparoMinutos = viewModel.TempoPreparoMinutos;
                receitaDB.Instrucoes = viewModel.Instrucoes;

                _context.ReceitaIngrediente.RemoveRange(receitaDB.ReceitaIngredientes!);

                if (!string.IsNullOrEmpty(viewModel.IngredientesInputData))
                {
                    var inputIngredientes = viewModel.IngredientesInputData.Split(';', StringSplitOptions.RemoveEmptyEntries);

                    var ingredientesExistentes = await _context.Ingrediente.ToListAsync();
                    var ingredientesMap = ingredientesExistentes.ToDictionary(i => i.Nome.ToLower());

                    foreach (var input in inputIngredientes)
                    {
                        var partes = input.Split('|', 2);
                        if (partes.Length != 2) continue;

                        var nomeIngrediente = partes[0].Trim();
                        var quantidade = partes[1].Trim();
                        var nomeNormalizado = nomeIngrediente.ToLower();
                        
                        Ingrediente? ingrediente = null;

                        // Busca Ingrediente Existente ou Cria Novo
                        if (ingredientesMap.ContainsKey(nomeNormalizado))
                        {
                            ingrediente = ingredientesMap[nomeNormalizado];
                        } else
                        {
                            ingrediente = new Ingrediente { Nome = nomeIngrediente };
                            _context.Ingrediente.Add(ingrediente);
                            await _context.SaveChangesAsync(); 
                            ingredientesMap.Add(nomeNormalizado, ingrediente);
                        }

                        if (ingrediente != null)
                        {
                            var receitaIngrediente = new ReceitaIngrediente
                            {
                                ReceitaId = receitaDB.Id,
                                IngredienteId = ingrediente.Id,
                                Quantidade = quantidade
                            };
                            _context.ReceitaIngrediente.Add(receitaIngrediente);
                        }
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceitaExists(viewModel.Id))
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
            ViewBag.IngredientesDisponiveis = await _context.Ingrediente.ToListAsync();
            
            return View(viewModel);
        }

        // GET: Receitas/Delete/5
        [Authorize]
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (receita.UserId != userId)
            {
                return Forbid(); 
            }

            return View(receita);
        }

        // POST: Receitas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receita = await _context.Receita.FindAsync(id);
            if (receita == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (receita.UserId != userId)
            {
                return Forbid(); 
            }

            if (!string.IsNullOrEmpty(receita.ImagemUrl))
            {
                // Constrói o caminho completo para o arquivo no servidor
                string caminhoCompleto = Path.Combine(_hostEnvironment.WebRootPath, 
                                                    receita.ImagemUrl.TrimStart('/'));

                // Verifica se o arquivo existe e o apaga
                if (System.IO.File.Exists(caminhoCompleto))
                {
                    System.IO.File.Delete(caminhoCompleto);
                }
            }

            _context.Receita.Remove(receita);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        private bool ReceitaExists(int id)
        {
            return _context.Receita.Any(e => e.Id == id);
        }
    }
}
