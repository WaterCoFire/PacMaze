using System.Collections.Generic;
using Entity.Ghostron;
using Entity.Map;
using Entity.Prop;
using UnityEngine;
using Random = System.Random;

namespace PlayMap {
    /**
     * Responsible for prop initialisation when game starts.
     */
    public class PropGenerator : MonoBehaviour {
        // All prop model prefabs (except for Ghostrons)
        public GameObject pacboyPrefab;
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

        // Pacboy game object (used for setting the chase target of all the Ghostrons)
        private GameObject _pacboy;

        /**
         * Places all the FIXED props (including Pacboy, fixed Ghostrons) on the map.
         * Randomly places all the RANDOM props (including Ghostrons) on the map.
         * Places dots on all the remaining tiles.
         */
        public void InitProps(PropData propData) {
            _propData = propData;

            // Initialise the list storing all the free tiles
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

            // Resets all the infos in Ghostron and dot manager
            GhostronManager.Instance.ResetGhostrons();
            DotManager.Instance.ResetDots();

            // Place all FIXED props on the map
            foreach (var kvp in _propData.PropOnTiles) {
                GameObject prefab = GetGameObjectByPropType(kvp.Value);

                if (kvp.Value == PropType.Pacboy) {
                    // The iterated prop type is Pacboy
                    // Store it
                    _pacboy = Instantiate(prefab, kvp.Key, Quaternion.identity);
                } else if (kvp.Value == PropType.Ghostron) {
                    // The iterated prop type is Ghostron
                    // Check how many Ghostrons are there already, use GhostronFactory to get the new Ghostron
                    GameObject newGhostronPrefab;
                    int currentGhostronCount = PlayerPrefs.GetInt("GhostronCount", -1);
                    switch (currentGhostronCount) {
                        case 0:
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron(GhostronType.Red);
                            break;
                        case 1:
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron(GhostronType.Blue);
                            break;
                        case 2:
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron(GhostronType.Yellow);
                            break;
                        case 3:
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron(GhostronType.Pink);
                            break;
                        case 4:
                            newGhostronPrefab = GhostronFactory.Instance.GetGhostron(GhostronType.Green);
                            break;
                        default:
                            Debug.LogError("Invalid Ghostron Count: " + PlayerPrefs.GetInt("GhostronCount"));
                            return;
                    }

                    // Instantiate the new Ghostron
                    GameObject newGhostron = Instantiate(newGhostronPrefab, kvp.Key, Quaternion.identity);
                    
                    // Update the current number of Ghostrons
                    PlayerPrefs.SetInt("GhostronCount", currentGhostronCount + 1);

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
            if (!PlaceRandomProps(PropType.Ghostron))
                Debug.LogError("Error occurred when setting random Ghostrons");
            if (!PlaceRandomProps(PropType.PowerPellet))
                Debug.LogError("Error occurred when setting random power pellets");
            if (!PlaceRandomProps(PropType.FastWheel))
                Debug.LogError("Error occurred when setting random fast wheels");
            if (!PlaceRandomProps(PropType.NiceBomb))
                Debug.LogError("Error occurred when setting random nice bombs");
            if (!PlaceRandomProps(PropType.SlowWheel))
                Debug.LogError("Error occurred when setting random slow wheels");
            if (!PlaceRandomProps(PropType.BadCherry))
                Debug.LogError("Error occurred when setting random bad cherries");
            if (!PlaceRandomProps(PropType.LuckyDice))
                Debug.LogError("Error occurred when setting random lucky dices");

            // Place dots at all the remaining free tile
            foreach (var freeTile in _freeTiles) {
                GameObject newDot = Instantiate(dotPrefab, freeTile, Quaternion.identity);
                DotManager.Instance.AddDot(newDot); // Add to the dot manager
            }

            // Set the Pacboy target for all the Ghostrons
            if (_pacboy == null) {
                Debug.LogError("Error: Pacboy not found!");
            } else {
                PlayMapController.Instance.SetPacboy(_pacboy);
                GhostronManager.Instance.SetPacboy(_pacboy);
            }

            // Reset the free tiles list
            _freeTiles.Clear();
        }

        private bool PlaceRandomProps(PropType propType) {
            GameObject propObject;
            // Get the corresponding prefab first
            switch (propType) {
                case PropType.Ghostron:
                    // Ghostrons will be spawned using GhostronFactory
                    // Since there are different types of Ghostrons
                    propObject = null;
                    break;
                case PropType.PowerPellet:
                    propObject = powerPelletPrefab;
                    break;
                case PropType.FastWheel:
                    propObject = fastWheelPrefab;
                    break;
                case PropType.NiceBomb:
                    propObject = niceBombPrefab;
                    break;
                case PropType.SlowWheel:
                    propObject = slowWheelPrefab;
                    break;
                case PropType.BadCherry:
                    propObject = badCherryPrefab;
                    break;
                case PropType.LuckyDice:
                    propObject = luckyDicePrefab;
                    break;
                default:
                    Debug.LogError("Invalid prop name when setting random props: " + propType);
                    return false;
            }

            // Get the count of the random ones required for this type of prop
            int randomCount = _propData.TotalPropCounts[propType] - _propData.FixedPropCounts[propType];
            int freeTilesNum;

            // Generate prop logic
            for (int i = 0; i < randomCount; i++) {
                freeTilesNum = _freeTiles.Count;
                int randomIndex = _random.Next(0, freeTilesNum); // Random number

                if (propType != PropType.Ghostron) {
                    // If the prop type is not Ghostron, directly instantiate it
                    Instantiate(propObject, _freeTiles[randomIndex], Quaternion.identity);
                } else {
                    // If the prop type is Ghostron
                    // Check how many Ghostrons are there already, use GhostronFactory to get the new Ghostron
                    int currentGhostronCount = PlayerPrefs.GetInt("GhostronCount", -1);
                    switch (currentGhostronCount) {
                        case 0:
                            propObject = GhostronFactory.Instance.GetGhostron(GhostronType.Red);
                            break;
                        case 1:
                            propObject = GhostronFactory.Instance.GetGhostron(GhostronType.Blue);
                            break;
                        case 2:
                            propObject = GhostronFactory.Instance.GetGhostron(GhostronType.Yellow);
                            break;
                        case 3:
                            propObject = GhostronFactory.Instance.GetGhostron(GhostronType.Pink);
                            break;
                        case 4:
                            propObject = GhostronFactory.Instance.GetGhostron(GhostronType.Green);
                            break;
                        default:
                            Debug.LogError("Invalid Ghostron Count: " + PlayerPrefs.GetInt("GhostronCount"));
                            return false;
                    }

                    // Instantiate it and store it to the GhostronManager
                    GameObject newGhostron = Instantiate(propObject, _freeTiles[randomIndex], Quaternion.identity);
                    
                    // Update the current number of Ghostrons
                    PlayerPrefs.SetInt("GhostronCount", currentGhostronCount + 1);

                    // Add the new Ghostron to GhostronManager
                    GhostronManager.Instance.AddGhostron(newGhostron);
                }

                _freeTiles.RemoveAt(randomIndex); // Remove the random chosen tile from free tiles list
            }

            return true;
        }
        
        /**
         * Obtains the corresponding GameObject by PropType.
         * (Pacboy/Ghostron are not dealt here)
         */
        private GameObject GetGameObjectByPropType(PropType propType) {
            return propType switch {
                PropType.Pacboy => pacboyPrefab,
                PropType.PowerPellet => powerPelletPrefab,
                PropType.FastWheel => fastWheelPrefab,
                PropType.NiceBomb => niceBombPrefab,
                PropType.SlowWheel => slowWheelPrefab,
                PropType.BadCherry => badCherryPrefab,
                PropType.LuckyDice => luckyDicePrefab,
                _ => null
            };
        }
    }
}