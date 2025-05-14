using System.Collections.Generic;
using Entity.Map;
using UnityEngine;
using Random = System.Random;

namespace PlayMap {
    public class PropGenerator : MonoBehaviour {
        // All prop models (except for Ghostrons and Pacboy)
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
        private readonly List<Vector3> _freeTiles = new();

        // Random instance for generating random numbers
        private readonly Random _random = new();

        // Pacboy game object (used for setting the chase target of all the ghostrons)
        private GameObject _pacboy;

        /**
         * Places all the FIXED props (including Pacboy, fixed ghostrons) on the map.
         * Randomly places all the RANDOM props (including ghostrons) on the map.
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

            // Resets all the infos in ghostron and dot manager
            GhostronManager.Instance.ResetGhostrons();
            DotManager.Instance.ResetDots();

            // Place all FIXED props on the map
            foreach (var kvp in _propData.PropOnTiles) {
                GameObject prefab = kvp.Value;

                if (prefab == null) {
                    Debug.LogError($"Error: Invalid prop type: {prefab}");
                    return;
                }

                if (prefab.name == "Pacboy") {
                    // The iterated prop type is Pacboy
                    // Store it
                    _pacboy = Instantiate(prefab, kvp.Key, Quaternion.identity);
                } else if (prefab.name == "GhostronEmpty") {
                    // The iterated prop type is Ghostron
                    // Check how many Ghostrons are there already, use GhostronFactory to get the new Ghostron
                    GameObject newGhostronPrefab;
                    switch (PlayerPrefs.GetInt("GhostronCount", -1)) {
                        case 0:
                            PlayerPrefs.SetInt("GhostronCount", 1);
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron("Red");
                            break;
                        case 1:
                            PlayerPrefs.SetInt("GhostronCount", 2);
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron("Blue");
                            break;
                        case 2:
                            PlayerPrefs.SetInt("GhostronCount", 3);
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron("Yellow");
                            break;
                        case 3:
                            PlayerPrefs.SetInt("GhostronCount", 4);
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron("Pink");
                            break;
                        case 4:
                            PlayerPrefs.SetInt("GhostronCount", 5);
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron("Green");
                            break;
                        default:
                            Debug.LogError("Invalid Ghostron Count: " + PlayerPrefs.GetInt("GhostronCount"));
                            return;
                    }

                    // Instantiate the new Ghostron
                    GameObject newGhostron = Instantiate(newGhostronPrefab, kvp.Key, Quaternion.identity);

                    // Store the new Ghostron in GhostronManager
                    GhostronManager.Instance.AddGhostron(newGhostron);
                } else {
                    // All other props
                    Instantiate(prefab, kvp.Key, Quaternion.identity);
                }

                // Remove the tile from the free tiles list
                _freeTiles.Remove(kvp.Key);
            }

            // Place the RANDOM props on the map
            if (!PlaceRandomProps("GhostronSpawn"))
                Debug.LogError("Error occurred when setting random ghostrons");
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
                GameObject newDot = Instantiate(dotPrefab, freeTile, Quaternion.identity);
                DotManager.Instance.AddDot(newDot); // Add to the dot manager
            }

            // Set the Pacboy target for all the ghostrons
            if (_pacboy == null) {
                Debug.LogError("Error: Pacboy not found!");
            } else {
                PlayMapController.Instance.SetPacboy(_pacboy);
                GhostronManager.Instance.SetPacboy(_pacboy);
            }

            // Reset the free tiles list
            _freeTiles.Clear();
        }

        private bool PlaceRandomProps(string propName) {
            GameObject propObject;
            // Get the corresponding prefab first
            switch (propName) {
                case "GhostronSpawn":
                    // Ghostrons will be spawned using GhostronFactory
                    // Since there are different types of Ghostrons
                    propObject = null;
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

                if (propName != "GhostronSpawn") {
                    // If the prop type is not Ghostron, directly instantiate it
                    Instantiate(propObject, _freeTiles[randomIndex], Quaternion.identity);
                } else {
                    // If the prop type is Ghostron
                    // Check how many Ghostrons are there already, use GhostronFactory to get the new Ghostron
                    switch (PlayerPrefs.GetInt("GhostronCount", -1)) {
                        case 0:
                            PlayerPrefs.SetInt("GhostronCount", 1);
                            propObject = GhostronFactory.Instance.GetGhostron("Red");
                            break;
                        case 1:
                            PlayerPrefs.SetInt("GhostronCount", 2);
                            propObject = GhostronFactory.Instance.GetGhostron("Blue");
                            break;
                        case 2:
                            PlayerPrefs.SetInt("GhostronCount", 3);
                            propObject = GhostronFactory.Instance.GetGhostron("Yellow");
                            break;
                        case 3:
                            PlayerPrefs.SetInt("GhostronCount", 4);
                            propObject = GhostronFactory.Instance.GetGhostron("Pink");
                            break;
                        case 4:
                            PlayerPrefs.SetInt("GhostronCount", 5);
                            propObject = GhostronFactory.Instance.GetGhostron("Green");
                            break;
                        default:
                            Debug.LogError("Invalid Ghostron Count: " + PlayerPrefs.GetInt("GhostronCount"));
                            return false;
                    }
                    
                    // Instantiate it and store it to the GhostronManager
                    GameObject newGhostron = Instantiate(propObject, _freeTiles[randomIndex], Quaternion.identity);

                    // Add the new ghostron to GhostronManager
                    GhostronManager.Instance.AddGhostron(newGhostron);
                }

                _freeTiles.RemoveAt(randomIndex); // Remove the random chosen tile from free tiles list
            }

            return true;
        }
    }
}