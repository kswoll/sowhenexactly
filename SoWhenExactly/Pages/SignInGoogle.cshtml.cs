using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var antiForgery = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key.StartsWith(".Login.Antiforgery")).Value;

            var stateDictionary = state.FromJsonEncoded();
            var antiForgeryInState = (string)stateDictionary["AntiForgery"];
            stateDictionary.Remove("AntiForgery");

            // There might be + characters in the cookie to indicate a space, so we decode
            // it so it will match state (which would have already been decoded)
//            var antiForgeryDecoded = WebUtility.UrlDecode(antiForgery);

            if (antiForgeryInState != antiForgery)
            {
                return Unauthorized();
            }

            var (accessToken, _) = await GoogleApi.ValidateSignIn(code);
            HttpContext.Response.Cookies.Append(".google-token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Expires = DateTimeOffset.Now.AddHours(1),
                IsEssential = true
            });

            return RedirectToPage("Index", stateDictionary);
        }
    }
}