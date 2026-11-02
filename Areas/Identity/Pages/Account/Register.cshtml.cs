using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GiftOfTheGivers.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            [Display(Name = "Confirm password")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? Url.Content("~") : returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? Url.Content("~") : returnUrl;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email.Trim(),
                Email = Input.Email.Trim(),
                FullName = Input.FullName.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Donor");
                if (!roleResult.Succeeded)
                {
                    var deleteResult = await _userManager.DeleteAsync(user);
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    if (!deleteResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Your account was created, but donor access could not be assigned. Please contact support before trying again.");
                    }

                    return Page();
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return string.IsNullOrWhiteSpace(returnUrl)
                    ? Redirect(Url.Content("~/"))
                    : LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
