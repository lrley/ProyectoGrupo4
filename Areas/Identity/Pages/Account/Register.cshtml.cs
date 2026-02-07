// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using DlaccessCore.Models.Models.IdentidadDeUsuario;
using DlaccessCore.Models.ViewModels;
using DlaccessCore.Utilidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace DLACCESS.Areas.Identity.Pages.Account
{
    

    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public PaisesCiudadesVM PaisesCiudadesVM { get; set; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "El Email es Requerido")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
         


            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }




            [Required(ErrorMessage = "El Nombre es Obligatorio")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "La cédula es obligatoria")]
            [StringLength(13, MinimumLength = 10)]
            [RegularExpression(@"^\d{10,13}$", ErrorMessage = "La cédula solo puede contener números")]
            public string Cedula { get; set; }


            [Required(ErrorMessage = "El Direccion es Obligatorio")]
            [StringLength(100, ErrorMessage = "La Direccion no puede exceder 100 dígitos")]
            public string Direccion { get; set; }

            [Required(ErrorMessage = "La Ciudad es Obligatoria")]
            public string Ciudad { get; set; }

            [Required(ErrorMessage = "El Pais es Obligatorio")]
            public string Pais { get; set; }

            [Phone(ErrorMessage = "El número de teléfono no es válido")]
            [StringLength(15, ErrorMessage = "El teléfono no puede exceder 15 dígitos")]
            public string PhoneNumber { get; set; }

            public bool Permiso { get; set; } = false;

            public bool Estado { get; set; } = true;

            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime UpdatedAt { get; set; } = DateTime.Now;



        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            PaisesCiudadesVM = new PaisesCiudadesVM();


            Input = new InputModel
            {
                Pais = "Ecuador",
                Ciudad = "Guayaquil"
            };

      


        }




        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                Input.Nombre = Input.Nombre?.ToUpper();
                Input.Email = Input.Email?.ToLower();
               // Input.Pais = Input.Pais?.ToUpper();
                Input.Direccion = Input.Direccion?.ToUpper();
                //Input.Ciudad = Input.Ciudad?.ToUpper();

                var user = CreateUser();

                user.Nombre = Input.Nombre;
                user.Cedula = Input.Cedula;
                user.Email = Input.Email;
                user.Direccion = Input.Direccion;
                user.Pais = Input.Pais;
                user.Ciudad = Input.Ciudad;
                user.PhoneNumber = Input.PhoneNumber;

    


                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {

                    if (!await _roleManager.RoleExistsAsync(CNT.Administrador)) {
                        await _roleManager.CreateAsync(new IdentityRole(CNT.Administrador));
                        await _roleManager.CreateAsync(new IdentityRole(CNT.Registrado));
                        await _roleManager.CreateAsync(new IdentityRole(CNT.Cliente));
                    }

                    //Obtener el Rol Seleccionado
                    string rol = Request.Form["radUsuarioRole"].ToString();

                    if (rol == CNT.Administrador) { 
                        await _userManager.AddToRoleAsync(user, CNT.Administrador);

                    }
                    else
                    {
                        if (rol == CNT.Registrado)
                        {
                            await _userManager.AddToRoleAsync(user, CNT.Registrado);

                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, CNT.Cliente);

                        }


                    }



                        _logger.LogInformation("El Usuario se creo con Exito..");




                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirma tu Email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

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

            PaisesCiudadesVM = new PaisesCiudadesVM();
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
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
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
