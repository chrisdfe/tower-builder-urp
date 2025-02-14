using UnityEngine;

namespace TowerBuilder
{
    public class InspectTool : ITool
    {
        public IInspectTarget hoveredInspectTarget { get; private set; }
        public IInspectTarget inspectTarget { get; private set; }

        WorldController worldController;

        public delegate void InspectToolEvent();
        public InspectToolEvent onInspectTargetUpdated;

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
            // inspect current hovered target
            if (hoveredInspectTarget != null)
            {
                SetInspectTarget(hoveredInspectTarget);
            }
        }

        public void Setup()
        {
            CalculateInspectHoverTarget();
        }

        public void Teardown()
        {
            // teardown inspectedHoveredTarget
            hoveredInspectTarget?.SetInspectionHoveredState(false);
            inspectTarget?.SetInspectedState(false);

            SetInspectTarget(null);
        }

        public void OnUpdate()
        {
            CalculateInspectHoverTarget();
        }

        public void SetHoveredInspectTarget(IInspectTarget hoveredInspectTarget)
        {
            // Don't do anything if this is alredy the current inspect hover target
            if (this.hoveredInspectTarget == hoveredInspectTarget)
            {
                return;
            }

            // teardown current hovered inspect target
            if (this.hoveredInspectTarget != null)
            {
                this.hoveredInspectTarget?.SetInspectionHoveredState(false);
                this.hoveredInspectTarget = null;
            }

            // Setup new hovered inspect target
            this.hoveredInspectTarget = hoveredInspectTarget;
            this.hoveredInspectTarget?.SetInspectionHoveredState(true);
        }

        public void SetInspectTarget(IInspectTarget inspectTarget)
        {
            if (this.inspectTarget == inspectTarget) return;

            // teardown current inspect target
            if (this.inspectTarget != null)
            {
                this.inspectTarget.SetInspectedState(false);
            }

            // setup new inspect target
            this.inspectTarget = inspectTarget;

            if (inspectTarget != null)
            {
                inspectTarget.SetInspectionHoveredState(false);
                inspectTarget.SetInspectedState(true);
            }

            onInspectTargetUpdated?.Invoke();
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

                            SetHoveredInspectTarget(resident);

                            break;
                        }
                    case "RoomTile":
                        {
                            var roomTile = hit.transform.GetComponent<RoomTile>();
                            var room = roomTile.room;

                            // Blueprint rooms aren't inspectable
                            if (room.isBlueprint) break;

                            SetHoveredInspectTarget(room);
                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                // nothing is being hovered over - unset hoveredInspectTarget if it is not null
                SetHoveredInspectTarget(null);
            }
        }
    }
}