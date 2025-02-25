using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace TowerBuilder
{
    public class Occupant : MonoBehaviour, IInspectTarget
    {
        public class SubtileOffset
        {
            public float x = 0f;
            public float z = 0f;

            public SubtileOffset() { }

            public SubtileOffset(float x, float z)
            {
                this.x = x;
                this.z = z;
            }
        }

        public static float OCCUPANT_Z_OFFSET = 0.1f;

        public string title { get; set; } = "Occupant";
        public Tile tile { get; private set; }

        // where this occupant works
        public Room office { get; set; }

        // where this occupant lives
        public Room residence { get; set; }

        // where this occupant currently is
        public Room currentRoom { get; set; }

        // player is hovering over the room with the inspect tool
        public bool isInspectionHovered { get; private set; } = false;

        // this is the room that is being inspected with the inspect tool
        public bool isInspected { get; private set; } = false;

        // Positioning
        public SubtileOffset subTileOffset = new SubtileOffset();

        Color originalColor;
        public Transform movementAnimationWrapper { get; private set; }
        Transform bod;

        public IOccupantTask currentTask { get; private set; } = new OccupantIdleTask();
        OccupantRouteFinder routeFinder;

        //
        // Lifecycle
        //
        void Awake()
        {
            movementAnimationWrapper = transform.Find("MovementAnimationWrapper");
            bod = movementAnimationWrapper.Find("Bod");
            var bodMaterial = bod.GetComponent<MeshRenderer>().material;
            originalColor = bodMaterial.color;
        }

        public void OnTick()
        {
            currentTask.OnTick();

            if (currentTask.isComplete)
            {
                // TODO - next task in queue
                //        for now, just transition to idle
                TransitionToTask(new OccupantIdleTask());
            }
        }

        public void FixedUpdate()
        {
            if (isInspected)
            {
                if (routeFinder != null)
                {
                    routeFinder.DebugDrawPaths();
                }
            }
        }

        //
        // Public interface
        //
        public void SetInspectionHoveredState(bool isInspectionHovered)
        {
            this.isInspectionHovered = isInspectionHovered;
            UpdateColor();
        }

        public void SetInspectedState(bool isInspected)
        {
            this.isInspected = isInspected;
            UpdateColor();
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;
            UpdatePosition();
        }

        public void SetSubTileOffset(SubtileOffset subTileOffset)
        {
            this.subTileOffset = subTileOffset;
            UpdatePosition();
        }

        public void SetRandomSubTileOffset()
        {
            SetSubTileOffset(GetRandomSubtileOffset());
        }

        public Vector2 GetInspectFocalPoint()
        {
            return new Vector2(
                bod.transform.position.x,
                bod.transform.position.y
            );
        }

        public void TransitionToTask(IOccupantTask task)
        {
            if (currentTask != null)
            {
                currentTask.Teardown();

                // TODO - I probably don't need to keep the routefinder around for now
                if (currentTask is OccupantTravelingToDestinationTask)
                {
                    routeFinder = null;
                }
            }

            currentTask = task;
            currentTask.Setup();
        }

        public void StartTravelToDestinationTask(Building building, Tile destinationTile)
        {
            routeFinder = new OccupantRouteFinder(building, tile, destinationTile);
            var route = routeFinder.FindRoute();

            if (route != null)
            {
                TransitionToTask(new OccupantTravelingToDestinationTask(this, routeFinder.route));
            }
        }

        //
        // Private interface
        //
        void UpdatePosition()
        {
            var tilePosition = tile.ToWorldPosition();

            transform.position = new Vector3(
                tilePosition.x + subTileOffset.x,
                tilePosition.y,
                -OCCUPANT_Z_OFFSET + subTileOffset.z
            );
        }

        void UpdateColor()
        {
            Color color;
            if (isInspected)
            {
                color = RoomConstants.ROOM_INSPECTED_COLOR;
            }
            else if (isInspectionHovered)
            {
                color = RoomConstants.ROOM_INSPECTION_HOVERED_COLOR;
            }
            else
            {
                color = originalColor;
            }

            bod.GetComponent<MeshRenderer>().material.color = color;
        }

        //
        // Static interface
        // 
        static SubtileOffset GetRandomSubtileOffset()
        {
            var x = Random.Range(-0.7f, 0.7f);
            var z = Random.Range(0f, 0.4f);
            return new SubtileOffset(x, z);
        }
    }
}