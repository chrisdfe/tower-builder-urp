using System.Collections.Generic;

namespace TowerBuilder.DataTypes.Time
{
    public class TimeOfDay
    {
        public enum Key
        {
            MorningNight,
            Dawn,
            Morning,
            Afternoon,
            Evening,
            Dusk,
            DuskNight
        }

        public Key key;
        public int startsOnHour;
        public string name;

        public override string ToString() => $"{key}";

        public TimeOfDay nextTimeOfDay
        {
            get => GetNextTimeOfDay(this);
        }

        public TimeOfDay previousTimeOfDay
        {
            get => GetPreviousTimeOfDay(this);
        }

        public TimeValue relativeStartTime
        {
            get => new TimeValue(new TimeValue.Input() { hour = startsOnHour });
        }

        public static Key GetPreviousTimeOfDayKey(Key currentTimeOfDay) =>
            currentTimeOfDay switch
            {
                Key.MorningNight => Key.DuskNight,
                Key.Dawn => Key.MorningNight,
                Key.Morning => Key.Dawn,
                Key.Afternoon => Key.Morning,
                Key.Evening => Key.Afternoon,
                Key.Dusk => Key.Evening,
                Key.DuskNight => Key.Dusk,
                _ => Key.Dawn
            };


        public static Key GetNextTimeOfDayKey(Key currentTimeOfDay) =>
            currentTimeOfDay switch
            {
                Key.MorningNight => Key.Dawn,
                Key.Dawn => Key.Morning,
                Key.Morning => Key.Afternoon,
                Key.Afternoon => Key.Evening,
                Key.Evening => Key.Dusk,
                Key.Dusk => Key.DuskNight,
                Key.DuskNight => Key.MorningNight,
                _ => Key.Dawn
            };

        public static TimeOfDay GetPreviousTimeOfDay(TimeOfDay timeOfDay) =>
            FindByKey(GetPreviousTimeOfDayKey(timeOfDay.key));

        public static TimeOfDay GetNextTimeOfDay(TimeOfDay timeOfDay) =>
            FindByKey(GetNextTimeOfDayKey(timeOfDay.key));

        public static TimeOfDay FindByKey(Key key) =>
            new List<TimeOfDay>(Constants.TIMES_OF_DAY).Find(timeOfDay => timeOfDay.key == key);
    }
}