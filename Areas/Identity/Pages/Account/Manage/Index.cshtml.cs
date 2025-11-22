// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable enable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CookBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using Microsoft.Extensions.Logging; 

namespace CookBook.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment hostEnvironment) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment; 
        }

        public string Username { get; set; } = string.Empty; 
        public UserManager<ApplicationUser> UserManager => _userManager;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }
            
            [Required] 
            [StringLength(256, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 3)]
            [Display(Name = "Nome de Usuário")]
            public string NewUserName { get; set; } = string.Empty; 

            [Display(Name = "Imagem de Perfil")]
            [DataType(DataType.Upload)]
            public IFormFile? ProfileImage { get; set; }
            
            [EmailAddress]
            public string Email { get; set; } = string.Empty; 
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName ?? string.Empty;

            Input = new InputModel
            {
                NewUserName = userName ?? string.Empty,
                Email = email ?? string.Empty,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
    }
    
    // Carrega os dados atuais, necessário para a validação e exibição em caso de falha.
    var currentUserName = await _userManager.GetUserNameAsync(user);
    var currentPhoneNumber = await _userManager.GetPhoneNumberAsync(user);


    //if (!ModelState.IsValid)
    //{
        // Se o ModelState falhar, carregamos os dados originais novamente para o formulário.
   //     await LoadAsync(user); 
    //    return Page();
    //}
    
    bool identityDataChanged = false; // Flag para refresh de login

    // --- 1. Lógica para Atualizar o Nome de Usuário (UserName) ---
    if (Input.NewUserName != currentUserName)
    {
        var userNameExists = await _userManager.FindByNameAsync(Input.NewUserName);
        
        if (userNameExists != null && userNameExists.Id != user.Id)
        {
            ModelState.AddModelError("Input.NewUserName", "Este nome de usuário já está sendo usado por outra conta.");
            await LoadAsync(user);
            return Page();
        }

        var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.NewUserName);
        if (setUserNameResult.Succeeded)
        {
            identityDataChanged = true;
        }
        else
        {
            // Trata o erro de setUserName
            StatusMessage = "Erro inesperado ao tentar definir o nome de usuário.";
            return RedirectToPage();
        }
    }
    
    // ------------------------------------------------------------------------------------------------
    // --- 2. Lógica para Atualizar o Número de Telefone (CORREÇÃO ADICIONADA) ---
    // ------------------------------------------------------------------------------------------------
    if (Input.PhoneNumber != currentPhoneNumber)
    {
        var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
        if (setPhoneResult.Succeeded)
        {
            identityDataChanged = true;
        }
        else
        {
            // Trata o erro de setPhoneNumber
            StatusMessage = "Erro inesperado ao tentar definir o número de telefone.";
            return RedirectToPage();
        }
    }

    // ------------------------------------------------------------------------------------------------
    // --- 3. Lógica para Upload da Imagem de Perfil (CORREÇÃO ADICIONADA) ---
    // ------------------------------------------------------------------------------------------------
    if (Input.ProfileImage != null && Input.ProfileImage.Length > 0)
    {
        // 3a. Define o caminho para salvar a imagem
        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "profiles");
        // Garante que a pasta de uploads exista
        Directory.CreateDirectory(uploadsFolder); 

        // 3b. Opcional: Deleta a imagem antiga (se não for a padrão)
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && 
            !user.ProfilePictureUrl.EndsWith("default-profile.png")) 
        {
            string oldPath = Path.Combine(_hostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
        }

        // 3c. Salva a nova imagem com um nome único
        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ProfileImage.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await Input.ProfileImage.CopyToAsync(fileStream);
        }
        
        // 3d. Atualiza a URL no modelo do usuário e salva no banco de dados.
        user.ProfilePictureUrl = $"/images/profiles/{uniqueFileName}";
        var updateProfileResult = await _userManager.UpdateAsync(user); // Salva a alteração do ProfilePictureUrl

        if (!updateProfileResult.Succeeded)
        {
            StatusMessage = "Erro inesperado ao salvar a imagem de perfil.";
            return RedirectToPage(); 
        }
    }

    // ------------------------------------------------------------------------------------------------
    // --- 4. Finalização (CORREÇÃO ADICIONADA) ---
    // ------------------------------------------------------------------------------------------------
    // Se o nome de usuário ou telefone mudaram, precisamos atualizar o cookie de autenticação.
    if (identityDataChanged)
    {
        await _signInManager.RefreshSignInAsync(user);
    }
    
    StatusMessage = "Seu perfil foi atualizado com sucesso.";
    return RedirectToPage();
}
    }
}