using System.Collections.Generic;
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
        // TODO 有问题
        public Dictionary<Vector3, GameObject> PropPositions() {
            var dict = new Dictionary<Vector3, GameObject>();
            for (int i = 0; i < propPositions.Count; i++) {
                Vector3 pos = propPositions[i];
                string type = propTypes[i];
                dict[pos] = Resources.Load<GameObject>("Props/" + type); // 道具需放入 Resources/Props 文件夹
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
    }
}