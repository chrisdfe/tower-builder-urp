using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class Occupant : MonoBehaviour, IInspectTarget
    {
        public static float OCCUPANT_Z_OFFSET = 0.1f;

        public string title { get; set; } = "Occupant";
        public Tile tile { get; private set; }
        public Room office { get; set; }
        public Room residence { get; set; }

        // player is hovering over the room with the inspect tool
        public bool isInspectionHovered { get; private set; } = false;

        // this is the room that is being inspected with the inspect tool
        public bool isInspected { get; private set; } = false;

        // Positioning
        public float subTileOffset = 0f;

        Color originalColor;
        Transform bod;

        //
        // Lifecycle
        //
        void Awake()
        {
            bod = transform.Find("Bod");
            var bodMaterial = bod.GetComponent<MeshRenderer>().material;
            originalColor = bodMaterial.color;
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

        public void SetSubTileOffset(float subTileOffset)
        {
            this.subTileOffset = subTileOffset;
            UpdatePosition();
        }

        public Vector2 GetInspectFocalPoint()
        {
            return new Vector2(
                bod.transform.position.x,
                bod.transform.position.y
            );
        }

        //
        // Private interface
        //
        void UpdatePosition()
        {
            var tilePosition = tile.ToWorldPosition();

            transform.position = new Vector3(
                tilePosition.x + subTileOffset,
                tilePosition.y,
                -OCCUPANT_Z_OFFSET
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
    }
}