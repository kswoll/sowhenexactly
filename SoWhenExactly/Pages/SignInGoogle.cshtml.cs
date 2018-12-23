using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoWhenExactly.Utils;

namespace SoWhenExactly.Pages
{
    public class SignInGoogleModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string state, string code)
        {
/*
            var antiForgery = HttpContext.Request.Cookies.First(x => x.Key.StartsWith(".Login.Antiforgery")).Value;
            if (state != antiForgery)
            {
                return Unauthorized();
            }
*/

            var (accessToken, jwtToken) = await GoogleApi.ValidateSignIn(code);
            HttpContext.Response.Cookies.Append(".google-token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Expires = DateTimeOffset.Now.AddHours(1),
                IsEssential = true
            });

            return RedirectToPage("Index");
        }
    }
}