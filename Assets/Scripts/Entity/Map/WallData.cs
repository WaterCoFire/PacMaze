namespace Entity.Map {
    public struct WallData {
        public bool[,] HorizontalWallStatus;
        public bool[,] VerticalWallStatus;
        
        public WallData(bool[,] horizontal, bool[,] vertical) {
            HorizontalWallStatus = horizontal;
            VerticalWallStatus = vertical;
        }
    }
}