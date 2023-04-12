using EmailApp.Data;
using EmailApp.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EmailApp.Pages
{
    public class LoginModel : PageModel
    {

        private readonly MailAppDbContext _context;

        public LoginModel(MailAppDbContext context)
        {
            _context = context;   
        }


        [BindProperty]
        public InputModel? Input { get; set; }
        public string? ReturnUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Name is necessary!")]
            [StringLength(50, ErrorMessage = "Maximum length of the name is 50 letters!")]
            [DataType(DataType.Text)]
            public string? Name { get; set; }
        }
        public async Task OnGetAsync(string? returnUrl = null)
        {
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			returnUrl ??= Url.Content("~/");
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			ReturnUrl = returnUrl;
		}

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid && Input?.Name != null)
            {
                string name = Input.Name.ToLower();
                await AddUserIfNotPresentAsync(name);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    GetPrincipalClames(name));
            }
            return LocalRedirect(returnUrl);
        }


        private async Task AddUserIfNotPresentAsync(string name)
        {
			var userInDB = await _context.Users.FirstOrDefaultAsync(u => u.UserName == name);
			if (userInDB == null)
			{
                _context.Users.Add(new MailUser { UserName = name });
                _context.SaveChanges();
			}
		}

        private ClaimsPrincipal GetPrincipalClames(string name)
        {
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, name),
				};
			var claimsIdentity = new ClaimsIdentity(claims,
				CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
		}
    }
}
