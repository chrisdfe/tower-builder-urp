namespace TowerBuilder
{
    public static class RoomBuildValidators
    {
        public class HasEnoughMoney : IRoomValidator
        {
            public RoomValidationError Validate(WorldController worldController)
            {
                // TODO
                return null;
            }
        }

        public class IsNotOverlappingOtherRoom : IRoomValidator
        {
            public RoomValidationError Validate(WorldController worldController)
            {
                // TODO
                return null;
            }
        }

        public class IsTouchingAnotherRoom : IRoomValidator
        {
            public RoomValidationError Validate(WorldController worldController)
            {
                // TODO
                return null;
            }
        }
    }
}