namespace HomePage {
    /**
     * The struct used for map view function.
     * Displays: the name of the map, how many ghostrons are there in total
     */
    public struct MapInfo {
        public readonly string Name;
        public readonly int GhostronNum;
        public readonly char Difficulty;

        public MapInfo(string name, int ghostronNum, char difficulty) {
            Name = name;
            GhostronNum = ghostronNum;
            Difficulty = difficulty;
        }
    }
}