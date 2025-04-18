namespace MainPage {
    /**
     * The struct used for map view function.
     * Displays: the name of the map, how many ghostrons are there in total
     */
    public struct MapInfo {
        public string Name;
        public int GhostronNum;
        public char Difficulty;

        public MapInfo(string name, int ghostronNum, char difficulty) {
            Name = name;
            GhostronNum = ghostronNum;
            Difficulty = difficulty;
        }
    }
}