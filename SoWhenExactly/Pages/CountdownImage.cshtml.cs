using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoWhenExactly.Utils;

namespace SoWhenExactly.Pages
{
    public class CountdownImageModel : PageModel
    {
        public async Task<IActionResult> OnGet(string title, DateTime when, DateTime from, string timezoneId, string backgroundUrl = null)
        {
            var countdown = from.Difference(when);

            var stream = new MemoryStream();
            using (var bitmap = new Bitmap(1200, 600, PixelFormat.Format32bppArgb))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                using (var pen = new Pen(Color.Black, 3))
                using (var fontA = new Font("Arial", 25))
                using (var fontB = new Font("Arial", 20))
                using (var fontC = new Font("Arial", 12))
                {
                    if (backgroundUrl != null)
                    {
                        using (var client = new HttpClient())
                        using (var backgroundStream = await client.GetStreamAsync(backgroundUrl))
                        using (var background = Image.FromStream(backgroundStream))
                        {
                            graphics.DrawImage(background, new Rectangle(0, 0, 1200, 600));
                        }
                    }

                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.High;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    int yOffset = 20;
                    int xOffset = 20;

                    void DrawStringRaw(string s, int size)
                    {
                        var emSize = graphics.DpiY * size / 72;
                        using (var path = new GraphicsPath())
                        {
                            path.AddString(s, FontFamily.GenericSansSerif, (int)FontStyle.Regular, emSize, new Point(xOffset, yOffset), new StringFormat());
                            graphics.FillPath(Brushes.AliceBlue, path);
                            graphics.DrawPath(pen, path);
                        }
                        yOffset += (int)emSize;
                    }
                    void DrawString(int amount, string unit)
                    {
                        var s = $"{amount} {unit}";
                        if (amount > 1)
                        {
                            s += "s";
                        }

                        DrawStringRaw(s, 80);
                    }

                    if (title != null)
                    {
                        DrawStringRaw(title, 50);
                        yOffset += 20;
                    }

                    int originalYOffset = yOffset;
                    if (countdown.Years > 0)
                    {
                        DrawString(countdown.Years, "Year");
                    }

                    if (countdown.Months > 0)
                    {
                        DrawString(countdown.Months, "Month");
                    }

                    if (countdown.Days > 0)
                    {
                        DrawString(countdown.Days, "Day");
                    }

                    xOffset = 600;
                    yOffset = originalYOffset;

                    if (countdown.Hours > 0)
                    {
                        DrawString(countdown.Hours, "Hour");
                    }

                    if (countdown.Minutes > 0)
                    {
                        DrawString(countdown.Minutes, "Minute");
                    }

                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;

                    return File(stream, "image/png");
                }
            }
        }
    }
}