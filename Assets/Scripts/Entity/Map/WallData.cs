namespace Entity.Map {
    /**
     * Struct for storing the wall status data of a map.
     */
    public struct WallData {
        // All horizontal walls status and all vertical walls status
        public readonly bool[,] HorizontalWallStatus;
        public readonly bool[,] VerticalWallStatus;
        
        // Constructor
        public WallData(bool[,] horizontalWallStatus, bool[,] verticalWallStatus) {
            HorizontalWallStatus = horizontalWallStatus;
            VerticalWallStatus = verticalWallStatus;
        }
    }
}