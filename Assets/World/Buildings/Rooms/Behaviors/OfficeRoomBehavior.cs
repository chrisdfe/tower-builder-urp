using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    public class OfficeRoomBehavior : RoomBehaviorBase
    {
        // Occupants currently working in this room
        public List<Occupant> workingOccupants { get; private set; } = new();

        // Gets added to wallet every day at 8pm
        public int currentAccumulatedProfit { get; private set; }

        static readonly DayTimeValue cashOutTime = new DayTimeValue(20, 0);

        public OfficeRoomBehavior(Room room) : base(room) { }

        public override void OnTick()
        {
            currentAccumulatedProfit += workingOccupants.Count * 10;
        }

        public override void OnTickAll(List<Room> allOffices)
        {
            if (worldController.timeController.timeValue.ToDayTimeValue().Matches(cashOutTime))
            {
                var totalAccumulatedProfit = allOffices
                    .Select(room => room.behavior as OfficeRoomBehavior)
                    .Aggregate(0, (acc, officeRoomBehavior) =>
                    {
                        acc += officeRoomBehavior.currentAccumulatedProfit;
                        // reset currentAccumulatedProfit here to avoid a second loop
                        officeRoomBehavior.currentAccumulatedProfit = 0;
                        return acc;
                    });

                if (totalAccumulatedProfit > 0)
                {
                    worldController.walletController.AddFunds(totalAccumulatedProfit);
                    worldController.notificationsController.AddNotification(new()
                    {
                        type = NotificationType.Success,
                        title = "Profit from offices",
                        message = $"Earned {Money.Format(totalAccumulatedProfit)} from offices"
                    });
                }
            }
        }
    }
}