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
        public void OnMouseUp()
        {
            worldController.buildingsController.RemoveFrontmostRoomAtCurrentTile();
        }

        public void Teardown()
        {
            var room = worldController.buildingsController.FindFrontmostRoomAtTile(worldController.hoveredTile.current);
            room?.SetMarkedForDeletionState(false);
        }

        public void Setup()
        {
            // TODO - have a local state for this like the inspect tool
            var room = worldController.buildingsController.FindFrontmostRoomAtTile(worldController.hoveredTile.current);
            room?.SetMarkedForDeletionState(true);
        }

        public void OnUpdate()
        {
            var hoveredTile = worldController.hoveredTile;

            if (hoveredTile.HasChanged())
            {
                if (hoveredTile.prev != null)
                {
                    // un-mark for deletion previous room
                    var room = worldController.buildingsController.FindFrontmostRoomAtTile(hoveredTile.prev);
                    room?.SetMarkedForDeletionState(false);
                }

                if (hoveredTile.current != null)
                {
                    var room = worldController.buildingsController.FindFrontmostRoomAtTile(hoveredTile.current);
                    room?.SetMarkedForDeletionState(true);
                }
            }
        }
    }
}