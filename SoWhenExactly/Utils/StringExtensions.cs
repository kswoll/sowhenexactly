using System.Net;
using Microsoft.AspNetCore.Html;

namespace SoWhenExactly.Utils
{
    public static class StringExtensions
    {
        public static IHtmlContent ToHtmlContent(this string s)
        {
            return new HtmlString(s);
        }

        public static string UrlEncode(this object o)
        {
            return o == null ? "" : WebUtility.UrlEncode(o.ToString());
        }
    }
}