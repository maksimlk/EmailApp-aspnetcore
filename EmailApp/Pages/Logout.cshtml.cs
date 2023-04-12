using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailApp.Pages
{
    public class LogoutModel : PageModel
    {
		public string? ReturnUrl { get; set; }
		public string? ErrorMessage { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			// Clear the existing external cookie
			await HttpContext.SignOutAsync(
				CookieAuthenticationDefaults.AuthenticationScheme);

            return LocalRedirect("/Login/");
        }
    }
}
