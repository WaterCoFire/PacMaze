using System;
using UnityEditor.UI;
using UnityEngine;

namespace Entity.Map {
    public class Map {
        // Basic information
        public String Name;
        public DateTime LastPlayedDateTime;
        
        // public Time FastestTime;

        public bool Played;

        // Information from map editor
        public PropData PropData;
        public WallData WallData;

        public Map(string name, WallData wallData, PropData propData) {
            Name = name;
            WallData = wallData;
            PropData = propData;

            Played = false;
            LastPlayedDateTime = DateTime.Now;
        }
    }
}