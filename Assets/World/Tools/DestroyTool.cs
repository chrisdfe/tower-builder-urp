namespace TowerBuilder
{
    public class DestroyTool : ITool
    {
        WorldController worldController;

        public DestroyTool(WorldController worldController)
        {
            this.worldController = worldController;
        }

        //
        // Lifecycle/handlers
        //
        public void OnLeftMouseUp()
        {
            worldController.buildingsController.RemoveFrontmostRoomAtCurrentTile();
        }

        public void Setup()
        {
            // TODO - have a local state for this like the inspect tool
            var room = worldController.buildingsController.FindRoomAtTile(worldController.hoveredTile.current);
            room?.SetMarkedForDeletionState(true);
        }

        public void Teardown()
        {
            var room = worldController.buildingsController.FindRoomAtTile(worldController.hoveredTile.current);
            room?.SetMarkedForDeletionState(false);
        }

        public void OnUpdate()
        {
            var hoveredTile = worldController.hoveredTile;

            if (hoveredTile.HasChanged())
            {
                if (hoveredTile.prev != null)
                {
                    // un-mark for deletion previous room
                    var room = worldController.buildingsController.FindRoomAtTile(hoveredTile.prev);
                    room?.SetMarkedForDeletionState(false);
                }

                if (hoveredTile.current != null)
                {
                    var room = worldController.buildingsController.FindRoomAtTile(hoveredTile.current);
                    room?.SetMarkedForDeletionState(true);
                }
            }
        }
    }
}