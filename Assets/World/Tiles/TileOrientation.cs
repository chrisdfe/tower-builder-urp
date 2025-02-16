using System;

namespace TowerBuilder
{
    // Cell position in relation to another cell
    [Flags]
    public enum TileOrientation
    {
        None = 0,
        Above = 1,
        AboveRight = 2,
        Right = 4,
        BelowRight = 8,
        Below = 16,
        BelowLeft = 32,
        Left = 64,
        AboveLeft = 128
    }
}