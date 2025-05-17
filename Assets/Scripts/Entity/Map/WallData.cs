namespace Entity.Map {
    public struct WallData {
        public readonly bool[,] HorizontalWallStatus;
        public readonly bool[,] VerticalWallStatus;
        
        public WallData(bool[,] horizontalWallStatus, bool[,] verticalWallStatus) {
            HorizontalWallStatus = horizontalWallStatus;
            VerticalWallStatus = verticalWallStatus;
        }
    }
}