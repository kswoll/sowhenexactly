using System;
using System.Text;

namespace SoWhenExactly.Utils
{
    public struct DateTimeSpan
    {
        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int Milliseconds { get; }

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            void AddPart(int amount, string unit)
            {
                if (amount <= 0)
                {
                    return;
                }

                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(amount);
                builder.Append(" " + unit);
                if (amount > 1)
                {
                    builder.Append("s");
                }
            }

            AddPart(Years, "Year");
            AddPart(Months, "Month");
            AddPart(Days, "Day");
            AddPart(Hours, "Hour");
            AddPart(Minutes, "Minute");

            return builder.ToString();
        }
    }
}