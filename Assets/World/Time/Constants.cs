using System;
using System.Collections;
using System.Collections.Generic;
using TowerBuilder.DataTypes.Time;
using TowerBuilder.Utils;
using UnityEngine;

namespace TowerBuilder.DataTypes.Time
{
    public static class Constants
    {
        public const int MINUTES_ELAPSED_PER_TICK = 5;

        // In seconds
        public const float TICK_INTERVAL = .8f;

        // tick intervals in seconds
        public static Dictionary<TimeSpeed, float> TIME_SPEED_TICK_INTERVALS = new Dictionary<TimeSpeed, float>()
        {
            [TimeSpeed.Pause] = float.PositiveInfinity,
            // [TimeSpeed.Normal] = 1.0f,
            [TimeSpeed.Normal] = 1.0f,
            // [TimeSpeed.Fast] = 0.7f,
            [TimeSpeed.Fast] = 0.5f,
            [TimeSpeed.Fastest] = 0.05f,
            // [TimeSpeed.Fastest] = 0.5f,
        };

        public const int MINUTES_PER_HOUR = 60;

        public const int HOURS_PER_DAY = 24;

        public const int MINUTES_PER_DAY = HOURS_PER_DAY * MINUTES_PER_HOUR;

        public const int TICKS_PER_DAY = MINUTES_PER_DAY * MINUTES_ELAPSED_PER_TICK;

        public static DayType[] WEEKDAY_SEQUENCE = new DayType[]
        {
            DayType.Weekday,
            DayType.Weekday,
            DayType.Weekday,
            DayType.Weekend,
            DayType.Weekend,
        };

        public static int DAYS_PER_WEEK = WEEKDAY_SEQUENCE.Length;

        public static Dictionary<DayType, String> WEEKDAY_LABEL_MAP = new Dictionary<DayType, String>
        {
            [DayType.Weekday] = "Weekday",
            [DayType.Weekend] = "Weekend",
        };

        public static int MINUTES_PER_WEEK = DAYS_PER_WEEK * MINUTES_PER_DAY;

        public const int WEEKS_PER_SEASON = 4;

        public static SeasonType[] SEASON_SEQUENCE = new SeasonType[]
        {
                SeasonType.Spring,
                SeasonType.Summer,
                SeasonType.Autumn,
                SeasonType.Winter,
        };

        public static Dictionary<SeasonType, String> SEASON_LABEL_MAP = new Dictionary<SeasonType, String>()
        {
            [SeasonType.Spring] = "Spring",
            [SeasonType.Summer] = "Summer",
            [SeasonType.Autumn] = "Autumn",
            [SeasonType.Winter] = "Winter",
        };

        public static int MINUTES_PER_SEASON = WEEKS_PER_SEASON * MINUTES_PER_WEEK;

        public static int SEASONS_PER_YEAR = SEASON_SEQUENCE.Length;

        public static int MINUTES_PER_YEAR = SEASONS_PER_YEAR * MINUTES_PER_SEASON;

        public static TimeOfDay[] TIMES_OF_DAY = new TimeOfDay[] {
            new TimeOfDay() {
                key = TimeOfDay.Key.MorningNight,
                startsOnHour = 0,
                name = "Night",
            },
            new TimeOfDay() {
                key = TimeOfDay.Key.Dawn,
                startsOnHour = 5,
                name = "Dawn",
            },
            new TimeOfDay() {
                key = TimeOfDay.Key.Morning,
                startsOnHour = 7,
                name = "Morning",
            },
            new TimeOfDay() {
                key = TimeOfDay.Key.Afternoon,
                startsOnHour = 12,
                name = "Afternoon",
            },
            new TimeOfDay() {
                key = TimeOfDay.Key.Evening,
                startsOnHour = 17,
                name = "Evening",
            },
            new TimeOfDay() {
                key = TimeOfDay.Key.Dusk,
                startsOnHour = 19,
                name = "Dusk",
            },
            new TimeOfDay() {
                key = TimeOfDay.Key.DuskNight,
                startsOnHour = 21,
                name = "Night",
            },
        };
    };
}
