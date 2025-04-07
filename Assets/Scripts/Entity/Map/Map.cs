using System;
using UnityEngine;

namespace Entity.Map {
    public class Map {
        // Basic information
        private String _name;
        private DateTime _lastPlayedDateTime;
        private Time _fastestTime;
        
        // Information from map editor
        private PropData _propData;
        private WallData _wallData;
    }
}