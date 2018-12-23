using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoWhenExactly.Utils;

namespace SoWhenExactly.Pages
{
    public class CountdownModel : PageModel
    {
        [BindProperty]
        public DateTime? When { get; set; }

        [BindProperty]
        public DateTimeSpan Countdown { get; set; }

        public void OnGet(DateTime? when, DateTime from)
        {
            When = when;
            if (when != null)
            {
                var now = DateTime.UtcNow;
                Countdown = now.Difference(when.Value);
            }
        }
    }
}