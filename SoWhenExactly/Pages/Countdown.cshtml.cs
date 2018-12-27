using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;
using NodaTime.TimeZones;
using SoWhenExactly.Utils;

namespace SoWhenExactly.Pages
{
    public class CountdownModel : PageModel
    {
        public string Title { get; set; }
        public DateTime When { get; set; }
        public DateTime From { get; set; }
        public DateTimeSpan Countdown { get; set; }
        public string TimeZone { get; set; }
        public string BackgroundUrl { get; set; }

        public void OnGet(string title, DateTime when, DateTime from, string timezone, string backgroundUrl = null)
        {
            Title = title;
            When = when;
            From = from;
            BackgroundUrl = backgroundUrl;

            var tz = TzdbDateTimeZoneSource.Default.ForId(timezone);

            var fromLocal = tz.AtLeniently(LocalDateTime.FromDateTime(when));
            var toLocal = tz.AtLeniently(LocalDateTime.FromDateTime(from));
            var fromUtc = fromLocal.ToDateTimeUtc();
            var toUtc = toLocal.ToDateTimeUtc();

            Countdown = fromUtc.Difference(toUtc);
        }
    }
}