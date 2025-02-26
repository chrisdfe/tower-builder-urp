namespace TowerBuilder
{
    public class TimeValue
    {
        public struct Input
        {
            public int? minute;
            public int? hour;
            public int? day;
            public int? week;
            public int? season;
            public int? year;
        }

        // 0-59
        public int minute;
        // 0-23
        public int hour;
        // day of week
        public int day;
        // week of season
        public int week;
        // season of year
        public int season;
        // 1 +
        public int year;

        public TimeValue(Input input)
        {
            minute = input.minute ?? 0;
            hour = input.hour ?? 0;
            day = input.day ?? 0;
            week = input.week ?? 0;
            season = input.season ?? 0;
            year = input.year ?? 0;
        }

        public TimeValue() : this(new Input()) { }

        public TimeValue(int minutes) : this(new Input())
        {
            SetFromMinutes(minutes);
        }

        public override string ToString()
        {
            int hour = this.hour + 1;
            int minute = this.minute;

            string amPm = "am";

            if (hour > 12)
            {
                amPm = "pm";
                hour -= 12;
            }

            string hourAsString = hour.ToString();
            // if (hourAsString.Length == 1)
            // {
            //     hourAsString = "0" + hourAsString;
            // }

            string minuteAsString = minute.ToString();
            if (minuteAsString.Length == 1)
            {
                minuteAsString = "0" + minuteAsString;
            }

            return hourAsString + ":" + minuteAsString + amPm;
        }

        public int GetTimeOfDayIndex()
        {
            for (var i = TimeConstants.TIMES_OF_DAY.Length - 1; i >= 0; i--)
            {
                TimeOfDay timeOfDay = TimeConstants.TIMES_OF_DAY[i];

                if (hour >= timeOfDay.startsOnHour)
                {
                    return i;
                }
            }

            return 0;
        }

        public TimeOfDay GetTimeOfDay()
        {
            return TimeConstants.TIMES_OF_DAY[GetTimeOfDayIndex()];
        }

        public int GetPreviousTimeOfDayIndex()
        {
            int index = GetTimeOfDayIndex();
            index--;
            if (index < 0)
            {
                index = TimeConstants.TIMES_OF_DAY.Length - 1;
            }
            return index;
        }

        public int GetNextTimeOfDayIndex()
        {
            int index = GetTimeOfDayIndex();
            index++;
            if (index >= TimeConstants.TIMES_OF_DAY.Length - 1)
            {
                index = 0;
            }
            return index;
        }

        public TimeOfDay GetPreviousTimeOfDay() =>
            TimeConstants.TIMES_OF_DAY[GetPreviousTimeOfDayIndex()];

        public TimeOfDay GetNextTimeOfDay() =>
            TimeConstants.TIMES_OF_DAY[GetNextTimeOfDayIndex()];

        public TimeValue(TimeValue timeValue)
        {
            minute = timeValue.minute;
            hour = timeValue.hour;
            day = timeValue.day;
            week = timeValue.week;
            season = timeValue.season;
            year = timeValue.year;
        }

        public TimeValue Clone()
        {
            return new TimeValue()
            {
                minute = minute,
                hour = hour,
                day = day,
                week = week,
                season = season,
                year = year
            };
        }

        public int AsMinutes()
        {
            int minutes = minute;
            int hourMinutes = hour * TimeConstants.MINUTES_PER_HOUR;
            int dayMinutes = day * TimeConstants.MINUTES_PER_DAY;
            int weekMinutes = week * TimeConstants.MINUTES_PER_WEEK;
            int seasonMinutes = season * TimeConstants.MINUTES_PER_SEASON;
            int yearMinutes = year * TimeConstants.MINUTES_PER_YEAR;

            return (
                minutes +
                hourMinutes +
                dayMinutes +
                weekMinutes +
                seasonMinutes +
                yearMinutes
            );
        }

        public void SetFromMinutes(int minutes)
        {
            int leftover = minutes;
            year = leftover / TimeConstants.MINUTES_PER_YEAR;
            leftover = leftover % TimeConstants.MINUTES_PER_YEAR;

            season = leftover / TimeConstants.MINUTES_PER_SEASON;
            leftover = leftover % TimeConstants.MINUTES_PER_SEASON;

            week = leftover / TimeConstants.MINUTES_PER_WEEK;
            leftover = leftover % TimeConstants.MINUTES_PER_WEEK;

            day = leftover / TimeConstants.MINUTES_PER_DAY;
            leftover = leftover % TimeConstants.MINUTES_PER_DAY;

            hour = leftover / TimeConstants.MINUTES_PER_HOUR;
            leftover = leftover % TimeConstants.MINUTES_PER_HOUR;

            minute = leftover;
        }

        public TimeValue ToRelative() => TimeValue.ToRelative(this);

        // 
        //  Static Interface
        //
        public static TimeValue Zero() =>
            new TimeValue()
            {
                minute = 0,
                hour = 0,
                day = 0,
                week = 0,
                season = 0,
                year = 0,
            };

        public static TimeValue Add(TimeValue timeValue, Input timeInput)
        {
            int timeAsMinutes = timeValue.AsMinutes();
            int timeInputAsMinutes = new TimeValue(timeInput).AsMinutes();

            int newMinutes = timeAsMinutes + timeInputAsMinutes;

            return new TimeValue(newMinutes);
        }

        // TODO - make sure time doesn't go below 0
        public static TimeValue Subtract(TimeValue timeValue, Input timeInput)
        {
            int timeAsMinutes = timeValue.AsMinutes();
            int timeInputAsMinutes = new TimeValue(timeInput).AsMinutes();

            int newMinutes = timeAsMinutes - timeInputAsMinutes;

            return new TimeValue(newMinutes);
        }

        public static TimeValue ToRelative(TimeValue timeValue)
        {
            return new TimeValue(new Input()
            {
                minute = timeValue.minute,
                hour = timeValue.hour,
            });
        }
    }
}