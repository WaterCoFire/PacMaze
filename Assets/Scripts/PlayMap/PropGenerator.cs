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

        // Pacman game object (used for setting the chase target of all the ghosts)
        private GameObject _pacmanGameObject;
        
        // DotManager component
        // As it is a frequently used component, declare it here could save some time
        public DotManager dotManager;

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
            
            // Resets all the infos in ghost and dot manager
            gameObject.GetComponent<GhostManager>().ResetGhosts();
            gameObject.GetComponent<DotManager>().ResetDots();

            // Place all FIXED props on the map
            foreach (var kvp in _propData.PropOnTiles) {
                GameObject prefab = kvp.Value;

                if (prefab == null) {
                    Debug.LogError($"Error: Invalid prop type: {prefab}");
                    return;
                }

                if (prefab.name == "Pacman") {
                    // Look for the pacman game object and store it
                    Debug.Log("PACMAN FOUND");
                    _pacmanGameObject = Instantiate(prefab, kvp.Key, Quaternion.identity);
                } else if (prefab.name.Contains("Ghost")) {
                    // Store all the ghosts in GhostManager
                    GameObject newGhost = Instantiate(prefab, kvp.Key, Quaternion.identity);
                    // Debug.Log(newGhost.name);
                    gameObject.GetComponent<GhostManager>().AddGhost(newGhost);
                } else {
                    // All other props
                    Instantiate(prefab, kvp.Key, Quaternion.identity);
                }

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

            Debug.Log("111");
            // Place dots at all the remaining free tile
            foreach (var freeTile in _freeTiles) {
                GameObject newDot = Instantiate(dotPrefab, freeTile, Quaternion.identity);
                dotManager.AddDot(newDot); // Add to the dot manager
            }
            
            Debug.Log("222");
            // Set the pacman target for all the ghosts
            if (_pacmanGameObject == null) {
                Debug.LogError("Error: Pacman not found!");
            } else {
                gameObject.GetComponent<GhostManager>().SetPacman(_pacmanGameObject);
            }

            // Reset the free tiles list
            _freeTiles.Clear();
        }

        private bool PlaceRandomProps(string propName) {
            GameObject propObject;
            // Get the corresponding prefab first
            switch (propName) {
                case "GhostSpawn":
                    propObject = null;
                    // Ghost spawn prop will be dealt with later for there are different variants
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

                if (propName != "GhostSpawn") {
                    Instantiate(propObject, _freeTiles[randomIndex], Quaternion.identity);
                } else {
                    GameObject newGhost;
                    switch (PlayerPrefs.GetInt("GhostsCount", -1)) {
                        case 0:
                            PlayerPrefs.SetInt("GhostsCount", 1);
                            newGhost = Instantiate(redGhostPrefab, _freeTiles[randomIndex], Quaternion.identity);
                            break;
                        case 1:
                            PlayerPrefs.SetInt("GhostsCount", 2);
                            newGhost = Instantiate(blueGhostPrefab, _freeTiles[randomIndex], Quaternion.identity);
                            break;
                        case 2:
                            PlayerPrefs.SetInt("GhostsCount", 3);
                            newGhost = Instantiate(yellowGhostPrefab, _freeTiles[randomIndex], Quaternion.identity);
                            break;
                        case 3:
                            PlayerPrefs.SetInt("GhostsCount", 4);
                            newGhost = Instantiate(greenGhostPrefab, _freeTiles[randomIndex], Quaternion.identity);
                            break;
                        case 4:
                            PlayerPrefs.SetInt("GhostsCount", 5);
                            newGhost = Instantiate(pinkGhostPrefab, _freeTiles[randomIndex], Quaternion.identity);
                            break;
                        default:
                            Debug.LogError("Invalid ghosts count when instantiating randomly: " +
                                           PlayerPrefs.GetInt("GhostsCount"));
                            return false;
                    }

                    // Add the new ghost to GhostManager
                    gameObject.GetComponent<GhostManager>().AddGhost(newGhost);
                }

                _freeTiles.RemoveAt(randomIndex); // Remove the random chosen tile from free tiles list
            }

            return true;
        }
    }
}