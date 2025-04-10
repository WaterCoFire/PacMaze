namespace MainPage {
    /**
     * The struct used for map view function.
     * Displays: the name of the map, how many ghosts are there in total
     */
    public struct MapInfo {
        public string Name;
        public int GhostNum;

        public MapInfo(string name, int ghostNum) {
            Name = name;
            GhostNum = ghostNum;
        }
    }
}