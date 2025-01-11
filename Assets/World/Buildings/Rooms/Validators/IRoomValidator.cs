namespace TowerBuilder
{
    public interface IRoomValidator
    {
        public RoomValidationError Validate(WorldController worldController);
    }
}