using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoWhenExactly.Utils;

namespace SoWhenExactly.Pages
{
    public class CountdownImageModel : PageModel
    {
        public IActionResult OnGet(DateTime when, DateTime from)
        {
            var countdown = from.Difference(when);

            var stream = new MemoryStream();
            using (var bitmap = new Bitmap(1200, 600, PixelFormat.Format32bppArgb))
            {
                bitmap.SetResolution(96 * 2, 96 * 2);
                using (var graphics = Graphics.FromImage(bitmap))
                using (var pen = new Pen(Color.Black))
                using (var fontA = new Font("Arial", 25))
                using (var fontB = new Font("Arial", 20))
                using (var fontC = new Font("Arial", 12))
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    int yOffset = 0;
                    if (countdown.Years > 0)
                    {
                        var s = $"{countdown.Years} Years";
                        graphics.DrawString(s, fontA, Brushes.Black, 0, yOffset);
                        yOffset += fontA.Height + 50;
                    }

                    if (countdown.Months > 0)
                    {
                        graphics.DrawString($"{countdown.Months} Months", fontB, Brushes.Black, 0, yOffset);
                        yOffset += fontB.Height + 50;
                    }

                    if (countdown.Days > 0)
                    {
                        graphics.DrawString($"{countdown.Days} Days", fontB, Brushes.Black, 0, yOffset);
                        yOffset += fontC.Height;
                    }
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;

                    return File(stream, "image/png");
                }
            }
        }
    }
}