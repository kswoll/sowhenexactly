using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime.TimeZones;
using SoWhenExactly.Utils;
using File = Google.Apis.Drive.v3.Data.File;

namespace SoWhenExactly.Pages
{
    public class IndexModel : PageModel
    {
        public string Title { get; set; }
        public DateTime Date { get; set; } = DateTime.Now.AddDays(1);
        public TimeSpan Time { get; set; } = TimeSpan.Zero;
        public string BackgroundUrl { get; set; }
        public string TimeZone { get; set; }
        public SelectListItem[] TimeZones { get; set; }
        public bool HasBackground { get; set; }

        public async Task OnGetAsync(string title, DateTime? when, string timezone, bool hasBackground = false, string backgroundUrl = null)
        {
            if (when != null)
            {
                Date = when.Value.Date;
                Time = when.Value.TimeOfDay;
            }

            Title = title;
            TimeZone = timezone;
            HasBackground = hasBackground;
            BackgroundUrl = backgroundUrl;

            TimeZones = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones
                .SelectMany(x => x.TzdbIds, (tz, id) => new { tz, id })
                .OrderBy(x => x.id)
                .Where(x => !x.id.StartsWith("Etc/"))
                .Select(x => new SelectListItem
                {
                    Text = x.id.Replace("/", " / ").Replace("_", " "),
                    Value = x.id
                })
                .ToArray();
/*
            TimeZones = TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(x => x.BaseUtcOffset.TotalHours)
                .Select(x => new SelectListItem
                {
                    Text = x.StandardName,
                    Value = x.BaseUtcOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture)
                })
                .ToArray();
*/

//            TimeZoneInfo.GetSystemTimeZones().Where(x => x.BaseUtcOffset.Hours)

            return;

            var accessToken = HttpContext.Request.Cookies[".google-token"];
            if (accessToken != null)
            {
                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
                    ApplicationName = "SoWhenExactly"
                });

                using (var stream = new FileStream(@"c:\temp\diff1.txt", FileMode.Open))
                {
                    var file = new File
                    {
                        Description = "Test File1",
                        Name = "TestFile1"
                    };
                    var upload = service.Files.Create(
                        file,
                        stream,
                        "text/plain");
                    await upload.UploadAsync();

                    var permissionRequest = service.Permissions.Create(
                        new Permission
                        {
                            Role = "reader",
                            Type = "anyone"
                        },
                        upload.ResponseBody.Id);

                    await permissionRequest.ExecuteAsync();

                    var getRequest = service.Files.Get(upload.ResponseBody.Id);
                    getRequest.Fields = "id, webContentLink";
                    var getRequestResponse = await getRequest.ExecuteAsync();

                    var link = getRequestResponse.WebContentLink;
                }
            }
        }


        public IActionResult OnPost(string title, DateTime date, TimeSpan time, string timezone, string backgroundUrl)
        {
            var when = date.Add(time);
            return RedirectToPage("Countdown", new { title, when, from = DateTime.Now, timezone, backgroundUrl });
        }

        public IActionResult OnPostAddBackground(string title, DateTime date, TimeSpan time, string timezone)
        {
            var when = date.Add(time);
            return RedirectToPage("Index", new { title, when, from = DateTime.Now, timezone, hasBackground = true });

/*
Log in to google
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

                var redirectUrl = GoogleApi.GetSignInUrl(antiForgery, new { when, from = DateTime.Now, timezone, hasBackground = true });
                return Redirect(redirectUrl);
            }
*/
        }
/*

            return RedirectToPage("Login", new { when, from = DateTime.Now, timezone, hasBackground = true });
        }

*/
        public IActionResult OnPostPreview(string title, DateTime date, TimeSpan time, string timezone, string backgroundUrl)
        {
            return RedirectToPage("Index", new { title, when = date.Add(time), timezone, backgroundUrl, hasBackground = backgroundUrl != null });
        }
    }
}
