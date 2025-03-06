using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantResidentSchedule : OccupantScheduleBase
    {
        public enum Activity
        {
            Nothing,
            Sleeping,
            Working,
            BeingAtHome,
            Recreation
        }

        WorldController worldController;

        public Activity currentActivity { get; private set; } = Activity.Nothing;

        protected List<(Activity, DayTimeValue)> scheduleTimes;
        int lastDayScheduleWasGenerated;

        public OccupantResidentSchedule(Occupant occupant) : base(occupant)
        {
            worldController = WorldController.Get();
            Regenerate();
        }

        public override void OnTick()
        {
            var time = worldController.timeController.timeValue;

            var activity = GetActivityForTime(time);

            if (activity != currentActivity)
            {
                TransitionToNewActivity(activity);
            }

            // Refresh schedule every day at midnight
            if (time.day > lastDayScheduleWasGenerated)
            {
                Regenerate();
            }
        }

        public override void Regenerate()
        {
            var timeValue = worldController.timeController.timeValue;

            scheduleTimes = new()
            {
                // sleeping
                (
                    Activity.Sleeping,
                    new DayTimeValue(0, 0)
                ),
                // time to wake up
                (
                    Activity.BeingAtHome,
                    new DayTimeValue(6, 30)
                )
            };

            // time to go to work (if occupant has a work place)
            if (occupant.office != null)
            {
                scheduleTimes.Add((
                    Activity.Working,
                    new DayTimeValue(8)
                ));

                // attempt to go hang out somewhere first
                scheduleTimes.Add((
                    Activity.Recreation,
                    new DayTimeValue(15, 30)
                ));
            }
            else
            {
                // hang out somewhere all day, if available
                scheduleTimes.Add((
                    Activity.Recreation,
                    new DayTimeValue(10)
                ));
            }

            // time to go home
            scheduleTimes.Add((
                Activity.BeingAtHome,
                new DayTimeValue(19, 30)
            ));

            // time to go to sleep
            scheduleTimes.Add((
                Activity.Sleeping,
                new DayTimeValue(21, 30)
            ));

            lastDayScheduleWasGenerated = timeValue.day;
        }

        //
        // Private interface
        //
        Activity GetActivityForTime(TimeValue currentTime)
        {
            var dayTimeAsMinutes = currentTime.ToDayTimeValue().AsMinutes();

            // Note - iterating in reverse
            for (var i = scheduleTimes.Count - 1; i >= 0; i--)
            {
                var (activity, timeForActivity) = scheduleTimes[i];

                if (timeForActivity.AsMinutes() <= dayTimeAsMinutes)
                {
                    return activity;
                }
            }

            return Activity.Nothing;
        }

        void TransitionToNewActivity(Activity activity)
        {
            occupant.CancelCurrentTask();

            IOccupantTask nextTask;

            switch (activity)
            {
                case Activity.BeingAtHome:
                    nextTask = new OccupantBeingAtHomeTask(occupant);
                    break;
                case Activity.Sleeping:
                    nextTask = new OccupantSleepingTask(occupant);
                    break;
                case Activity.Working:
                    nextTask = new OccupantWorkingTask(occupant);
                    break;
                case Activity.Recreation:
                    nextTask = new OccupantRecreationTask(occupant);
                    break;
                default:
                    nextTask = new OccupantIdleTask();
                    break;
            }

            occupant.EnqueueTask(nextTask);

            currentActivity = activity;
        }
    }
}