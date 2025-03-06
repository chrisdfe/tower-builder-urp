using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class BuildingRoomVacancyManager
    {
        Building building;

        WorldController worldController;

        public BuildingRoomVacancyManager(Building building)
        {
            this.building = building;
            worldController = WorldController.Get();
        }

        public void OnTick()
        {
            // check for vacancies every hour
            if (worldController.timeController.timeValue.minute == 0)
            {
                HandleResidentialVacancies(building);
                HandleOfficeVacancies(building);
            }
        }

        void HandleResidentialVacancies(Building building)
        {
            var entrance = building.GetEntrance();
            Assert.IsNotNull(entrance);

            // TODO - re-use the same occupants for residences/offices
            // create occupants for rooms that have residence slots available
            var availableResidenceRooms = building.GetRoomsWithAvailableResidenceSlots();
            var newOccupants = new List<Occupant>();

            foreach (var room in availableResidenceRooms)
            {
                var entranceTile = entrance.GetRandomTile();
                var roomTile = room.GetRandomTile();
                var routeFinder = new OccupantRouteFinder(building, entranceTile, roomTile);
                var route = routeFinder.FindRoute();
                if (route == null)
                {
                    // No path found from entrance to residence
                    continue;
                }

                var vacanciesCount = room.GetResidentialVacancies();
                for (var i = 0; i < vacanciesCount; i++)
                {
                    var occupant = worldController.buildingsController.CreateOccupantAtBuildingEntrance(building);

                    occupant.SetupSchedule(OccupantScheduleType.Resident);

                    room.residents.Add(occupant);
                    occupant.SetResidence(room);

                    // Immediately travel to new home
                    // TODO - set this task to high priority so daily schedule tasks don't cancel it
                    occupant.TransitionToTask(new OccupantTravelingToDestinationTask(occupant, route));
                    newOccupants.Add(occupant);
                }
            }

            if (newOccupants.Count > 0)
            {
                worldController.notifications.Add(
                    new Notification($"{newOccupants.Count} occupants have moved into {building.title}")
                );
            }
        }

        void HandleOfficeVacancies(Building building)
        {
            // give occupants jobs if they are unemployed and there are workplaces available
            var unemployedOccupants = building.GetUnemployedOccupants();

            if (unemployedOccupants.Count > 0)
            {
                var availableWorkerRooms = building.GetRoomsWithAvailableWorkerSlots();

                foreach (var room in availableWorkerRooms)
                {
                    foreach (var occupant in unemployedOccupants)
                    {
                        occupant.SetOffice(room);
                        room.workers.Add(occupant);
                        worldController.notifications.Add(new Notification(occupant.title + " has been assigned work at " + room.title));
                    }
                }
            }
        }
    }
}