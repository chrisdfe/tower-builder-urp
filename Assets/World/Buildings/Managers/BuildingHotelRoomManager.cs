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
        // TODO - use an enum instead of hasCheckedIn/hasCheckedOut
        DayTimeValue checkinTime;
        DayTimeValue checkoutTime;
        int lastDayCheckedIn = -1;
        int lastDayCheckedOut = -1;

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

            var currentTime = worldController.timeController.timeValue;
            var currentDayTime = currentTime.ToDayTimeValue();

            if (currentDayTime.Matches(DayTimeValue.midnight))
            {
                ResetDay();
            }

            if (currentDayTime.IsGreaterThanOrEqualTo(checkinTime) && currentTime.day > lastDayCheckedIn)
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
                        occupant.SetHotelGuestData(new()
                        {
                            hotelRoom = hotelRoom,
                            // TODO - randomize this number somewhere
                            checkinTime = worldController.timeController.timeValue
                        });
                        hotelRoom.hotelGuests.Add(occupant);

                        // Go straight to hotel room
                        occupant.StartImmediateTask(new OccupantTravelingToDestinationTask(occupant, routeFinder, route));
                        (hotelRoom.behavior as HotelRoomBehavior).AddGuest();
                        hotelGuests.Add(occupant);
                    }
                }

                Debug.Log($"total hotel guests checked in: {hotelGuests.Count}");
                lastDayCheckedIn = currentTime.day;
            }
            else if (currentDayTime.IsGreaterThanOrEqualTo(checkoutTime) && currentTime.day > lastDayCheckedOut)
            {
                // TODO - this will check all hotel guests out at once
                foreach (var hotelGuest in hotelGuests)
                {
                    hotelGuest.hotelGuestData.hotelRoom.hotelGuests.Remove(hotelGuest);
                    hotelGuest.StartImmediateTask(new OccupantLeavingBuildingTask(hotelGuest));
                }

                lastDayCheckedOut = currentTime.day;
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
                    worldController.notificationsController.AddNotification(new Notification($"Earned {Money.Format(totalProfit)} from hotel"));
                }
            }
        }

        void ResetDay()
        {
            checkinTime = HotelRoomBehavior.GetRandomCheckinTime();
            checkoutTime = HotelRoomBehavior.GetRandomCheckoutTime();
        }
    }
}