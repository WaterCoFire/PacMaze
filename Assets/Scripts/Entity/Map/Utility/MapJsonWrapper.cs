using System;
using System.Collections.Generic;
using Entity.Prop;
using UnityEngine;

namespace Entity.Map.Utility {
    /**
     * Wrapper for Map data.
     * Used when serialising and de-serialising map data.
     */
    [Serializable]
    public class MapJsonWrapper {
        public string name;
        public DifficultyType difficulty;
        public bool eventEnabled;

        public List<Vector3> propPositions;
        public List<PropType> propTypes;
        public List<PropType> fixedPropKeys;
        public List<int> fixedPropValues;
        public List<PropType> totalPropKeys;
        public List<int> totalPropValues;

        public List<bool> horizontalWallStatus;
        public List<bool> verticalWallStatus;

        public int highScore;

        public MapJsonWrapper() { }

        public MapJsonWrapper(Map map) {
            name = map.Name;
            difficulty = map.Difficulty;
            eventEnabled = map.EventEnabled;
            highScore = map.HighScore;

            propPositions = new List<Vector3>();
            propTypes = new List<PropType>();
            foreach (var kvp in map.PropData.PropOnTiles) {
                // Add prop position and type information to Json
                propPositions.Add(kvp.Key);
                propTypes.Add(kvp.Value); // Use PropType to identify its type
            }

            fixedPropKeys = new List<PropType>(map.PropData.FixedPropCounts.Keys);
            fixedPropValues = new List<int>(map.PropData.FixedPropCounts.Values);
            totalPropKeys = new List<PropType>(map.PropData.TotalPropCounts.Keys);
            totalPropValues = new List<int>(map.PropData.TotalPropCounts.Values);

            horizontalWallStatus = new List<bool>();
            for (int i = 0; i < 10; i++) {
                foreach (var horizontalWall in map.WallData.HorizontalWallStatus) {
                    horizontalWallStatus.Add(horizontalWall);
                }
            }
            
            verticalWallStatus = new List<bool>();
            for (int i = 0; i < 11; i++) {
                foreach (var verticalWall in map.WallData.VerticalWallStatus) {
                    verticalWallStatus.Add(verticalWall);
                }
            }
        }

        // Used to convert to the format required by PropData when deserializing
        // Check the player preferences to judge if the prefabs for editor or the prefabs for map play should be obtained
        public Dictionary<Vector3, PropType> PropPositions() {
            var dict = new Dictionary<Vector3, PropType>();
            switch (PlayerPrefs.GetString("GameObjectReadMode", "NOT DEFINED")) {
                case "EDITOR":
                    // The game object prefabs to be obtained should be the ones for the editor
                    for (int i = 0; i < propPositions.Count; i++) {
                        Vector3 pos = propPositions[i];
                        PropType type = propTypes[i];
                        dict[pos] = type;
                    }

                    return dict;
                case "PLAY":
                    // The game object prefabs to be obtained should be the ones for map play
                    for (int i = 0; i < propPositions.Count; i++) {
                        Vector3 pos = propPositions[i];
                        PropType type = propTypes[i];
                        dict[pos] = type;
                    }

                    return dict;
                default:
                    Debug.LogError("Invalid game object read mode: " + PlayerPrefs.GetString("GameObjectReadMode"));
                    return null;
            }
        }

        public Dictionary<PropType, int> FixedPropCounts {
            get {
                var dict = new Dictionary<PropType, int>();
                for (int i = 0; i < fixedPropKeys.Count; i++) {
                    dict[fixedPropKeys[i]] = fixedPropValues[i];
                }

                return dict;
            }
        }

        public Dictionary<PropType, int> TotalPropCounts {
            get {
                var dict = new Dictionary<PropType, int>();
                for (int i = 0; i < totalPropKeys.Count; i++) {
                    dict[totalPropKeys[i]] = totalPropValues[i];
                }

                return dict;
            }
        }

        public bool[,] HorizontalWallStatus {
            get {
                bool[,] result = new bool[10, 11];
                for (int column = 0; column < 11; column++) {
                    result[0, column] = horizontalWallStatus[column];
                    result[1, column] = horizontalWallStatus[column + 11];
                    result[2, column] = horizontalWallStatus[column + 22];
                    result[3, column] = horizontalWallStatus[column + 33];
                    result[4, column] = horizontalWallStatus[column + 44];
                    result[5, column] = horizontalWallStatus[column + 55];
                    result[6, column] = horizontalWallStatus[column + 66];
                    result[7, column] = horizontalWallStatus[column + 77];
                    result[8, column] = horizontalWallStatus[column + 88];
                    result[9, column] = horizontalWallStatus[column + 99];
                }

                return result;
            }
        }

        public bool[,] VerticalWallStatus {
            get {
                bool[,] result = new bool[11, 10];
                for (int column = 0; column < 10; column++) {
                    result[0, column] = verticalWallStatus[column];
                    result[1, column] = verticalWallStatus[column + 10];
                    result[2, column] = verticalWallStatus[column + 20];
                    result[3, column] = verticalWallStatus[column + 30];
                    result[4, column] = verticalWallStatus[column + 40];
                    result[5, column] = verticalWallStatus[column + 50];
                    result[6, column] = verticalWallStatus[column + 60];
                    result[7, column] = verticalWallStatus[column + 70];
                    result[8, column] = verticalWallStatus[column + 80];
                    result[9, column] = verticalWallStatus[column + 90];
                    result[10, column] = verticalWallStatus[column + 100];
                }

                return result;
            }
        }

        // For wall data: convert all the elements in the given row of the bool[,] array to List<bool>
        private List<bool> ConvertBoolArrayToList(bool[,] array, int row) {
            int cols = array.GetLength(1);

            List<bool> list = new List<bool>(cols);
            for (int i = 0; i < cols; i++) {
                list.Add(array[row, i]);
            }

            return list;
        }
    }
}