
namespace TowerBuilder
{
    public class InspectTarget
    {
        // Nothing here yet. but at some point there probably will be.
    }

    public class ResidentInspectTarget : InspectTarget
    {
        public Resident resident;

        public ResidentInspectTarget(Resident resident)
        {
            this.resident = resident;
        }
    }

    public class RoomInspectTarget : InspectTarget
    {
        public Room room;

        public RoomInspectTarget(Room room)
        {
            this.room = room;
        }
    }

    public class BuildingInspectTarget : InspectTarget
    {
        public Building building;

        public BuildingInspectTarget(Building building)
        {
            this.building = building;
        }
    }
}
