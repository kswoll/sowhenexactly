using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoWhenExactly.Utils;
using File = Google.Apis.Drive.v3.Data.File;

namespace SoWhenExactly.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public DateTime When { get; set; }

        [BindProperty]
        public string BackgroundUrl { get; set; }

        public async Task OnGetAsync()
        {
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
/*
                        Permissions = new List<Permission>
                        {
                            new Permission
                            {
                                Role = "reader",
                                Type = "anyone"
                            }
                        },
*/
                        Name = "TestFile1"
                    };
                    var upload = service.Files.Create(
                        file,
                        stream,
                        "text/plain");
                    var progress = await upload.UploadAsync();

                    var permissionRequest = service.Permissions.Create(
                        new Permission
                        {
                            Role = "reader",
                            Type = "anyone"
                        },
                        upload.ResponseBody.Id);

                    var permissionResult = await permissionRequest.ExecuteAsync();

                    var getRequest = service.Files.Get(upload.ResponseBody.Id);
                    getRequest.Fields = "id, webContentLink";
                    var getRequestResponse = await getRequest.ExecuteAsync();

                    var link = getRequestResponse.WebContentLink;
                }
            }

        }
    }
}
