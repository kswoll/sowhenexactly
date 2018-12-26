using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoWhenExactly.Utils;

namespace SoWhenExactly.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet(DateTime when, DateTime from, string timezone)
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                // Create a cookie for the request forgery token
                var bytes = new byte[128];
                provider.GetBytes(bytes);
                var antiForgery = Convert.ToBase64String(bytes);
                HttpContext.Response.Cookies.Append(".Login.Antiforgery", antiForgery, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTimeOffset.Now.AddHours(1),
                    IsEssential = true
                });

                var redirectUrl = GoogleApi.GetSignInUrl(antiForgery, new object());
                return Redirect(redirectUrl);
            }
        }
    }
}