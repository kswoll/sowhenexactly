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
        public IActionResult OnGet()
        {
            // Request forgery token
            using (var provider = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[128];
                provider.GetBytes(bytes);
                var antiForgery = Convert.ToBase64String(bytes);
                HttpContext.Response.Cookies.Append(".Login.Antiforgery", antiForgery, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None
                });

                var redirectUrl = GoogleApi.GetSignInUrl(antiForgery);
                return Redirect(redirectUrl);
            }

            //            var antiForgery = HttpContext.Request.Cookies.First(x => x.Key.StartsWith(".AspNetCore.Antiforgery")).Value;
        }
    }
}