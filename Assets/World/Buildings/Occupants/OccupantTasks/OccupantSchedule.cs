using UnityEngine;

namespace TowerBuilder
{
    public class OccupantSchedule
    {
        public enum Activity
        {
            Sleeping,
            BeingAtHome,
            Working,
            Nothing,
            // TODO - recreation, eating? other things?
        }

        WorldController worldController;

        Occupant occupant;

        public Activity currentActivity { get; private set; } = Activity.Nothing;

        TimeValue timeToWakeUp;
        TimeValue timeToGoToWork;
        TimeValue timeToGoHome;
        TimeValue timeToGoToSleep;
        int lastDayScheduleWasGenerated;

        public OccupantSchedule(Occupant occupant)
        {
            this.occupant = occupant;

            worldController = WorldController.Get();
        }

        public void OnTick()
        {
            var time = worldController.timeController.timeValue;

            // TODO - create new schedule every morning

            // Regenerate schedule every day at midnight
            if (time.day > lastDayScheduleWasGenerated)
            {
                RandomizeScheduleTimeValues(time);
            }
        }

        Activity GetActivityForTime(TimeValue time)
        {
            // tODO - return next activity
            return Activity.Nothing;
        }

        void RandomizeScheduleTimeValues(TimeValue time)
        {
            // time to wake up
            // any time between 6 & 8:40
            timeToWakeUp = new TimeValue(new TimeValue.Input()
            {
                hour = Random.Range(6, 7),
                minute = Random.Range(0, 40)
            });

            // time to go to work
            // any time between 9:00 and 9:45
            timeToGoToWork = TimeValue.AddMinutes(
                new TimeValue(new TimeValue.Input() { hour = 9 }),
                Random.Range(0, 45)
            );

            // time to go home
            // any time between 4:30 and 60
            timeToGoHome = TimeValue.AddMinutes(
                new TimeValue(new TimeValue.Input() { hour = 4, minute = 30 }),
                Random.Range(0, 90)
            );

            // time to go to sleep
            // any time between 9:30pm and 11:30pm
            timeToGoToSleep = TimeValue.AddMinutes(
                new TimeValue(new TimeValue.Input() { hour = 9, minute = 30 }),
                Random.Range(0, 120)
            );

            lastDayScheduleWasGenerated = time.day;
        }
    }
}