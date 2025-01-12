namespace TowerBuilder
{
    public interface IRoomValidator
    {
        // public string description { get; }
        public RoomValidationError Validate(Room room, WorldController worldController);
    }
}