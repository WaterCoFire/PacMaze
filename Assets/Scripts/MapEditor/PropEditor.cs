using System.Collections.Generic;
using Entity.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    /**
     * PacMaze Game Props:
     *
     * BASIC
     * - Ghost Spawn Point
     * - Pacman Spawn Point
     *
     * GOOD ONES
     * - Power Pellet: Like the classical Pacman Power Pellet
     * - Fast Wheel： Player becomes faster for 5 sec
     * - Nice Bomb: Press K to kill the nearest ghost
     *
     * BAD ONES
     * - Slow Wheel: Player slows down for 5 sec
     * - Bad Cherry: One more ghost (spawns randomly)
     *
     * RANDOM
     * - Lucky Dice: One of the five props
     */
    public class PropEditor : MonoBehaviour {
        private Vector3 _selectedTileVector3; // the selected tile's vector
        private bool _tileSelected; // if a tile is selected

        // Materials
        public Material tileNormalMaterial;
        public Material tileHighlightMaterial;

        private GameObject _lastSelectedTile; // Last selected tile (to change that to normal material)

        // All prop models
        public GameObject ghostSpawnPrefab;
        public GameObject pacmanSpawnPrefab;
        public GameObject powerPelletPrefab;
        public GameObject fastWheelPrefab;
        public GameObject niceBombPrefab;
        public GameObject slowWheelPrefab;
        public GameObject badCherryPrefab;
        public GameObject luckyDicePrefab;

        // Props buttons
        public Button ghostSpawnButton;
        public Button pacmanSpawnButton;
        public Button powerPelletButton;
        public Button fastWheelButton;
        public Button niceBombButton;
        public Button slowWheelButton;
        public Button badCherryButton;
        public Button luckyDiceButton;
        public Button removeButton;

        // Add/Minus buttons
        public Button ghostSpawnAdd;
        public Button ghostSpawnMinus;
        public Button powerPelletAdd;
        public Button powerPelletMinus;
        public Button fastWheelAdd;
        public Button fastWheelMinus;
        public Button niceBombAdd;
        public Button niceBombMinus;
        public Button slowWheelAdd;
        public Button slowWheelMinus;
        public Button badCherryAdd;
        public Button badCherryMinus;
        public Button luckyDiceAdd;
        public Button luckyDiceMinus;

        // Count texts
        public TMP_Text ghostCountText;
        public TMP_Text powerPelletCountText;
        public TMP_Text fastWheelCountText;
        public TMP_Text niceBombCountText;
        public TMP_Text slowWheelCountText;
        public TMP_Text badCherryCountText;
        public TMP_Text luckyDiceCountText;

        public GameObject propEditPanel; // Only active when a tile is selected
        public GameObject tileNotSelectedPrompt;

        public bool propMode;

        private Dictionary<Vector3, GameObject> _propOnTiles = new(); // Prop on every tile

        // FIXED counts of all the props
        private Dictionary<string, int> _fixedPropCounts = new() {
            { "PacmanSpawn", 0 },
            { "GhostSpawn", 0 },
            { "PowerPellet", 0 },
            { "FastWheel", 0 },
            { "NiceBomb", 0 },
            { "SlowWheel", 0 },
            { "BadCherry", 0 },
            { "LuckyDice", 0 }
        };

        // TOTAL counts of all the props - including FIXED and RANDOM ones
        private Dictionary<string, int> _totalPropCounts = new() {
            { "GhostSpawn", 2 },
            { "PowerPellet", 0 },
            { "FastWheel", 0 },
            { "NiceBomb", 0 },
            { "SlowWheel", 0 },
            { "BadCherry", 0 },
            { "LuckyDice", 0 }
        };

        private readonly Vector3 _gridStart = new(-15, 0, 15); // The top left corner
        private readonly float _gridSpacing = 3.0f; // The length of each tile
        private readonly int _gridSize = 11; // Map grid size

        /**
         * Sets the prop data. Used in MapEditor class.
         * Information to be set: propOnTiles, fixedPropCounts, totalPropCounts
         */
        public void SetPropData(PropData propData) {
            // Data update
            _propOnTiles = propData.PropOnTiles;
            _fixedPropCounts = propData.FixedPropCounts;
            _totalPropCounts = propData.TotalPropCounts;

            // Place props on the map
            foreach (var kvp in _propOnTiles) {
                GameObject prefab = kvp.Value;

                if (prefab == null) {
                    Debug.LogError($"WARNING Invalid prop type: {prefab}");
                    return;
                }

                Instantiate(prefab, kvp.Key, Quaternion.identity);
            }
            
            // UI update
            LoadPropTotalCount();
            TotalNumberButtonUpdate();
        }

        // Enters the prop editing mode. Used in MapEditor class.
        public void EnterPropMode() {
            propMode = true;
            _tileSelected = false;

            propEditPanel.SetActive(false);
            tileNotSelectedPrompt.SetActive(true);
            PropsButtonInit();
        }

        // Quits the prop editing mode. Used in MapEditor class.
        public void QuitPropMode() {
            propMode = false;

            // Change the material of the last tile back to the normal one
            if (_lastSelectedTile != null) {
                var renderer = _lastSelectedTile.GetComponent<Renderer>();
                if (renderer != null) renderer.material = tileNormalMaterial;
            }
        }

        private void Start() {
            SetButtonActionListener();
        }

        // Load the total count information UI text for all props
        private void LoadPropTotalCount() {
            ghostCountText.text = "Total:" + _totalPropCounts["GhostSpawn"];
            powerPelletCountText.text = "Total:" + _totalPropCounts["PowerPellet"];
            fastWheelCountText.text = "Total:" + _totalPropCounts["FastWheel"];
            niceBombCountText.text = "Total:" + _totalPropCounts["NiceBomb"];
            slowWheelCountText.text = "Total:" + _totalPropCounts["SlowWheel"];
            badCherryCountText.text = "Total:" + _totalPropCounts["BadCherry"];
            luckyDiceCountText.text = "Total:" + _totalPropCounts["LuckyDice"];
        }

        // Handle mouse press
        void Update() {
            if (!propMode) return;
            if (Input.GetMouseButtonDown(0)) {
                SelectTile();
            }
        }

        // Select a tile
        private void SelectTile() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                Vector3 tilePosition = SnapToGrid(hit.point);

                if (!hit.collider.CompareTag("Tile")) {
                    Debug.Log("Warning: Clicked but not found a tile!");
                    return;
                }

                if (tilePosition != _selectedTileVector3) {
                    // Display the prop edit panel
                    tileNotSelectedPrompt.SetActive(false);
                    TotalNumberButtonUpdate();
                    propEditPanel.SetActive(true);

                    _selectedTileVector3 = tilePosition;
                    _tileSelected = true;

                    // Change the material of the last tile back to the normal one
                    if (_lastSelectedTile != null) {
                        var renderer = _lastSelectedTile.GetComponent<Renderer>();
                        if (renderer != null) renderer.material = tileNormalMaterial;
                    }

                    // Set the material of the currently selected tile
                    GameObject currentTile = hit.collider.gameObject;
                    var currentRenderer = currentTile.GetComponent<Renderer>();
                    if (currentRenderer != null) currentRenderer.material = tileHighlightMaterial;

                    // Update the last selected tile
                    _lastSelectedTile = currentTile;

                    Debug.Log($"Tile Selected: {_selectedTileVector3}");
                }

                // UI update
                if (!_propOnTiles.ContainsKey(_selectedTileVector3) || _propOnTiles[_selectedTileVector3] == null) {
                    Debug.Log("UI: This tile does not have a valid tile.");
                    PropMissingButtonUpdate();
                } else {
                    Debug.Log("UI: This tile have a valid tile.");
                    PropPlacedButtonUpdate();
                }
            }
        }

        // Convert arbitrary coordinates (mouse click) to the nearest grid point
        private Vector3 SnapToGrid(Vector3 position) {
            int xIndex = Mathf.RoundToInt((position.x - _gridStart.x) / _gridSpacing);
            int zIndex = Mathf.RoundToInt((position.z - _gridStart.z) / -_gridSpacing);

            xIndex = Mathf.Clamp(xIndex, 0, _gridSize - 1);
            zIndex = Mathf.Clamp(zIndex, 0, _gridSize - 1);

            return new Vector3(_gridStart.x + xIndex * _gridSpacing, 0, _gridStart.z - zIndex * _gridSpacing);
        }

        // Prop placement operations
        public bool PlaceProp(string propType) {
            if (!_tileSelected) return false;

            if (!_propOnTiles.ContainsKey(_selectedTileVector3)) {
                _propOnTiles[_selectedTileVector3] = new GameObject();
            }

            if (propType == "PacmanSpawn" && _fixedPropCounts["PacmanSpawn"] >= 1) {
                Debug.Log("WARNING Only one Pacman Spawn Point is allowed.");
                return false;
            }

            if (propType != "PacmanSpawn" && _fixedPropCounts[propType] > _totalPropCounts[propType]) {
                Debug.Log($"WARNING Maximum of: {propType} reached (defined by the total number).");
                return false;
            }

            GameObject prefab = GetPrefabByType(propType);
            if (prefab == null) {
                Debug.LogError($"WARNING Invalid prop type: {propType}");
                return false;
            }

            GameObject newProp = Instantiate(prefab, _selectedTileVector3, Quaternion.identity);
            _propOnTiles[_selectedTileVector3] = newProp;
            _fixedPropCounts[propType]++;

            // UI update
            PropPlacedButtonUpdate();

            return true;
        }

        // Handle removals
        public bool RemoveProp() {
            Debug.Log("Removing");

            if (!_tileSelected) {
                Debug.LogError("NO TILE SELECTED");
                return false;
            }

            if (!_propOnTiles.ContainsKey(_selectedTileVector3) || _propOnTiles[_selectedTileVector3] == null) {
                Debug.LogError("TILE CONTAINS NO PROPS OR NULL");
                return false;
            }

            GameObject propToRemove = _propOnTiles[_selectedTileVector3];
            if (propToRemove != null) {
                Destroy(propToRemove);
                _propOnTiles[_selectedTileVector3] = null;

                string propName = CleanName(propToRemove.name);

                Debug.Log("Prop to be removed: " + propName);

                // Update the fixed prop counts
                _fixedPropCounts[propName]--;

                // Enable the corresponding minus button if the fixed count is already less than total number
                if (_fixedPropCounts[propName] < _totalPropCounts[propName]) {
                    switch (propName) {
                        case "GhostSpawn":
                            ghostSpawnMinus.gameObject.SetActive(true);
                            break;
                        case "PowerPellet":
                            powerPelletMinus.gameObject.SetActive(true);
                            break;
                        case "FastWheel":
                            fastWheelMinus.gameObject.SetActive(true);
                            break;
                        case "NiceBomb":
                            niceBombMinus.gameObject.SetActive(true);
                            break;
                        case "SlowWheel":
                            slowWheelMinus.gameObject.SetActive(true);
                            break;
                        case "BadCherry":
                            badCherryMinus.gameObject.SetActive(true);
                            break;
                        case "LuckyDice":
                            luckyDiceMinus.gameObject.SetActive(true);
                            break;
                        default:
                            Debug.LogError("Invalid prop Name: " + propName);
                            break;
                    }
                }
            } else {
                Debug.LogError("PROP TO REMOVE IS NULL");
            }

            // UI update
            PropMissingButtonUpdate();

            return true;
        }

        // Obtain the prefab of different types of prop
        private GameObject GetPrefabByType(string propType) {
            return propType switch {
                "GhostSpawn" => ghostSpawnPrefab,
                "PacmanSpawn" => pacmanSpawnPrefab,
                "PowerPellet" => powerPelletPrefab,
                "FastWheel" => fastWheelPrefab,
                "NiceBomb" => niceBombPrefab,
                "SlowWheel" => slowWheelPrefab,
                "BadCherry" => badCherryPrefab,
                "LuckyDice" => luckyDicePrefab,
                _ => null
            };
        }

        // Checks if the condition is satisfied for save & quit, returns:
        // true if all set
        // false if the pacman spawn point is not set
        private bool CheckCondition() {
            if (_fixedPropCounts["PacmanSpawn"] == 0) return false;
            return true;
        }

        /* On click operations */
        // Props buttons
        private void OnPacmanSpawnButtonClick() {
            PlaceProp("PacmanSpawn");
        }

        private void OnGhostSpawnButtonClick() {
            if (!PlaceProp("GhostSpawn")) {
                Debug.LogError("GhostSpawn place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts["GhostSpawn"] == _totalPropCounts["GhostSpawn"]) {
                ghostSpawnMinus.gameObject.SetActive(false);
            }
        }

        private void OnPowerPelletButtonClick() {
            if (!PlaceProp("PowerPellet")) {
                Debug.LogError("PowerPellet place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts["PowerPellet"] == _totalPropCounts["PowerPellet"]) {
                powerPelletMinus.gameObject.SetActive(false);
            }
        }

        private void OnFastWheelButtonClick() {
            if (!PlaceProp("FastWheel")) {
                Debug.LogError("FastWheel place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts["FastWheel"] == _totalPropCounts["FastWheel"]) {
                fastWheelMinus.gameObject.SetActive(false);
            }
        }

        private void OnNiceBombButtonClick() {
            if (!PlaceProp("NiceBomb")) {
                Debug.LogError("NiceBomb place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts["NiceBomb"] == _totalPropCounts["NiceBomb"]) {
                niceBombMinus.gameObject.SetActive(false);
            }
        }

        private void OnSlowWheelButtonClick() {
            if (!PlaceProp("SlowWheel")) {
                Debug.LogError("SlowWheel place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts["SlowWheel"] == _totalPropCounts["SlowWheel"]) {
                slowWheelMinus.gameObject.SetActive(false);
            }
        }

        private void OnBadCherryButtonClick() {
            if (!PlaceProp("BadCherry")) {
                Debug.LogError("BadCherry place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts["BadCherry"] == _totalPropCounts["BadCherry"]) {
                badCherryMinus.gameObject.SetActive(false);
            }
        }

        private void OnLuckyDiceButtonClick() {
            if (!PlaceProp("LuckyDice")) {
                Debug.LogError("LuckyDice place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts["LuckyDice"] == _totalPropCounts["LuckyDice"]) {
                luckyDiceMinus.gameObject.SetActive(false);
            }
        }

        // Remove operation
        private void OnRemoveButtonClick() {
            if (!RemoveProp()) {
                Debug.LogError("Remove error!");
            }
        }

        // Add/minus buttons
        private void OnGhostSpawnAddClick() {
            if (_totalPropCounts["GhostSpawn"] >= 5) {
                Debug.LogError("Ghost add error!");
                return;
            }

            // Update count
            _totalPropCounts["GhostSpawn"]++;
            ghostCountText.text = "Total:" + _totalPropCounts["GhostSpawn"];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts["GhostSpawn"] > _fixedPropCounts["GhostSpawn"]) {
                ghostSpawnButton.interactable = true;
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["GhostSpawn"] == 5) {
                ghostSpawnAdd.gameObject.SetActive(false);
            }

            // If already more than one ghost or the number of fixed ones, enable the minus button
            if (_totalPropCounts["GhostSpawn"] == 2 ||
                _totalPropCounts["GhostSpawn"] > _fixedPropCounts["GhostSpawn"]) {
                ghostSpawnMinus.gameObject.SetActive(true);
            }
        }

        private void OnGhostSpawnMinusClick() {
            if (_totalPropCounts["GhostSpawn"] <= 1) {
                Debug.LogError("Ghost minus minus error!");
                return;
            }

            // Update count
            _totalPropCounts["GhostSpawn"]--;
            ghostCountText.text = "Total:" + _totalPropCounts["GhostSpawn"];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts["GhostSpawn"] == _fixedPropCounts["GhostSpawn"]) {
                ghostSpawnButton.interactable = false;
            }

            // If reaches the minimum number 1 or the number of fixed ones, disable the minus button
            if (_totalPropCounts["GhostSpawn"] == 1 ||
                _totalPropCounts["GhostSpawn"] == _fixedPropCounts["GhostSpawn"]) {
                ghostSpawnMinus.gameObject.SetActive(false);
            }

            // If already less than five ghosts, enable the add button
            if (_totalPropCounts["GhostSpawn"] == 4) {
                ghostSpawnAdd.gameObject.SetActive(true);
            }
        }

        private void OnPowerPelletAddClick() {
            if (_totalPropCounts["PowerPellet"] >= 5) {
                Debug.LogError("Power pellet add error!");
                return;
            }

            // Update count
            _totalPropCounts["PowerPellet"]++;
            powerPelletCountText.text = "Total:" + _totalPropCounts["PowerPellet"];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts["PowerPellet"] > _fixedPropCounts["PowerPellet"]) {
                powerPelletButton.interactable = true;
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["PowerPellet"] == 5) {
                powerPelletAdd.gameObject.SetActive(false);
            }

            // If already more than zero power pellet or the number of fixed ones, enable the minus button
            if (_totalPropCounts["PowerPellet"] == 1 ||
                _totalPropCounts["PowerPellet"] > _fixedPropCounts["PowerPellet"]) {
                powerPelletMinus.gameObject.SetActive(true);
            }
        }

        private void OnPowerPelletMinusClick() {
            if (_totalPropCounts["PowerPellet"] <= 0) {
                Debug.LogError("Power pellet minus error!");
                return;
            }

            // Update count
            _totalPropCounts["PowerPellet"]--;
            powerPelletCountText.text = "Total:" + _totalPropCounts["PowerPellet"];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts["PowerPellet"] == _fixedPropCounts["PowerPellet"]) {
                powerPelletButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts["PowerPellet"] == 0 ||
                _totalPropCounts["PowerPellet"] == _fixedPropCounts["PowerPellet"]) {
                powerPelletMinus.gameObject.SetActive(false);
            }

            // If already less than five power pellets, enable the add button
            if (_totalPropCounts["PowerPellet"] == 4) {
                powerPelletAdd.gameObject.SetActive(true);
            }
        }

        private void OnFastWheelAddClick() {
            if (_totalPropCounts["FastWheel"] >= 5) {
                Debug.LogError("Fast wheel add error!");
                return;
            }

            // Update count
            _totalPropCounts["FastWheel"]++;
            fastWheelCountText.text = "Total:" + _totalPropCounts["FastWheel"];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts["FastWheel"] > _fixedPropCounts["FastWheel"]) {
                fastWheelButton.interactable = true;
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["FastWheel"] == 5) {
                fastWheelAdd.gameObject.SetActive(false);
            }

            // If already more than zero fast wheel or the number of fixed ones, enable the minus button
            if (_totalPropCounts["FastWheel"] == 1 || _totalPropCounts["FastWheel"] > _fixedPropCounts["FastWheel"]) {
                fastWheelMinus.gameObject.SetActive(true);
            }
        }

        private void OnFastWheelMinusClick() {
            if (_totalPropCounts["FastWheel"] <= 0) {
                Debug.LogError("Fast wheel minus error!");
                return;
            }

            // Update count
            _totalPropCounts["FastWheel"]--;
            fastWheelCountText.text = "Total:" + _totalPropCounts["FastWheel"];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts["FastWheel"] == _fixedPropCounts["FastWheel"]) {
                fastWheelButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts["FastWheel"] == 0 || _totalPropCounts["FastWheel"] == _fixedPropCounts["FastWheel"]) {
                fastWheelMinus.gameObject.SetActive(false);
            }

            // If already less than five fast wheels, enable the add button
            if (_totalPropCounts["FastWheel"] == 4) {
                fastWheelAdd.gameObject.SetActive(true);
            }
        }

        private void OnNiceBombAddClick() {
            if (_totalPropCounts["NiceBomb"] >= 5) {
                Debug.LogError("Nice bomb add error!");
                return;
            }

            // Update count
            _totalPropCounts["NiceBomb"]++;
            niceBombCountText.text = "Total:" + _totalPropCounts["NiceBomb"];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts["NiceBomb"] > _fixedPropCounts["NiceBomb"]) {
                niceBombButton.interactable = true;
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["NiceBomb"] == 5) {
                niceBombAdd.gameObject.SetActive(false);
            }

            // If already more than zero nice bomb or the number of fixed ones, enable the minus button
            if (_totalPropCounts["NiceBomb"] == 1 || _totalPropCounts["NiceBomb"] > _fixedPropCounts["NiceBomb"]) {
                niceBombMinus.gameObject.SetActive(true);
            }
        }

        private void OnNiceBombMinusClick() {
            if (_totalPropCounts["NiceBomb"] <= 0) {
                Debug.LogError("Nice bomb minus error!");
                return;
            }

            // Update count
            _totalPropCounts["NiceBomb"]--;
            niceBombCountText.text = "Total:" + _totalPropCounts["NiceBomb"];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts["NiceBomb"] == _fixedPropCounts["NiceBomb"]) {
                niceBombButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts["NiceBomb"] == 0 || _totalPropCounts["NiceBomb"] == _fixedPropCounts["NiceBomb"]) {
                niceBombMinus.gameObject.SetActive(false);
            }

            // If already less than five nice bombs, enable the add button
            if (_totalPropCounts["NiceBomb"] == 4) {
                niceBombAdd.gameObject.SetActive(true);
            }
        }

        private void OnSlowWheelAddClick() {
            if (_totalPropCounts["SlowWheel"] >= 5) {
                Debug.LogError("Slow wheel add error!");
                return;
            }

            // Update count
            _totalPropCounts["SlowWheel"]++;
            slowWheelCountText.text = "Total:" + _totalPropCounts["SlowWheel"];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts["SlowWheel"] > _fixedPropCounts["SlowWheel"]) {
                slowWheelButton.interactable = true;
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["SlowWheel"] == 5) {
                slowWheelAdd.gameObject.SetActive(false);
            }

            // If already more than zero slow wheel or the number of fixed ones, enable the minus button
            if (_totalPropCounts["SlowWheel"] == 1 || _totalPropCounts["SlowWheel"] > _fixedPropCounts["SlowWheel"]) {
                slowWheelMinus.gameObject.SetActive(true);
            }
        }

        private void OnSlowWheelMinusClick() {
            if (_totalPropCounts["SlowWheel"] <= 0) {
                Debug.LogError("Slow wheel minus error!");
                return;
            }

            // Update count
            _totalPropCounts["SlowWheel"]--;
            slowWheelCountText.text = "Total:" + _totalPropCounts["SlowWheel"];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts["SlowWheel"] == _fixedPropCounts["SlowWheel"]) {
                slowWheelButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts["SlowWheel"] == 0 || _totalPropCounts["SlowWheel"] == _fixedPropCounts["SlowWheel"]) {
                slowWheelMinus.gameObject.SetActive(false);
            }

            // If already less than five slow wheels, enable the add button
            if (_totalPropCounts["SlowWheel"] == 4) {
                slowWheelAdd.gameObject.SetActive(true);
            }
        }

        private void OnBadCherryAddClick() {
            if (_totalPropCounts["BadCherry"] >= 5) {
                Debug.LogError("Bad cherry add error!");
                return;
            }

            // Update count
            _totalPropCounts["BadCherry"]++;
            badCherryCountText.text = "Total:" + _totalPropCounts["BadCherry"];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts["BadCherry"] > _fixedPropCounts["BadCherry"]) {
                badCherryButton.interactable = true;
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["BadCherry"] == 5) {
                badCherryAdd.gameObject.SetActive(false);
            }

            // If already more than zero bad cherry or the number of fixed ones, enable the minus button
            if (_totalPropCounts["BadCherry"] == 1 || _totalPropCounts["BadCherry"] > _fixedPropCounts["BadCherry"]) {
                badCherryMinus.gameObject.SetActive(true);
            }
        }

        private void OnBadCherryMinusClick() {
            if (_totalPropCounts["BadCherry"] <= 0) {
                Debug.LogError("Bad cherry minus error!");
                return;
            }

            // Update count
            _totalPropCounts["BadCherry"]--;
            badCherryCountText.text = "Total:" + _totalPropCounts["BadCherry"];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts["BadCherry"] == _fixedPropCounts["BadCherry"]) {
                badCherryButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts["BadCherry"] == 0 || _totalPropCounts["BadCherry"] == _fixedPropCounts["BadCherry"]) {
                badCherryMinus.gameObject.SetActive(false);
            }

            // If already less than five bad cherries, enable the add button
            if (_totalPropCounts["BadCherry"] == 4) {
                badCherryAdd.gameObject.SetActive(true);
            }
        }

        private void OnLuckyDiceAddClick() {
            if (_totalPropCounts["LuckyDice"] >= 5) {
                Debug.LogError("Lucky dice add error!");
                return;
            }

            // Update count
            _totalPropCounts["LuckyDice"]++;
            luckyDiceCountText.text = "Total:" + _totalPropCounts["LuckyDice"];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts["LuckyDice"] > _fixedPropCounts["LuckyDice"]) {
                luckyDiceButton.interactable = true;
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["LuckyDice"] == 5) {
                luckyDiceAdd.gameObject.SetActive(false);
            }

            // If already more than zero lucky dice or the number of fixed ones, enable the minus button
            if (_totalPropCounts["LuckyDice"] == 1 || _totalPropCounts["LuckyDice"] > _fixedPropCounts["LuckyDice"]) {
                luckyDiceMinus.gameObject.SetActive(true);
            }
        }

        private void OnLuckyDiceMinusClick() {
            if (_totalPropCounts["LuckyDice"] <= 0) {
                Debug.LogError("Lucky dice minus error!");
                return;
            }

            // Update count
            _totalPropCounts["LuckyDice"]--;
            luckyDiceCountText.text = "Total:" + _totalPropCounts["LuckyDice"];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts["LuckyDice"] == _fixedPropCounts["LuckyDice"]) {
                luckyDiceButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts["LuckyDice"] == 0 || _totalPropCounts["LuckyDice"] == _fixedPropCounts["LuckyDice"]) {
                luckyDiceMinus.gameObject.SetActive(false);
            }

            // If already less than five lucky dices, enable the add button
            if (_totalPropCounts["LuckyDice"] == 4) {
                luckyDiceAdd.gameObject.SetActive(true);
            }
        }

        // Button action listener setting
        private void SetButtonActionListener() {
            ghostSpawnButton.onClick.AddListener(OnGhostSpawnButtonClick);
            pacmanSpawnButton.onClick.AddListener(OnPacmanSpawnButtonClick);
            powerPelletButton.onClick.AddListener(OnPowerPelletButtonClick);
            fastWheelButton.onClick.AddListener(OnFastWheelButtonClick);
            niceBombButton.onClick.AddListener(OnNiceBombButtonClick);
            slowWheelButton.onClick.AddListener(OnSlowWheelButtonClick);
            badCherryButton.onClick.AddListener(OnBadCherryButtonClick);
            luckyDiceButton.onClick.AddListener(OnLuckyDiceButtonClick);

            removeButton.onClick.AddListener(OnRemoveButtonClick);

            ghostSpawnAdd.onClick.AddListener(OnGhostSpawnAddClick);
            ghostSpawnMinus.onClick.AddListener(OnGhostSpawnMinusClick);
            powerPelletAdd.onClick.AddListener(OnPowerPelletAddClick);
            powerPelletMinus.onClick.AddListener(OnPowerPelletMinusClick);
            fastWheelAdd.onClick.AddListener(OnFastWheelAddClick);
            fastWheelMinus.onClick.AddListener(OnFastWheelMinusClick);
            niceBombAdd.onClick.AddListener(OnNiceBombAddClick);
            niceBombMinus.onClick.AddListener(OnNiceBombMinusClick);
            slowWheelAdd.onClick.AddListener(OnSlowWheelAddClick);
            slowWheelMinus.onClick.AddListener(OnSlowWheelMinusClick);
            badCherryAdd.onClick.AddListener(OnBadCherryAddClick);
            badCherryMinus.onClick.AddListener(OnBadCherryMinusClick);
            luckyDiceAdd.onClick.AddListener(OnLuckyDiceAddClick);
            luckyDiceMinus.onClick.AddListener(OnLuckyDiceMinusClick);
        }

        // When a prop is placed on a block:
        // Disable all props button, enable the remove button
        private void PropPlacedButtonUpdate() {
            ghostSpawnButton.interactable = false;
            pacmanSpawnButton.interactable = false;
            powerPelletButton.interactable = false;
            fastWheelButton.interactable = false;
            niceBombButton.interactable = false;
            slowWheelButton.interactable = false;
            badCherryButton.interactable = false;
            luckyDiceButton.interactable = false;

            removeButton.interactable = true;

            Debug.Log("UI updated: Only remove button available");
        }

        // When a prop is removed/missing on a block:
        // Disable the remove button, enable all props button
        private void PropMissingButtonUpdate() {
            // Disable the remove button
            removeButton.interactable = false;

            // Check every prop except for Pacman spawn:
            // Only if its current total count is more than the fixed count can its button be enabled
            if (_totalPropCounts["GhostSpawn"] > _fixedPropCounts["GhostSpawn"]) {
                ghostSpawnButton.interactable = true;
            }

            if (_totalPropCounts["PowerPellet"] > _fixedPropCounts["PowerPellet"]) {
                powerPelletButton.interactable = true;
            }

            if (_totalPropCounts["FastWheel"] > _fixedPropCounts["FastWheel"]) {
                fastWheelButton.interactable = true;
            }

            if (_totalPropCounts["NiceBomb"] > _fixedPropCounts["NiceBomb"]) {
                niceBombButton.interactable = true;
            }

            if (_totalPropCounts["SlowWheel"] > _fixedPropCounts["SlowWheel"]) {
                slowWheelButton.interactable = true;
            }

            if (_totalPropCounts["BadCherry"] > _fixedPropCounts["BadCherry"]) {
                badCherryButton.interactable = true;
            }

            if (_totalPropCounts["LuckyDice"] > _fixedPropCounts["LuckyDice"]) {
                luckyDiceButton.interactable = true;
            }

            // Pacman spawn button logic
            if (_fixedPropCounts["PacmanSpawn"] == 0) {
                pacmanSpawnButton.interactable = true;
            }

            Debug.Log("UI updated: Only prop buttons available");
        }

        // Update the status of the total number add/minus buttons
        private void TotalNumberButtonUpdate() {
            // 1 Add buttons
            // Reaching the maximum number 5

            if (_totalPropCounts["GhostSpawn"] == 5) {
                ghostSpawnAdd.gameObject.SetActive(false);
            } else {
                ghostSpawnAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts["PowerPellet"] == 5) {
                powerPelletAdd.gameObject.SetActive(false);
            } else {
                powerPelletAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts["FastWheel"] == 5) {
                fastWheelAdd.gameObject.SetActive(false);
            } else {
                fastWheelAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts["NiceBomb"] == 5) {
                niceBombAdd.gameObject.SetActive(false);
            } else {
                niceBombAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts["SlowWheel"] == 5) {
                slowWheelMinus.gameObject.SetActive(false);
            } else {
                slowWheelMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts["BadCherry"] == 5) {
                badCherryAdd.gameObject.SetActive(false);
            } else {
                badCherryAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts["LuckyDice"] == 5) {
                luckyDiceAdd.gameObject.SetActive(false);
            } else {
                luckyDiceAdd.gameObject.SetActive(true);
            }

            // 2 Minus buttons
            // 2.1 At their minimum numbers (1 for ghost spawn and 0 for others)

            if (_totalPropCounts["GhostSpawn"] == 1) {
                ghostSpawnMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts["PowerPellet"] == 0) {
                powerPelletMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts["FastWheel"] == 0) {
                fastWheelMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts["NiceBomb"] == 0) {
                niceBombMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts["SlowWheel"] == 0) {
                slowWheelMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts["BadCherry"] == 0) {
                badCherryMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts["LuckyDice"] == 0) {
                luckyDiceMinus.gameObject.SetActive(false);
            }

            // 2.2 Equal to their fixed prop numbers

            if (_totalPropCounts["GhostSpawn"] == _fixedPropCounts["GhostSpawn"]) {
                ghostSpawnMinus.gameObject.SetActive(false);
            } else {
                ghostSpawnMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts["PowerPellet"] == _fixedPropCounts["PowerPellet"]) {
                powerPelletMinus.gameObject.SetActive(false);
            } else {
                powerPelletMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts["FastWheel"] == _fixedPropCounts["FastWheel"]) {
                fastWheelMinus.gameObject.SetActive(false);
            } else {
                fastWheelMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts["NiceBomb"] == _fixedPropCounts["NiceBomb"]) {
                niceBombMinus.gameObject.SetActive(false);
            } else {
                niceBombMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts["SlowWheel"] == _fixedPropCounts["SlowWheel"]) {
                slowWheelMinus.gameObject.SetActive(false);
            } else {
                slowWheelMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts["BadCherry"] == _fixedPropCounts["BadCherry"]) {
                badCherryMinus.gameObject.SetActive(false);
            } else {
                badCherryMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts["LuckyDice"] == _fixedPropCounts["LuckyDice"]) {
                luckyDiceMinus.gameObject.SetActive(false);
            } else {
                luckyDiceMinus.gameObject.SetActive(true);
            }
        }

        // Remove the "(Clone)" at the end of the game object Name if it exists
        // Used when trying to destroy an object
        private string CleanName(string nameToBeCleaned) {
            const string cloneTag = "(Clone)";
            return nameToBeCleaned.EndsWith(cloneTag)
                ? nameToBeCleaned.Remove(nameToBeCleaned.Length - 7)
                : nameToBeCleaned;
        }

        // Initial setting: Disable all the props buttons (including remove) at the beginning
        private void PropsButtonInit() {
            ghostSpawnButton.interactable = false;
            pacmanSpawnButton.interactable = false;
            powerPelletButton.interactable = false;
            fastWheelButton.interactable = false;
            niceBombButton.interactable = false;
            slowWheelButton.interactable = false;
            badCherryButton.interactable = false;
            luckyDiceButton.interactable = false;
            removeButton.interactable = false;
        }

        /**
         * Obtains the data about the props.
         * Called in MapEditor.
         */
        public PropData GetPropData() {
            return new PropData(_propOnTiles, _fixedPropCounts, _totalPropCounts);
        }
    }
}