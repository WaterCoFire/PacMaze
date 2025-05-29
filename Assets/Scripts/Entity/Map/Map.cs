namespace Entity.Map {
    public class Map {
        // Basic information
        public readonly string Name;
        public readonly DifficultyType Difficulty;
        public readonly bool EventEnabled;

        // Information from map editor
        public PropData PropData;
        public WallData WallData;

        public Map(string name, DifficultyType difficulty, bool eventEnabled, WallData wallData, PropData propData) {
            Name = name;
            Difficulty = difficulty;
            EventEnabled = eventEnabled;
            WallData = wallData;
            PropData = propData;
        }
    }
}