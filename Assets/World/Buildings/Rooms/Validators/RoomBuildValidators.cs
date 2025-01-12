using UnityEngine;

namespace TowerBuilder
{
    public static class RoomBuildValidators
    {
        public static readonly IRoomValidator hasEnoughMoney = new HasEnoughMoney();
        public static readonly IRoomValidator isNotOverlappingAnotherRoom = new IsNotOverlappingAnotherRoom();
        public static readonly IRoomValidator isTouchingAnotherRoom = new IsTouchingAnotherRoom();

        public static readonly IRoomValidator[] standardBuildValidators = new IRoomValidator[] {
            hasEnoughMoney,
            isNotOverlappingAnotherRoom,
            isTouchingAnotherRoom
        };

        class HasEnoughMoney : IRoomValidator
        {
            public RoomValidationError Validate(Room room, WorldController worldController)
            {
                // TODO
                return null;
            }
        }

        class IsNotOverlappingAnotherRoom : IRoomValidator
        {
            static RoomValidationError error = new RoomValidationError("can't overlap other rooms");

            public RoomValidationError Validate(Room room, WorldController worldController)
            {
                if (worldController.buildingsController.ContainsRoomsAtTiles(room.tiles, room.definition.layer))
                {
                    return error;
                }

                // TODO
                return null;
            }
        }

        class IsTouchingAnotherRoom : IRoomValidator
        {
            static RoomValidationError error = new RoomValidationError("must be touching another room");

            public RoomValidationError Validate(Room room, WorldController worldController)
            {
                var roomAdjacentTiles = room.GetAdjacentTiles();

                if (!worldController.buildingsController.ContainsRoomsAtTiles(roomAdjacentTiles, room.definition.layer))
                {
                    // 
                    return error;
                }

                // TODO
                return null;
            }
        }
    }
}