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
            }
            else
            {
                // hang out somewhere for the morning
                scheduleTimes.Add((
                    Activity.Recreation,
                    new DayTimeValue(10)
                ));
            }

            // Go eat at a restaurant if available
            scheduleTimes.Add((
                Activity.EatingAtRestaurant,
                new DayTimeValue(12)
            ));


            // Back to what they were doing before lunch
            // working, if they have a workplace
            if (occupant.office != null)
            {
                scheduleTimes.Add((
                    Activity.Working,
                    new DayTimeValue(13)
                ));

                // attempt to go hang out somewhere after work (if available)
                // TODO - backup activity for this would be BeingAtHome
                scheduleTimes.Add((
                    Activity.Recreation,
                    new DayTimeValue(17, 30)
                ));
            }
            else
            {
                // hang out somewhere all afternoon, if available
                scheduleTimes.Add((
                    Activity.Recreation,
                    new DayTimeValue(14)
                ));
            }

            // Have dinner at restaurant (if available)
            // the backup to this would be being at home
            scheduleTimes.Add((
                Activity.EatingAtRestaurant,
                new DayTimeValue(19)
            ));

            // time to go home
            scheduleTimes.Add((
                Activity.BeingAtHome,
                new DayTimeValue(21)
            ));

            // time to go to sleep
            scheduleTimes.Add((
                Activity.Sleeping,
                new DayTimeValue(22, 30)
            ));
        }
    }
}