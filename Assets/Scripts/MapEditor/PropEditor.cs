using System;
using System.Collections.Generic;
using TMPro;
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
        public Button clearButton;

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
            { "PacmanSpawn", 0 },
            { "GhostSpawn", 0 },
            { "PowerPellet", 0 },
            { "FastWheel", 0 },
            { "NiceBomb", 0 },
            { "SlowWheel", 0 },
            { "BadCherry", 0 },
            { "LuckyDice", 0 }
        };

        private readonly int _maxPropCount = 5; // Max prop count (EXCEPT PACMAN SPAWN POINT)
        private readonly Vector3 _gridStart = new(-15, 0, 15); // The top left corner
        private readonly float _gridSpacing = 3.0f; // The length of each tile
        private readonly int _gridSize = 11; // Map grid size

        private void Start() {
            propMode = false;

            // Button functions bonding
            ghostSpawnButton.onClick.AddListener(OnGhostSpawnButtonClick);
            pacmanSpawnButton.onClick.AddListener(OnPacmanSpawnButtonClick);
            powerPelletButton.onClick.AddListener(OnPowerPelletButtonClick);
            fastWheelButton.onClick.AddListener(OnFastWheelButtonClick);
            niceBombButton.onClick.AddListener(OnNiceBombButtonClick);
            slowWheelButton.onClick.AddListener(OnSlowWheelButtonClick);
            badCherryButton.onClick.AddListener(OnBadCherryButtonClick);
            luckyDiceButton.onClick.AddListener(OnLuckyDiceButtonClick);
            clearButton.onClick.AddListener(OnClearButtonClick);

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
                if (tilePosition != _selectedTile) {
                    _selectedTile = tilePosition;
                    _tileSelected = true;
                    Debug.Log($"Tile Selected: {_selectedTile}");
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

        // Handle prop placements
        public void PlaceProp(string propType) {
            if (!_tileSelected) return;

            if (!_propOnTiles.ContainsKey(_selectedTile)) {
                _propOnTiles[_selectedTile] = new GameObject();
            }

            if (propType == "PacmanSpawn" && _fixedPropCounts["PacmanSpawn"] >= 1) {
                Debug.Log("WARNING Only one Pacman Spawn Point is allowed.");
                return;
            }

            if (_fixedPropCounts[propType] >= _maxPropCount) {
                Debug.Log($"WARNING Maximum of {_maxPropCount} {propType}s reached.");
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
        }

        // Handle removals
        public void RemoveProp(string propType) {
            if (!_tileSelected) return;
            if (!_propOnTiles.ContainsKey(_selectedTile) || _propOnTiles[_selectedTile] == null) return;

            GameObject propToRemove = _propOnTiles[_selectedTile];
            if (propToRemove != null) {
                Destroy(propToRemove);
                _propOnTiles[_selectedTile] = null;
                _fixedPropCounts[propType]--;
            }
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


        /* On click operations */
        // Props buttons
        private void OnPacmanSpawnButtonClick() { }

        private void OnGhostSpawnButtonClick() { }

        private void OnPowerPelletButtonClick() { }

        private void OnFastWheelButtonClick() { }

        private void OnNiceBombButtonClick() { }

        private void OnSlowWheelButtonClick() { }

        private void OnBadCherryButtonClick() { }

        private void OnLuckyDiceButtonClick() { }

        // Clear
        private void OnClearButtonClick() { }

        // Add/minus buttons
        private void OnGhostSpawnAddClick() { }

        private void OnGhostSpawnMinusClick() { }

        private void OnPowerPelletAddClick() { }

        private void OnPowerPelletMinusClick() { }

        private void OnFastWheelAddClick() { }

        private void OnFastWheelMinusClick() { }

        private void OnNiceBombAddClick() { }

        private void OnNiceBombMinusClick() { }

        private void OnSlowWheelAddClick() { }

        private void OnSlowWheelMinusClick() { }

        private void OnBadCherryAddClick() { }

        private void OnBadCherryMinusClick() { }

        private void OnLuckyDiceAddClick() { }

        private void OnLuckyDiceMinusClick() { }
    }
}