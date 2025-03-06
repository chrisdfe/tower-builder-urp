using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    public class BuildingHotelRoomManager
    {
        Building building;

        WorldController worldController;

        // TODO - shouldn't this live on building on buildingsController?
        List<Occupant> hotelGuests = new();

        // TODO - I should stagger the guests' arrival/departure
        bool hasCheckedIn;
        bool hasCheckedOut;
        DayTimeValue checkinTime;
        DayTimeValue checkoutTime;

        public BuildingHotelRoomManager(Building building)
        {
            this.building = building;

            worldController = WorldController.Get();
            ResetDay();
        }

        public void OnTick()
        {
            var hotelRooms = building.GetRoomsByType(RoomType.Hotel);
            var entrance = building.GetEntrance();

            if (hotelRooms.Count == 0) return;

            var currentDayTime = worldController.timeController.timeValue.ToDayTimeValue();

            if (currentDayTime.Matches(DayTimeValue.midnight))
            {
                ResetDay();
            }

            if (currentDayTime.IsGreaterThanOrEqualTo(checkinTime) && !hasCheckedIn)
            {
                foreach (var hotelRoom in hotelRooms)
                {
                    var routeFinder = new OccupantRouteFinder(hotelRoom.building, entrance.GetRandomTile(), hotelRoom.GetRandomTile());
                    var route = routeFinder.FindRoute();
                    if (route == null)
                    {
                        // No route found to this hotel room
                        continue;
                    }

                    var guestCount = Random.Range(1, hotelRoom.definition.capacity);
                    for (var i = 0; i < guestCount; i++)
                    {
                        var occupant = worldController.buildingsController.CreateOccupantAtBuildingEntrance(hotelRoom.building);
                        occupant.SetSchedule(OccupantScheduleType.HotelGuest);
                        occupant.SetHotelRoom(hotelRoom);
                        occupant.TransitionToTask(new OccupantTravelingToDestinationTask(occupant, routeFinder, route));
                        (hotelRoom.behavior as HotelRoomBehavior).AddGuest();
                        hotelGuests.Add(occupant);
                    }
                }

                hasCheckedIn = true;
            }
            else if (currentDayTime.IsGreaterThanOrEqualTo(checkoutTime) && !hasCheckedOut)
            {
                // TODO - this will check all hotel guests out at once
                foreach (var hotelGuest in hotelGuests)
                {
                    hotelGuest.TransitionToTask(new OccupantLeavingBuildingTask(hotelGuest));
                }

                hasCheckedOut = true;
            }
            else if (currentDayTime.Matches(HotelRoomBehavior.cashoutTime))
            {
                var totalProfit = hotelRooms.Aggregate(0, (acc, hotelRoom) =>
                {
                    var behavior = hotelRoom.behavior as HotelRoomBehavior;
                    var roomTotalProfit = behavior.GetTotalProfit();

                    // reset room here to avoid a second loop
                    behavior.ResetGuests();

                    return acc + roomTotalProfit;
                });

                if (totalProfit > 0)
                {
                    worldController.walletController.AddFunds(totalProfit);
                    worldController.notifications.Add(new Notification($"Earned {Money.Format(totalProfit)} from hotel"));
                }
            }
        }

        void ResetDay()
        {
            hasCheckedIn = false;
            hasCheckedOut = false;
            checkinTime = HotelRoomBehavior.GetRandomCheckinTime();
            checkoutTime = HotelRoomBehavior.GetRandomCheckoutTime();
        }
    }
}