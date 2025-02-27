using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantSchedule
    {
        public enum Activity
        {
            Nothing,
            Sleeping,
            Working,
            BeingAtHome,
            // TODO - recreation, eating? other things?
        }

        WorldController worldController;

        Occupant occupant;

        public Activity currentActivity { get; private set; } = Activity.Nothing;

        List<(Activity, DayTimeValue)> scheduleTimes;
        int lastDayScheduleWasGenerated;

        public OccupantSchedule(Occupant occupant)
        {
            this.occupant = occupant;

            worldController = WorldController.Get();
            RegenerateScheduleTimeValues(worldController.timeController.timeValue);
        }

        public void OnTick()
        {
            var time = worldController.timeController.timeValue;

            var activity = GetActivityForTime(time);


            if (activity != currentActivity)
            {
                Debug.Log($"current: {currentActivity} next: {activity}");
                TransitionToNewActivity(activity);
            }

            // Regenerate schedule every day at midnight
            if (time.day > lastDayScheduleWasGenerated)
            {
                RegenerateScheduleTimeValues(time);
            }
        }

        public void RegenerateScheduleTimeValues(TimeValue currentTime)
        {
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
                    // 7:30am
                    new DayTimeValue(6, 30)
                )
            };

            // time to go to work (if occupant has a work place)
            if (occupant.office != null)
            {
                scheduleTimes.Add((
                    Activity.Working,
                    // 9am
                    new DayTimeValue(8)
                ));

                // time to go home
                scheduleTimes.Add((
                    Activity.BeingAtHome,
                    // 4:30pm
                    new DayTimeValue(15, 30)
                ));
            }

            // time to go to sleep
            scheduleTimes.Add((
                Activity.Sleeping,
                // 10:30pm 
                new DayTimeValue(21, 30)
            ));

            lastDayScheduleWasGenerated = currentTime.day;
        }

        //
        // Private interface
        //
        Activity GetActivityForTime(TimeValue currentTime)
        {
            var dayTimeAsMinutes = currentTime.ToDayTimeValue().AsMinutes();

            // Note - iterating in reverse
            for (var i = scheduleTimes.Count - 1; i > 0; i--)
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
            // TODO - cancel current activity
            switch (activity)
            {
                case Activity.BeingAtHome:
                    // TODO - different task
                    nextTask = new OccupantBeingAtHomeTask(occupant);
                    break;
                case Activity.Sleeping:
                    nextTask = new OccupantSleepingTask(occupant);
                    break;
                case Activity.Working:
                    nextTask = new OccupantWorkingTask(occupant);
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