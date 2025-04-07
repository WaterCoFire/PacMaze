using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
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
        private Vector3 _selectedTile; // the selected tile
        private bool _tileSelected = false; // if a tile is selected

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

        public Button quitButton;
        public Button saveAndQuitButton;

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

        private void Start() {
            propMode = false;

            SetButtonActionListener();
            LoadPropTotalCount();
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

                if (tilePosition != _selectedTile) {
                    _selectedTile = tilePosition;
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

                    Debug.Log($"Tile Selected: {_selectedTile}");
                }

                // UI update
                if (!_propOnTiles.ContainsKey(_selectedTile) || _propOnTiles[_selectedTile] == null) {
                    PropMissingButtonUpdate();
                } else {
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
        public void PlaceProp(string propType) {
            if (!_tileSelected) return;

            if (!_propOnTiles.ContainsKey(_selectedTile)) {
                _propOnTiles[_selectedTile] = new GameObject();
            }

            if (propType == "PacmanSpawn" && _fixedPropCounts["PacmanSpawn"] >= 1) {
                Debug.Log("WARNING Only one Pacman Spawn Point is allowed.");
                return;
            }

            if (propType != "PacmanSpawn" && _fixedPropCounts[propType] > _totalPropCounts[propType]) {
                Debug.Log($"WARNING Maximum of: {propType} reached (defined by the total number).");
                return;
            }

            GameObject prefab = GetPrefabByType(propType);
            if (prefab == null) {
                Debug.LogError($"WARNING Invalid prop type: {propType}");
                return;
            }

            GameObject newProp = Instantiate(prefab, _selectedTile, Quaternion.identity);
            _propOnTiles[_selectedTile] = newProp;
            _fixedPropCounts[propType]++;

            // UI update
            PropPlacedButtonUpdate();
        }

        // Handle removals
        public void RemoveProp() {
            Debug.Log("Removing");
            
            if (!_tileSelected) {
                Debug.LogError("NO TILE SELECTED");
                return;
            }

            if (!_propOnTiles.ContainsKey(_selectedTile) || _propOnTiles[_selectedTile] == null) {
                Debug.LogError("TILE CONTAINS NO PROPS OR NULL");
                return;
            }

            GameObject propToRemove = _propOnTiles[_selectedTile];
            if (propToRemove != null) {
                Destroy(propToRemove);
                _propOnTiles[_selectedTile] = null;

                Debug.Log("Prop to be removed: " + propToRemove.name);
                
                // TODO (Clone) issue
                _fixedPropCounts[propToRemove.name]--;
            } else {
                Debug.LogError("PROP TO REMOVE IS NULL");
            }

            // UI update
            PropMissingButtonUpdate();
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
            PlaceProp("GhostSpawn");
        }

        private void OnPowerPelletButtonClick() {
            PlaceProp("PowerPellet");
        }

        private void OnFastWheelButtonClick() {
            PlaceProp("FastWheel");
        }

        private void OnNiceBombButtonClick() {
            PlaceProp("NiceBomb");
        }

        private void OnSlowWheelButtonClick() {
            PlaceProp("SlowWheel");
        }

        private void OnBadCherryButtonClick() {
            PlaceProp("BadCherry");
        }

        private void OnLuckyDiceButtonClick() {
            PlaceProp("LuckyDice");
        }

        // Remove operation
        private void OnRemoveButtonClick() {
            RemoveProp();
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

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["GhostSpawn"] == 5) {
                ghostSpawnAdd.gameObject.SetActive(false);
            }

            // If already more than one ghost, enable the minus button
            if (_totalPropCounts["GhostSpawn"] == 2) {
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

            // If reaches the minimum number 1, disable the minus button
            if (_totalPropCounts["GhostSpawn"] == 1) {
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

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["PowerPellet"] == 5) {
                powerPelletAdd.gameObject.SetActive(false);
            }

            // If already more than zero power pellet, enable the minus button
            if (_totalPropCounts["PowerPellet"] == 1) {
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

            // If reaches the minimum number 0, disable the minus button
            if (_totalPropCounts["PowerPellet"] == 0) {
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

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["FastWheel"] == 5) {
                fastWheelAdd.gameObject.SetActive(false);
            }

            // If already more than zero fast wheel, enable the minus button
            if (_totalPropCounts["FastWheel"] == 1) {
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

            // If reaches the minimum number 0, disable the minus button
            if (_totalPropCounts["FastWheel"] == 0) {
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

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["NiceBomb"] == 5) {
                niceBombAdd.gameObject.SetActive(false);
            }

            // If already more than zero nice bomb, enable the minus button
            if (_totalPropCounts["NiceBomb"] == 1) {
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

            // If reaches the minimum number 0, disable the minus button
            if (_totalPropCounts["NiceBomb"] == 0) {
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

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["SlowWheel"] == 5) {
                slowWheelAdd.gameObject.SetActive(false);
            }

            // If already more than zero slow wheel, enable the minus button
            if (_totalPropCounts["SlowWheel"] == 1) {
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

            // If reaches the minimum number 0, disable the minus button
            if (_totalPropCounts["SlowWheel"] == 0) {
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

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["BadCherry"] == 5) {
                badCherryAdd.gameObject.SetActive(false);
            }

            // If already more than zero bad cherry, enable the minus button
            if (_totalPropCounts["BadCherry"] == 1) {
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

            // If reaches the minimum number 0, disable the minus button
            if (_totalPropCounts["BadCherry"] == 0) {
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

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts["LuckyDice"] == 5) {
                luckyDiceAdd.gameObject.SetActive(false);
            }

            // If already more than zero lucky dice, enable the minus button
            if (_totalPropCounts["LuckyDice"] == 1) {
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

            // If reaches the minimum number 0, disable the minus button
            if (_totalPropCounts["LuckyDice"] == 0) {
                luckyDiceMinus.gameObject.SetActive(false);
            }

            // If already less than five lucky dices, enable the add button
            if (_totalPropCounts["LuckyDice"] == 4) {
                luckyDiceAdd.gameObject.SetActive(true);
            }
        }

        // When a prop is placed on a block:
        // Disable all props button, enable the remove button
        private void PropPlacedButtonUpdate() {
            ghostSpawnButton.enabled = false;
            pacmanSpawnButton.enabled = false;
            powerPelletButton.enabled = false;
            fastWheelButton.enabled = false;
            niceBombButton.enabled = false;
            slowWheelButton.enabled = false;
            badCherryButton.enabled = false;
            luckyDiceButton.enabled = false;

            removeButton.enabled = true;
            
            Debug.Log("UI updated: Only remove button available");
        }

        // When a prop is removed/missing on a block:
        // Disable the remove button, enable all props button
        private void PropMissingButtonUpdate() {
            // TODO Disable props that are all occupied by fixed ones
            
            removeButton.enabled = false;

            ghostSpawnButton.enabled = true;
            powerPelletButton.enabled = true;
            fastWheelButton.enabled = true;
            niceBombButton.enabled = true;
            slowWheelButton.enabled = true;
            badCherryButton.enabled = true;
            luckyDiceButton.enabled = true;
            
            // Pacman spawn button logic
            if (_fixedPropCounts["PacmanSpawn"] == 0) {
                pacmanSpawnButton.enabled = true;
            }
            
            Debug.Log("UI updated: Only prop buttons available");
        }
    }
}