using System.Collections.Generic;
using System.IO;
using MapEditor;
using UnityEngine;

namespace Entity.Map {
    [System.Serializable]
    public class MapJsonWrapper {
        public string name;
        public char difficulty;

        public List<Vector3> propPositions;
        public List<string> propTypes;
        public List<string> fixedPropKeys;
        public List<int> fixedPropValues;
        public List<string> totalPropKeys;
        public List<int> totalPropValues;

        public List<bool> hwStatusRow1;
        public List<bool> hwStatusRow2;
        public List<bool> hwStatusRow3;
        public List<bool> hwStatusRow4;
        public List<bool> hwStatusRow5;
        public List<bool> hwStatusRow6;
        public List<bool> hwStatusRow7;
        public List<bool> hwStatusRow8;
        public List<bool> hwStatusRow9;
        public List<bool> hwStatusRow10;

        public List<bool> vwStatusRow1;
        public List<bool> vwStatusRow2;
        public List<bool> vwStatusRow3;
        public List<bool> vwStatusRow4;
        public List<bool> vwStatusRow5;
        public List<bool> vwStatusRow6;
        public List<bool> vwStatusRow7;
        public List<bool> vwStatusRow8;
        public List<bool> vwStatusRow9;
        public List<bool> vwStatusRow10;
        public List<bool> vwStatusRow11;

        public MapJsonWrapper() { }

        public MapJsonWrapper(Map map) {
            name = map.Name;
            difficulty = map.Difficulty;

            propPositions = new List<Vector3>();
            propTypes = new List<string>();
            foreach (var kvp in map.PropData.PropOnTiles) {
                // Add prop position and type information to Json
                if (kvp.Value != null) {
                    propPositions.Add(kvp.Key);
                    propTypes.Add(kvp.Value.name); // Use the name of the prop to identify its type
                }
            }

            fixedPropKeys = new List<string>(map.PropData.FixedPropCounts.Keys);
            fixedPropValues = new List<int>(map.PropData.FixedPropCounts.Values);
            totalPropKeys = new List<string>(map.PropData.TotalPropCounts.Keys);
            totalPropValues = new List<int>(map.PropData.TotalPropCounts.Values);

            hwStatusRow1 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 0));
            hwStatusRow2 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 1));
            hwStatusRow3 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 2));
            hwStatusRow4 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 3));
            hwStatusRow5 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 4));
            hwStatusRow6 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 5));
            hwStatusRow7 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 6));
            hwStatusRow8 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 7));
            hwStatusRow9 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 8));
            hwStatusRow10 = new List<bool>(ConvertBoolArrayToList(map.WallData.HorizontalWallStatus, 9));

            vwStatusRow1 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 0));
            vwStatusRow2 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 1));
            vwStatusRow3 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 2));
            vwStatusRow4 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 3));
            vwStatusRow5 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 4));
            vwStatusRow6 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 5));
            vwStatusRow7 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 6));
            vwStatusRow8 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 7));
            vwStatusRow9 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 8));
            vwStatusRow10 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 9));
            vwStatusRow11 = new List<bool>(ConvertBoolArrayToList(map.WallData.VerticalWallStatus, 10));
        }

        // Used to convert to the format required by PropData when deserializing
        // Check the player preferences to judge if the prefabs for editor or the prefabs for map play should be obtained
        public Dictionary<Vector3, GameObject> PropPositions() {
            var dict = new Dictionary<Vector3, GameObject>();
            switch (PlayerPrefs.GetString("GameObjectReadMode", "NOT DEFINED")) {
                case "EDITOR":
                    // The game object prefabs to be obtained should be the ones for the editor
                    for (int i = 0; i < propPositions.Count; i++) {
                        Vector3 pos = propPositions[i];
                        string type = propTypes[i];
                        dict[pos] = GetCorrespondingEditorGameObject(type);
                    }

                    return dict;
                case "PLAY":
                    // The game object prefabs to be obtained should be the ones for map play
                    for (int i = 0; i < propPositions.Count; i++) {
                        Vector3 pos = propPositions[i];
                        string type = propTypes[i];
                        dict[pos] = GetCorrespondingPlayGameObject(type);
                    }

                    return dict;
                default:
                    Debug.LogError("Invalid game object read mode: " + PlayerPrefs.GetString("GameObjectReadMode"));
                    return null;
            }
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

        public bool[,] HorizontalWallStatus {
            get {
                bool[,] result = new bool[10, 11];
                for (int column = 0; column < 11; column++) {
                    result[0, column] = hwStatusRow1[column];
                    result[1, column] = hwStatusRow2[column];
                    result[2, column] = hwStatusRow3[column];
                    result[3, column] = hwStatusRow4[column];
                    result[4, column] = hwStatusRow5[column];
                    result[5, column] = hwStatusRow6[column];
                    result[6, column] = hwStatusRow7[column];
                    result[7, column] = hwStatusRow8[column];
                    result[8, column] = hwStatusRow9[column];
                    result[9, column] = hwStatusRow10[column];
                }

                return result;
            }
        }

        public bool[,] VerticalWallStatus {
            get {
                bool[,] result = new bool[11, 10];
                for (int column = 0; column < 10; column++) {
                    result[0, column] = vwStatusRow1[column];
                    result[1, column] = vwStatusRow2[column];
                    result[2, column] = vwStatusRow3[column];
                    result[3, column] = vwStatusRow4[column];
                    result[4, column] = vwStatusRow5[column];
                    result[5, column] = vwStatusRow6[column];
                    result[6, column] = vwStatusRow7[column];
                    result[7, column] = vwStatusRow8[column];
                    result[8, column] = vwStatusRow9[column];
                    result[9, column] = vwStatusRow10[column];
                    result[10, column] = vwStatusRow11[column];
                }

                return result;
            }
        }

        /**
         * Obtains the corresponding game object based on the prop type given.
         * The prefabs are used for map editor.
         */
        private GameObject GetCorrespondingEditorGameObject(string propName) {
            switch (CleanName(propName)) {
                case "PacmanSpawn":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/Spawn/PacmanSpawn");
                case "GhostronSpawn":
                    return Resources.Load<GameObject>("Prefabs/Props/Editor/Spawn/GhostronSpawn");
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

        /**
         * Obtains the corresponding game object based on the prop type given.
         * The prefabs are used for the map to be played.
         */
        private GameObject GetCorrespondingPlayGameObject(string propName) {
            switch (CleanName(propName)) {
                case "PacmanSpawn":
                    return Resources.Load<GameObject>("Prefabs/Props/Game/Spawn/Pacman");
                case "GhostronSpawn":
                    switch (PlayerPrefs.GetInt("GhostronCount", -1)) {
                        case 0:
                            PlayerPrefs.SetInt("GhostronCount", 1);
                            return Resources.Load<GameObject>("Prefabs/Props/Game/Spawn/RedGhostron");
                        case 1:
                            PlayerPrefs.SetInt("GhostronCount", 2);
                            return Resources.Load<GameObject>("Prefabs/Props/Game/Spawn/BlueGhostron");
                        case 2:
                            PlayerPrefs.SetInt("GhostronCount", 3);
                            return Resources.Load<GameObject>("Prefabs/Props/Game/Spawn/YellowGhostron");
                        case 3:
                            PlayerPrefs.SetInt("GhostronCount", 4);
                            return Resources.Load<GameObject>("Prefabs/Props/Game/Spawn/GreenGhostron");
                        case 4:
                            PlayerPrefs.SetInt("GhostronCount", 5);
                            return Resources.Load<GameObject>("Prefabs/Props/Game/Spawn/PinkGhostron");
                        default:
                            Debug.LogError("Invalid Ghostron Count: " + PlayerPrefs.GetInt("GhostronCount"));
                            return null;
                    }
                case "PowerPellet":
                    return Resources.Load<GameObject>("Prefabs/Props/Game/PowerPellet");
                case "FastWheel":
                    return Resources.Load<GameObject>("Prefabs/Props/Game/FastWheel");
                case "NiceBomb":
                    return Resources.Load<GameObject>("Prefabs/Props/Game/NiceBomb");
                case "SlowWheel":
                    return Resources.Load<GameObject>("Prefabs/Props/Game/SlowWheel");
                case "BadCherry":
                    return Resources.Load<GameObject>("Prefabs/Props/Game/BadCherry");
                case "LuckyDice":
                    return Resources.Load<GameObject>("Prefabs/Props/Game/LuckyDice");
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