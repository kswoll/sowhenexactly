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
        public async Task<IActionResult> OnGet(DateTime when, DateTime from, string timezoneId, string backgroundUrl = null)
        {
            var countdown = from.Difference(when);

            var stream = new MemoryStream();
            using (var bitmap = new Bitmap(1200, 600, PixelFormat.Format32bppArgb))
            {
//                bitmap.SetResolution(96 * 2, 96 * 2);
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
                            graphics.DrawImage(background, new Rectangle(0, 0, 600, 300));
                        }
                    }

                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.High;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    int yOffset = 0;

                    void DrawString(int amount, string unit, Font font)
                    {
                        var emSize = graphics.DpiY * 20 / 72;
                        using (var path = new GraphicsPath())
                        {
                            var s = $"{amount} {unit}";
                            if (amount > 1)
                            {
                                s += "s";
                            }
                            path.AddString(s, FontFamily.GenericSansSerif, (int)FontStyle.Regular, emSize, new Point(0, yOffset), new StringFormat());
                            graphics.FillPath(Brushes.AliceBlue, path);
                            graphics.DrawPath(pen, path);
                        }
                        yOffset += (int)emSize;
                    }

                    if (countdown.Years > 0)
                    {
                        DrawString(countdown.Years, "Year", fontA);
                    }

                    if (countdown.Months > 0)
                    {
                        DrawString(countdown.Months, "Month", fontB);
                    }

                    if (countdown.Days > 0)
                    {
                        DrawString(countdown.Days, "Day", fontB);
                    }

                    if (countdown.Hours > 0)
                    {
                        DrawString(countdown.Hours, "Hour", fontB);
                    }

                    if (countdown.Minutes > 0)
                    {
                        DrawString(countdown.Hours, "Minute", fontB);
                    }

                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;

                    return File(stream, "image/png");
                }
            }
        }
    }
}