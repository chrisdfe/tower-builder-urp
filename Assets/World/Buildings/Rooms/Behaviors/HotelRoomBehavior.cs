using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class HotelRoomBehavior : RoomBehaviorBase
    {
        // 4pm
        readonly static DayTimeValue checkinTimeMin = new(16, 0);
        // 7pm
        readonly static DayTimeValue checkinTimeMax = new(19, 0);

        // 7am
        public readonly static DayTimeValue checkoutTimeMin = new(7, 0);
        // 11am
        public readonly static DayTimeValue checkoutTimeMax = new(11, 0);

        // 3pm 
        public readonly static DayTimeValue cashoutTime = new(15, 0);

        // guests are staying/have stayed the night 
        public int guestCount { get; private set; }

        // how much per guest per night this hotel room earns
        // TODO - ability to set this
        public int price { get; private set; } = 100;

        public HotelRoomBehavior(Room room) : base(room) { }

        public override void OnTick()
        {
            //
        }

        public override void OnTickAll(List<Room> rooms)
        {
            //
        }

        public int GetTotalProfit() => guestCount * price;

        public void AddGuest()
        {
            guestCount++;
            Debug.Log("Adding Guest. now: " + guestCount);
        }

        public void ResetGuests()
        {
            guestCount = 0;
        }

        //
        // Static interface
        //
        public static DayTimeValue GetRandomCheckinTime() => DayTimeValue.RandomBetween(checkinTimeMin, checkinTimeMax);

        public static DayTimeValue GetRandomCheckoutTime() => DayTimeValue.RandomBetween(checkoutTimeMin, checkoutTimeMax);
    }
}