using System.Collections;
using System.Collections.Generic;

namespace TowerBuilder
{
    public abstract class OccupantScheduleBase
    {
        public enum Activity
        {
            Nothing,
            Sleeping,
            Working,
            BeingAtHome,
            Recreation,
            BeingInHotelRoom,
            EatingAtRestaurant
        }

        protected Occupant occupant;

        public OccupantTaskBase currentTask { get; protected set; }
        public Activity currentActivity { get; protected set; }

        protected List<(Activity, DayTimeValue)> scheduleTimes;
        int lastDayScheduleWasGenerated;

        protected WorldController worldController;

        public OccupantScheduleBase(Occupant occupant)
        {
            this.occupant = occupant;
            worldController = WorldController.Get();
        }

        public virtual void OnTick()
        {
            var time = worldController.timeController.timeValue;

            if (occupant.immediateTask == null)
            {
                currentTask?.OnTick();

                if (currentTask != null && currentTask.isComplete)
                {
                    currentTask.Teardown();
                    currentTask = null;
                }

                var activity = GetActivityForTime(time);
                if (currentTask == null || currentActivity != activity)
                {
                    TransitionToNewActivity(activity);
                }
            }

            // Refresh schedule every day at midnight
            if (time.day > lastDayScheduleWasGenerated)
            {
                Regenerate();
            }
        }

        public void Regenerate()
        {
            RegenerateSchedule();
            lastDayScheduleWasGenerated = worldController.timeController.timeValue.day;
        }

        protected virtual void RegenerateSchedule() { }

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

            return default;
        }

        void TransitionToNewActivity(Activity activity)
        {
            if (occupant.immediateTask != null) return;

            currentTask?.Teardown();

            var nextTask = GetTaskForActivity(activity);

            currentActivity = activity;
            currentTask = nextTask;
            currentTask.Setup();
        }

        OccupantTaskBase GetTaskForActivity(Activity activity) =>
             activity switch
             {
                 Activity.BeingAtHome =>
                    new OccupantBeingAtHomeTask(occupant),
                 Activity.Sleeping =>
                    new OccupantSleepingTask(occupant),
                 Activity.Working =>
                    new OccupantWorkingTask(occupant),
                 Activity.Recreation =>
                    new OccupantRecreationTask(occupant),
                 Activity.BeingInHotelRoom =>
                    new OccupantBeingInHotelRoomTask(occupant),
                 Activity.EatingAtRestaurant =>
                    new OccupantEatingAtRestaurantTask(occupant),
                 _ =>
                     new OccupantIdleTask(occupant),
             };
    }
}