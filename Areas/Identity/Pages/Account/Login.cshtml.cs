// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CookBook.Models; // << ADICIONADO: Onde sua classe ApplicationUser está definida

namespace CookBook.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        // Alterado de IdentityUser para ApplicationUser
        private readonly SignInManager<ApplicationUser> _signInManager; 
        private readonly ILogger<LoginModel> _logger;
        // ADICIONADO: Precisamos do UserManager para procurar o usuário pelo Email
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(
            // Alterado de IdentityUser para ApplicationUser no construtor
            SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            // ADICIONADO: Injetar UserManager
            UserManager<ApplicationUser> userManager) 
        {
            _signInManager = signInManager;
            _logger = logger;
            // ATRIBUIÇÃO
            _userManager = userManager; 
        }

        [BindProperty]
        // CORRIGIDO: Inicializado para evitar CS8618
        public InputModel Input { get; set; } = new InputModel();

        // CORRIGIDO: Inicializado para evitar CS8618
        public IList<AuthenticationScheme> ExternalLogins { get; set; } = default!;

        // CORRIGIDO: Inicializado para evitar CS8618
        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        // CORRIGIDO: Inicializado para evitar CS8618
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            // Removendo [EmailAddress] do InputModel para permitir que o usuário digite o Username.
            // O campo agora aceita tanto Email quanto Username.
            [Display(Name = "Email ou Nome de Usuário")]
            public string Email { get; set; } = string.Empty; // Usamos 'Email' como nome da propriedade do modelo para manter o HTML/Razor.

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Lembrar-me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // === LÓGICA DE LOGIN DUPLO ===
                
                ApplicationUser? user = null;
                string loginIdentifier = Input.Email; // O valor que o usuário digitou (pode ser Email ou Username)

                // 1. Tenta encontrar o usuário pelo Email
                if (loginIdentifier.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(loginIdentifier);
                }

                // 2. Se não encontrou (ou se não era um Email), tenta encontrar pelo Nome de Usuário
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(loginIdentifier);
                }

                if (user == null)
                {
                    // Se não encontrou o usuário de forma alguma
                    ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
                    return Page();
                }

                // 3. Usa o Nome de Usuário encontrado (user.UserName) para tentar o login com senha
                // O método PasswordSignInAsync exige o UserName (a chave primária de login)
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!, 
                    Input.Password, 
                    Input.RememberMe, 
                    lockoutOnFailure: false);
                
                // === FIM DA LÓGICA DE LOGIN DUPLO ===

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // Caso a senha esteja incorreta
                    ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}