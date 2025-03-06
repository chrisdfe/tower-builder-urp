using System.Collections.Generic;
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

        // public static float OCCUPANT_Z_OFFSET = 0.1f;
        public static float OCCUPANT_Z_OFFSET = -1f;

        public string title { get; private set; } = "Occupant";
        public Tile tile { get; private set; }

        // where this occupant works
        public Room office { get; private set; }

        // where this occupant lives
        public Room residence { get; private set; }

        // where this occupant currently is
        public Room currentRoom { get; private set; }

        // the hotel room this occupant is staying in (if they are here as a hotel guest)
        public Room hotelRoom { get; private set; }

        // player is hovering over the room with the inspect tool
        public bool isInspectionHovered { get; private set; } = false;

        // this is the room that is being inspected with the inspect tool
        public bool isInspected { get; private set; } = false;

        // Positioning
        public SubtileOffset subTileOffset = new SubtileOffset();

        public OccupantAnimationWrapper animationWrapper { get; private set; }

        // Schedules/Tasks
        public OccupantScheduleBase schedule { get; private set; }
        public OccupantTaskBase currentTask { get; private set; }
        public Queue<OccupantTaskBase> taskQueue { get; private set; } = new();

        // GameWorld stuff references
        Transform bod;
        Material bodMaterial;

        //
        // Lifecycle
        //
        void Awake()
        {
            animationWrapper = OccupantAnimationWrapper.FindFor(this);
            bod = animationWrapper.transform.Find("Bod");
            bodMaterial = bod.GetComponent<MeshRenderer>().material;
        }

        public void FixedUpdate()
        {
            if (isInspected)
            {
                if (currentTask != null && currentTask is OccupantTravelingToDestinationTask)
                {
                    (currentTask as OccupantTravelingToDestinationTask).routeFinder.DebugDrawPaths();
                }
            }
        }

        public void OnTick()
        {
            schedule.OnTick();

            currentTask.OnTick();

            if (currentTask.isComplete)
            {
                OccupantTaskBase nextTask;
                taskQueue.TryDequeue(out nextTask);

                if (nextTask != null)
                {
                    TransitionToTask(nextTask);
                }
                else
                {
                    TransitionToTask(null);
                }
            }
        }

        //
        // Public interface
        //
        public void SetTitle(string title)
        {
            this.title = title;
            gameObject.name = title;
        }

        public void SetSchedule(OccupantScheduleType scheduleType)
        {
            schedule = scheduleType switch
            {
                OccupantScheduleType.Resident => new OccupantResidentSchedule(this),
                OccupantScheduleType.HotelGuest => new OccupantHotelGuestSchedule(this),
                _ => throw new System.NotImplementedException($"Unsupported schedule type: {scheduleType}"),
            };
        }

        public void SetCurrentRoom(Room room)
        {
            currentRoom = room;

            if (currentRoom != null)
            {
                SetInteriorLightColor(currentRoom.GetInteriorLightsColor());
            }
            else
            {
                SetInteriorLightColor(Color.black);
            }
        }

        public void SetResidence(Room room)
        {
            residence = room;
            schedule.Regenerate();
        }

        public void SetOffice(Room room)
        {
            office = room;
            schedule.Regenerate();
        }

        public void SetHotelRoom(Room room)
        {
            hotelRoom = room;
            schedule.Regenerate();
        }

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

        public void TransitionToTask(OccupantTaskBase task)
        {
            currentTask?.Teardown();

            currentTask = task;

            currentTask?.Setup();
        }

        public void SendToTile(Tile destinationTile)
        {
            TransitionToTask(new OccupantTravelingToDestinationTask(this, destinationTile));
        }

        public void EnqueueTask(OccupantTaskBase task)
        {
            taskQueue.Enqueue(task);
        }

        public void CancelCurrentTask()
        {
            currentTask.Cancel();
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
            if (isInspected)
            {
                SetHighlightIntensity(1f);
            }
            else if (isInspectionHovered)
            {
                SetHighlightIntensity(0.5f);
            }
            else
            {
                SetHighlightIntensity(0f);
            }
        }

        void SetHighlightIntensity(float intensity)
        {
            bodMaterial.SetFloat("_HighlightIntensity", intensity);
        }

        void SetInteriorLightColor(Color color)
        {
            bodMaterial.SetColor("_InteriorLightColor", color);
        }

        //
        // Static interface
        // 
        static SubtileOffset GetRandomSubtileOffset()
        {
            var x = Random.Range(-0.2f, 0.2f);
            var z = Random.Range(0f, 0.4f);
            return new SubtileOffset(x, z);
        }
    }
}