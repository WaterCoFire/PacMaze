using System.Collections.Generic;
using System.IO;
using MapEditor;
using UnityEngine;

namespace Entity.Map {
    [System.Serializable]
    public class MapJsonWrapper {
        public string name;
        public bool played;
        public string lastPlayedDateTime;
        public string fastestTime;

        public List<Vector3> propPositions;
        public List<string> propTypes;
        public List<string> fixedPropKeys;
        public List<int> fixedPropValues;
        public List<string> totalPropKeys;
        public List<int> totalPropValues;

        public bool[,] HorizontalWallStatus = new bool[10, 11];
        public bool[,] VerticalWallStatus = new bool[11, 10];

        public MapJsonWrapper(Map map) {
            name = map.Name;
            played = map.Played;
            lastPlayedDateTime = map.LastPlayedDateTime.ToString();
            // fastestTime = map.FastestTime.ToString();

            propPositions = new List<Vector3>();
            propTypes = new List<string>();
            foreach (var kvp in map.PropData.PropOnTile) {
                propPositions.Add(kvp.Key);
                propTypes.Add(kvp.Value.name); // Use the name of the prop to identify its type
            }

            fixedPropKeys = new List<string>(map.PropData.FixedPropCounts.Keys);
            fixedPropValues = new List<int>(map.PropData.FixedPropCounts.Values);
            totalPropKeys = new List<string>(map.PropData.TotalPropCounts.Keys);
            totalPropValues = new List<int>(map.PropData.TotalPropCounts.Values);

            HorizontalWallStatus = map.WallData.HorizontalWallStatus;
            VerticalWallStatus = map.WallData.VerticalWallStatus;
        }

        // Used to convert to the format required by PropData when deserializing
        public Dictionary<Vector3, GameObject> PropPositions() {
            var dict = new Dictionary<Vector3, GameObject>();
            for (int i = 0; i < propPositions.Count; i++) {
                Vector3 pos = propPositions[i];
                string type = propTypes[i];
                dict[pos] = GetCorrespondingGameObject(type);
            }

            return dict;
        }

        public Dictionary<string, int> FixedPropCounts {
            get {
                var dict = new Dictionary<string, int>();
                for (int i = 0; i < fixedPropKeys.Count; i++) {
                    dict[fixedPropKeys[i]] = fixedPropValues[i];
                }

                return dict;
            }
        }

        public Dictionary<string, int> TotalPropCounts {
            get {
                var dict = new Dictionary<string, int>();
                for (int i = 0; i < totalPropKeys.Count; i++) {
                    dict[totalPropKeys[i]] = totalPropValues[i];
                }

                return dict;
            }
        }

        /**
         * Obtains the corresponding game object based on the prop type given.
         */
        // TODO 优化
        private GameObject GetCorrespondingGameObject(string propName) {
            switch (CleanName(propName)) {
                case "PacmanSpawn":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/Spawn/PacmanSpawn");
                case "GhostSpawn":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/Spawn/GhostSpawn");
                case "PowerPellet":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/PowerPellet");
                case "FastWheel":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/FastWheel");
                case "NiceBomb":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/NiceBomb");
                case "SlowWheel":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/SlowWheel");
                case "BadCherry":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/BadCherry");
                case "LuckyDice":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/LuckyDice");
                default:
                    Debug.LogError("Get corresponding game object error: " + CleanName(propName));
                    return null;
            }
        }

        // Remove the "(Clone)" at the end of the game object Name if it exists
        private string CleanName(string nameToBeCleaned) {
            const string cloneTag = "(Clone)";
            return nameToBeCleaned.EndsWith(cloneTag)
                ? nameToBeCleaned.Remove(nameToBeCleaned.Length - 7)
                : nameToBeCleaned;
        }
    }
}