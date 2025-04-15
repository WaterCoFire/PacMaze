using System.Collections.Generic;
using Entity.Map;
using UnityEngine;
using Random = System.Random;

namespace PlayMap {
    public class PropGenerator : MonoBehaviour {
        // All prop models (except for pacman)
        public GameObject redGhostPrefab;
        public GameObject blueGhostPrefab;
        public GameObject yellowGhostPrefab;
        public GameObject greenGhostPrefab;
        public GameObject pinkGhostPrefab;
        public GameObject powerPelletPrefab;
        public GameObject fastWheelPrefab;
        public GameObject niceBombPrefab;
        public GameObject slowWheelPrefab;
        public GameObject badCherryPrefab;
        public GameObject luckyDicePrefab;
        public GameObject dotPrefab;

        private readonly float _gridSpacing = 3.0f; // The length of each tile
        private PropData _propData; // For storing the prop data

        // Used for storing all the free tiles
        private List<Vector3> _freeTiles = new();

        // Random instance for generating random numbers
        private Random _random = new();

        /**
         * Places all the FIXED props (including pacman, fixed ghosts) on the map.
         * Randomly places all the RANDOM props (including ghosts) on the map.
         * Places dots on all the remaining tiles.
         */
        public void InitProps(PropData propData) {
            _propData = propData;

            // Initialize the list storing all the free tiles
            // (should be all the tiles at the beginning)
            _freeTiles.Clear();
            float z, x;
            for (int i = 0; i < 11; i++) {
                z = 15f - i * _gridSpacing;
                for (int j = 0; j < 11; j++) {
                    x = -15f + j * _gridSpacing;
                    Vector3 vector3 = new Vector3(x, 0f, z);
                    _freeTiles.Add(vector3);
                }
            }

            // Place all FIXED props on the map
            PlayerPrefs.SetInt("GhostsCount", 0);
            foreach (var kvp in _propData.PropOnTiles) {
                GameObject prefab = kvp.Value;

                if (prefab == null) {
                    Debug.LogError($"Error: Invalid prop type: {prefab}");
                    return;
                }

                Instantiate(prefab, kvp.Key, Quaternion.identity);

                // Remove the tile from the free tiles list
                _freeTiles.Remove(kvp.Key);
            }

            // Place the RANDOM props on the map
            if (!PlaceRandomProps("GhostSpawn"))
                Debug.LogError("Error occurred when setting random ghosts");
            if (!PlaceRandomProps("PowerPellet"))
                Debug.LogError("Error occurred when setting random power pellets");
            if (!PlaceRandomProps("FastWheel"))
                Debug.LogError("Error occurred when setting random fast wheels");
            if (!PlaceRandomProps("NiceBomb"))
                Debug.LogError("Error occurred when setting random nice bombs");
            if (!PlaceRandomProps("SlowWheel"))
                Debug.LogError("Error occurred when setting random slow wheels");
            if (!PlaceRandomProps("BadCherry"))
                Debug.LogError("Error occurred when setting random bad cherries");
            if (!PlaceRandomProps("LuckyDice"))
                Debug.LogError("Error occurred when setting random lucky dices");
            
            // Place dots at all the remaining free tile
            foreach (var freeTile in _freeTiles) {
                Instantiate(dotPrefab, freeTile, Quaternion.identity);
            }
            
            _freeTiles.Clear();
        }

        private bool PlaceRandomProps(string propName) {
            GameObject propObject;
            // Get the corresponding prefab first
            switch (propName) {
                case "GhostSpawn":
                    switch (PlayerPrefs.GetInt("GhostsCount", -1)) {
                        case 0:
                            PlayerPrefs.SetInt("GhostsCount", 1);
                            propObject = redGhostPrefab;
                            break;
                        case 1:
                            PlayerPrefs.SetInt("GhostsCount", 2);
                            propObject = blueGhostPrefab;
                            break;
                        case 2:
                            PlayerPrefs.SetInt("GhostsCount", 3);
                            propObject = yellowGhostPrefab;
                            break;
                        case 3:
                            PlayerPrefs.SetInt("GhostsCount", 4);
                            propObject = greenGhostPrefab;
                            break;
                        case 4:
                            PlayerPrefs.SetInt("GhostsCount", 5);
                            propObject = pinkGhostPrefab;
                            break;
                        default:
                            Debug.LogError("Invalid Ghosts Count: " + PlayerPrefs.GetInt("GhostsCount"));
                            return false;
                    }

                    break;
                case "PowerPellet":
                    propObject = powerPelletPrefab;
                    break;
                case "FastWheel":
                    propObject = fastWheelPrefab;
                    break;
                case "NiceBomb":
                    propObject = niceBombPrefab;
                    break;
                case "SlowWheel":
                    propObject = slowWheelPrefab;
                    break;
                case "BadCherry":
                    propObject = badCherryPrefab;
                    break;
                case "LuckyDice":
                    propObject = luckyDicePrefab;
                    break;
                default:
                    Debug.LogError("Invalid prop name when setting random props: " + propName);
                    return false;
            }

            // Get the count of the random ones required for this type of prop
            int randomCount = _propData.TotalPropCounts[propName] - _propData.FixedPropCounts[propName];
            int freeTilesNum;

            // Generate prop logic
            for (int i = 0; i < randomCount; i++) {
                freeTilesNum = _freeTiles.Count;
                int randomIndex = _random.Next(0, freeTilesNum); // Random number

                Instantiate(propObject, _freeTiles[randomIndex], Quaternion.identity);
                _freeTiles.RemoveAt(randomIndex); // Remove the random chosen tile from free tiles list
            }

            return true;
        }
    }
}