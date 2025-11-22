// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable enable

using CookBook.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting; // Para IWebHostEnvironment
using System.IO; // Para manipulação de arquivos
using Microsoft.AspNetCore.Http; // Para IFormFile

namespace CookBook.Areas.Identity.Pages.Account
{
    // A classe RegisterModel foi adaptada para usar ApplicationUser e incluir IWebHostEnvironment.
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        
        // NOVO: Campo para salvar a imagem no servidor
        private readonly IWebHostEnvironment _hostEnvironment;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            // NOVO: Injetar IWebHostEnvironment
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            // ATRIBUIÇÃO
            _hostEnvironment = hostEnvironment;
        }

        // CORRIGIDO: Inicializado para evitar CS8618
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        // CORRIGIDO: Inicializado para evitar CS8618
        public string ReturnUrl { get; set; } = string.Empty;

        // CORRIGIDO: Inicializado para evitar CS8618. Usamos default! para coleções
        public IList<AuthenticationScheme> ExternalLogins { get; set; } = default!; 

        // InputModel adaptado para Username e ProfileImage.
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(256, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 3)]
            [Display(Name = "Nome de Usuário")]
            public string Username { get; set; } = string.Empty; 
            
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            // NOVO: Propriedade para receber a imagem do formulário
            [Display(Name = "Imagem de Perfil")]
            public IFormFile? ProfileImage { get; set; }
        }


        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? string.Empty;
            // CORREÇÃO: Inicializa ExternalLogins antes de usar
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // CORREÇÃO: Define o UserName com o valor do formulário (Input.Username)
                await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    bool profileUpdated = false;

                    // === LÓGICA DE UPLOAD DA IMAGEM NO REGISTRO ===
                    if (Input.ProfileImage != null && Input.ProfileImage.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "profiles");
                        System.IO.Directory.CreateDirectory(uploadsFolder); // Garante que a pasta exista

                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ProfileImage.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Input.ProfileImage.CopyToAsync(fileStream);
                        }

                        // Atualiza a URL da imagem no modelo do usuário
                        user.ProfilePictureUrl = $"/images/profiles/{uniqueFileName}";
                        profileUpdated = true;
                    } 
                    else 
                    {
                        // Opcional: Atribuir uma URL padrão se nenhuma imagem for enviada
                        user.ProfilePictureUrl = "/images/profiles/default-profile.png"; 
                        profileUpdated = true;
                    }
                    
                    // CORREÇÃO: Chama UpdateAsync UMA VEZ para persistir as mudanças (ProfilePictureUrl)
                    if (profileUpdated)
                    {
                        await _userManager.UpdateAsync(user);
                    }
                    // ===============================================
                    
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        // Aviso CS8604 corrigido usando '!'
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                // Garante que ApplicationUser seja instanciável
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            // Garante que o store suporta operações de e-mail
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}