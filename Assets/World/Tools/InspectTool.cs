using UnityEngine;

namespace TowerBuilder
{
    public class InspectTool : ITool
    {
        public Room inspectedRoom { get; private set; }
        public IInspectTarget hoveredInspectTarget { get; private set; }
        public IInspectTarget inspectTarget { get; private set; }

        WorldController worldController;

        // TODO - probably should be in worldController
        int worldEntityLayerMask;

        public InspectTool(WorldController worldController)
        {
            this.worldController = worldController;

            // TODO - Probably should go somewhere else, since this is game-wide
            int worldEntityMaskLayerIndex = LayerMask.NameToLayer("World Entity");
            worldEntityLayerMask = 1 << worldEntityMaskLayerIndex;
        }

        //
        // Lifecycle/handlers
        //
        public void OnMouseUp()
        {
            //
            // inspect current hovered target
            if (hoveredInspectTarget != null)
            {
                // uninspect current inspected target, (if it exists)
                inspectTarget?.SetInspectedState(false);

                // inspect hovered inspect target
                inspectTarget = hoveredInspectTarget;

                inspectTarget.SetInspectionHoveredState(false);
                inspectTarget.SetInspectedState(true);
            }
        }

        public void Teardown()
        {
            // teardown inspectedHoveredTarget
            hoveredInspectTarget?.SetInspectionHoveredState(false);
            inspectTarget?.SetInspectedState(false);
        }

        public void Setup()
        {
            CalculateInspectHoverTarget();
        }

        public void OnUpdate()
        {
            CalculateInspectHoverTarget();
        }

        //
        // Private interface
        //
        void CalculateInspectHoverTarget()
        {
            // 
            if (worldController.cursorIsOverUI.current) return;

            // Determine what "world entity" is being hovered over currently
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, worldEntityLayerMask))
            {
                var tag = hit.transform.tag;

                // TODO - see if I can clean any of this up now that ITool exists
                switch (tag)
                {
                    case "Resident":
                        {
                            var resident = hit.transform.GetComponent<Resident>();

                            // Don't do anything if this resident is the current inspect hover target
                            if (
                                !(hoveredInspectTarget is Resident && (hoveredInspectTarget as Resident) == resident)
                            )
                            {
                                TeardownHoveredInspectTarget();

                                hoveredInspectTarget = resident;
                                resident.SetInspectionHoveredState(true);
                            }

                            break;
                        }
                    case "RoomTile":
                        {
                            var roomTile = hit.transform.GetComponent<RoomTile>();
                            var room = roomTile.room;

                            // Blueprint rooms aren't inspectable
                            if (room.isBlueprint) break;

                            // Don't do anything if this room is currently being hovered over
                            if (
                                !(hoveredInspectTarget is Room && (hoveredInspectTarget as Room) == room)
                            )
                            {
                                TeardownHoveredInspectTarget();

                                hoveredInspectTarget = room;
                                room.SetInspectionHoveredState(true);
                            }

                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                // nothing is being hovered over - unset hoveredInspectTarget if it is not null
                TeardownHoveredInspectTarget();
            }
        }

        void TeardownHoveredInspectTarget()
        {
            //
            hoveredInspectTarget?.SetInspectionHoveredState(false);
            hoveredInspectTarget = null;
        }
    }
}