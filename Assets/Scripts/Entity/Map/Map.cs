using System;
using UnityEditor.UI;
using UnityEngine;

namespace Entity.Map {
    public class Map {
        // Basic information
        public String Name;
        public char Difficulty;

        // Information from map editor
        public PropData PropData;
        public WallData WallData;

        public Map(string name, char difficulty, WallData wallData, PropData propData) {
            Name = name;
            Difficulty = difficulty;
            WallData = wallData;
            PropData = propData;
        }
    }
}