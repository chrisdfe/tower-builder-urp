namespace TowerBuilder
{
    // TODO - recreation
    // TODO - sleep at night time
    // TODO - leave at checkout time
    public class OccupantHotelGuestSchedule : OccupantScheduleBase
    {
        public OccupantHotelGuestSchedule(Occupant occupant) : base(occupant) { }

        protected override void RegenerateSchedule()
        {
            // TODO - some variability
            scheduleTimes = new()
            {
                // sleeping
                (
                    Activity.Sleeping,
                    new DayTimeValue(0, 0)
                ),
                // time to wake up
                (
                    Activity.BeingInHotelRoom,
                    new DayTimeValue(8, 30)
                ),

                //
                // Note - check in happens right here
                //

                // hang out somewhere for a bit (if available)
                (
                    Activity.Recreation,
                    new DayTimeValue(18)
                ),

                // time to go back to the room
                (
                    Activity.BeingInHotelRoom,
                    new DayTimeValue(20)
                ),

                // time to go to sleep
                (
                    Activity.Sleeping,
                    new DayTimeValue(22)
                )
            };
        }
    }
}