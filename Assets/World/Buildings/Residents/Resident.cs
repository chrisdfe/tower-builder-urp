using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class Resident : MonoBehaviour
    {
        public static float RESIDENT_Z_OFFSET = 2f;

        public Tile tile { get; private set; }
        public Room office { get; set; }
        public Room residence { get; set; }

        // Positioning
        public float subTileOffset = 0f;

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

        void UpdatePosition()
        {
            var tilePosition = tile.ToWorldPosition();
            transform.position = new Vector3(
                tilePosition.x + subTileOffset,
                tilePosition.y,
                -RESIDENT_Z_OFFSET
            );
        }
    }
}