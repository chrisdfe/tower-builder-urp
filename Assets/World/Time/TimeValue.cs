using System;
using System.Collections;
using System.Collections.Generic;

namespace TowerBuilder.DataTypes.Time
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

        public int timeOfDayIndex
        {
            get
            {
                for (int i = Constants.TIMES_OF_DAY.Length - 1; i >= 0; i--)
                {
                    TimeOfDay timeOfDay = Constants.TIMES_OF_DAY[i];

                    if (hour >= timeOfDay.startsOnHour)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }

        public TimeOfDay timeOfDay
        {
            get
            {
                return Constants.TIMES_OF_DAY[timeOfDayIndex];
            }
        }

        public int previousTimeOfDayIndex
        {
            get
            {
                int index = timeOfDayIndex;
                index--;
                if (index < 0)
                {
                    index = Constants.TIMES_OF_DAY.Length - 1;
                }
                return index;
            }
        }

        public int nextTimeOfDayIndex
        {
            get
            {
                int index = timeOfDayIndex;
                index++;
                if (index >= Constants.TIMES_OF_DAY.Length - 1)
                {
                    index = 0;
                }
                return index;
            }
        }

        public TimeOfDay previousTimeOfDay
        {
            get => Constants.TIMES_OF_DAY[previousTimeOfDayIndex];
        }

        public TimeOfDay nextTimeOfDay
        {
            get => Constants.TIMES_OF_DAY[nextTimeOfDayIndex];
        }

        public TimeValue(TimeValue timeValue)
        {
            minute = timeValue.minute;
            hour = timeValue.hour;
            day = timeValue.day;
            week = timeValue.week;
            season = timeValue.season;
            year = timeValue.year;
        }

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

        public override string ToString() =>
            $"{hour}:{minute}, day: {day}, week: {week}, season: {season}, year: {year}";

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
            int hourMinutes = hour * Constants.MINUTES_PER_HOUR;
            int dayMinutes = day * Constants.MINUTES_PER_DAY;
            int weekMinutes = week * Constants.MINUTES_PER_WEEK;
            int seasonMinutes = season * Constants.MINUTES_PER_SEASON;
            int yearMinutes = year * Constants.MINUTES_PER_YEAR;

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
            year = leftover / Constants.MINUTES_PER_YEAR;
            leftover = leftover % Constants.MINUTES_PER_YEAR;

            season = leftover / Constants.MINUTES_PER_SEASON;
            leftover = leftover % Constants.MINUTES_PER_SEASON;

            week = leftover / Constants.MINUTES_PER_WEEK;
            leftover = leftover % Constants.MINUTES_PER_WEEK;

            day = leftover / Constants.MINUTES_PER_DAY;
            leftover = leftover % Constants.MINUTES_PER_DAY;

            hour = leftover / Constants.MINUTES_PER_HOUR;
            leftover = leftover % Constants.MINUTES_PER_HOUR;

            minute = leftover;
        }

        public TimeValue ToRelative()
        {
            return TimeValue.ToRelative(this);
        }

        /* 
            Static Interface
        */
        public static TimeValue zero
        {
            get
            {
                return new TimeValue()
                {

                    minute = 0,
                    hour = 0,
                    day = 0,
                    week = 0,
                    season = 0,
                    year = 0,
                };
            }
        }

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
            return new TimeValue(new TimeValue.Input()
            {
                minute = timeValue.minute,
                hour = timeValue.hour,
            });
        }
    }
}