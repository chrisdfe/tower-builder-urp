namespace TowerBuilder
{
    // Cell position in relation to a Tile
    public enum TilePosition
    {
        None,

        // Single (isolated)
        Single,

        // middle walls
        Top,
        Right,
        Bottom,
        Left,

        // Centers
        Center,
        HorizontalCenter,
        VerticalCenter,
        LowToHighDiagonalCenter,
        HighToLowDiagonalCenter,

        // corners
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,

        // Isolated cells
        TopIsolated,
        RightIsolated,
        BottomIsolated,
        LeftIsolated,

        TopRightIsolated,
        BottomRightIsolated,
        BottomLeftIsolated,
        TopLeftIsolated,
    }
}

