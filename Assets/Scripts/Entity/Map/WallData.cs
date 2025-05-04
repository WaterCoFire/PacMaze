namespace Entity.Map {
    public struct WallData {
        public readonly bool[,] HorizontalWallStatus;
        public readonly bool[,] VerticalWallStatus;
        
        public WallData(bool[,] horizontal, bool[,] vertical) {
            HorizontalWallStatus = horizontal;
            VerticalWallStatus = vertical;
        }
    }
}