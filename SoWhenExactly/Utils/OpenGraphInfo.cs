using Microsoft.AspNetCore.Html;

namespace SoWhenExactly.Utils
{
    public class OpenGraphInfo
    {
        public string Title { get; set; }
        public IHtmlContent Description { get; set; }
        public IHtmlContent Image { get; set; }
    }

}