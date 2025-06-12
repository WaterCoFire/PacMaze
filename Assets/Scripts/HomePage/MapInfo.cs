using Entity.Map;

namespace HomePage {
    /**
     * The struct used for map view function.
     * Displays: the name of the map, how many Ghostrons are there in total, map difficulty
     */
    public struct MapInfo {
        public readonly string Name;
        public readonly int GhostronNum;
        public readonly DifficultyType Difficulty;

        // Constructor
        public MapInfo(string name, int ghostronNum, DifficultyType difficulty) {
            Name = name;
            GhostronNum = ghostronNum;
            Difficulty = difficulty;
        }
    }
}