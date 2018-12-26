using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace SoWhenExactly.Utils
{
    public class GoogleApi
    {
        public static string GetSignInUrl(string antiForgery, object state)
        {
            var googleSettings = Environment.GetEnvironmentVariable("SOWHEN_GOOGLEAPI");
            var googleSettingsJson = JObject.Parse(googleSettings);
            var googleClientId = (string)googleSettingsJson["web"]["client_id"];

            var stateDictionary = state.ToDictionary();
            stateDictionary["AntiForgery"] = antiForgery;

            var stateString = stateDictionary.ToJsonEncoded();

            using (var provider = new RNGCryptoServiceProvider())
            {
                var seedBytes = new byte[32];
                provider.GetBytes(seedBytes);
                var seed = BitConverter.ToInt32(seedBytes);
                var random = new Random(seed);
                var nonceParts = Enumerable.Repeat(0, 3).Select(_ => random.Next(100000, 999999).ToString()).ToArray();
                var nonce = string.Join("-", nonceParts);

                var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={googleClientId}&response_type=code&scope=openid%20email%20https://www.googleapis.com/auth/drive.file&redirect_uri=https://localhost:44324/signin-google&state={stateString}&openid.realm=localhost&nonce={nonce}";
                return url;
            }
        }

        public static async Task<(string accessToken, JwtSecurityToken jwtToken)> ValidateSignIn(string code)
        {
            var googleSettings = Environment.GetEnvironmentVariable("SOWHEN_GOOGLEAPI");
            var googleSettingsJson = JObject.Parse(googleSettings);
            var googleClientId = (string)googleSettingsJson["web"]["client_id"];
            var googleSecret = (string)googleSettingsJson["web"]["client_secret"];

            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", googleClientId),
                    new KeyValuePair<string, string>("client_secret", googleSecret),
                    new KeyValuePair<string, string>("redirect_uri", "https://localhost:44324/signin-google"),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                };
                var content = new FormUrlEncodedContent(values);
                var response = await (await client.PostAsync("https://www.googleapis.com/oauth2/v4/token", content)).Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(response);
                var accessToken = (string)responseJson["access_token"];
                var idToken = (string)responseJson["id_token"];
                var jwtToken = new JwtSecurityToken(idToken);
                var expiresIn = (string)responseJson["expires_in"];
                var tokenType = (string)responseJson["token_type"];

                return (accessToken, jwtToken);
            }
        }
    }
}