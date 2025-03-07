using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantResidentSchedule : OccupantScheduleBase
    {
        public OccupantResidentSchedule(Occupant occupant) : base(occupant)
        {
            worldController = WorldController.Get();
            currentActivity = Activity.Nothing;
            Regenerate();
        }

        protected override void RegenerateSchedule()
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
        }
    }
}