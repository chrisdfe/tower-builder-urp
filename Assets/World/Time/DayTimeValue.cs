using UnityEngine;

namespace TowerBuilder
{
    // Not to be confused with TimeOfDay and TimeValue!
    // A generic time value with only of hours and minutes
    public class DayTimeValue
    {
        public int hour = 0;
        public int minute = 0;

        public override string ToString()
        {
            var hourAsString = hour.ToString();
            if (hourAsString.Length == 1)
            {
                hourAsString = "0" + hourAsString;
            }

            var minuteAsString = minute.ToString();
            if (minuteAsString.Length == 1)
            {
                minuteAsString = "0" + minuteAsString;
            }

            return $"{hourAsString}:{minuteAsString}";
        }

        public DayTimeValue(int hour, int minute)
        {
            this.hour = hour;
            this.minute = minute;
        }

        public DayTimeValue(int hour)
        {
            this.hour = hour;
        }

        public int AsMinutes()
        {
            int minutes = minute;
            int hourMinutes = hour * TimeConstants.MINUTES_PER_HOUR;

            return minutes + hourMinutes;
        }

        public bool Matches(DayTimeValue other) =>
            hour == other.hour && minute == other.minute;

        public bool IsGreaterThan(DayTimeValue other) =>
            hour > other.hour ||
            (hour == other.hour) && minute > other.minute;

        public bool IsGreaterThanOrEqualTo(DayTimeValue other) =>
            Matches(other) || IsGreaterThan(other);

        //
        // Static interface
        //
        public static DayTimeValue FromMinutes(int minutes)
        {
            int leftover = minutes;

            var hour = leftover / TimeConstants.MINUTES_PER_HOUR;
            hour %= 23;
            leftover = leftover % TimeConstants.MINUTES_PER_HOUR;

            var minute = leftover;

            return new DayTimeValue(hour, minute);
        }

        public static DayTimeValue Add(DayTimeValue a, DayTimeValue b)
        {
            var aAsMinutes = a.AsMinutes();
            var bAsMinutes = b.AsMinutes();

            var total = aAsMinutes + bAsMinutes;

            return DayTimeValue.FromMinutes(total);
        }

        public static DayTimeValue RandomBetween(DayTimeValue a, DayTimeValue b)
        {
            var aAsMinutes = a.AsMinutes();
            var bAsMinutes = b.AsMinutes();

            int lower, higher;
            if (aAsMinutes <= bAsMinutes)
            {
                lower = aAsMinutes;
                higher = bAsMinutes;
            }
            else
            {
                lower = bAsMinutes;
                higher = aAsMinutes;
            }
            var minutes = Random.Range(lower, higher);

            return FromMinutes(minutes);
        }

        public static DayTimeValue midnight = new DayTimeValue(0, 0);
    }
}