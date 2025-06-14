﻿namespace Entity.Map {
    /**
     * Map class
     * Information stored:
     * - Name
     * - Difficulty
     * - Random event status
     * - Data about props
     * - Data about walls
     * - High score
     */
    public class Map {
        // Basic information
        public readonly string Name;
        public readonly DifficultyType Difficulty;
        public readonly bool EventEnabled;

        // Information from map editor
        public PropData PropData;
        public WallData WallData;

        // High score information
        public readonly int HighScore;

        /**
         * Constructor, used when creating a new map.
         */
        public Map(string name, DifficultyType difficulty, bool eventEnabled, WallData wallData, PropData propData) {
            Name = name;
            Difficulty = difficulty;
            EventEnabled = eventEnabled;
            WallData = wallData;
            PropData = propData;

            HighScore = 0; // the high score is 0 when a new map is created
        }
    }
}