using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

        public bool tileMode;

        private Dictionary<Vector3, GameObject> _propOnTiles = new(); // Prop on every tile

        // Manages the counts of all the props
        private Dictionary<string, int> _propCounts = new() {
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
            tileMode = false;
        }

        // Handle mouse press
        void Update() {
            if (!tileMode) return;
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

            if (propType == "PacmanSpawn" && _propCounts["PacmanSpawn"] >= 1) {
                Debug.Log("WARNING Only one Pacman Spawn Point is allowed.");
                return;
            }

            if (_propCounts[propType] >= _maxPropCount) {
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
            _propCounts[propType]++;
        }

        // Handle removals
        public void RemoveProp(string propType) {
            if (!_tileSelected) return;
            if (!_propOnTiles.ContainsKey(_selectedTile) || _propOnTiles[_selectedTile] == null) return;

            GameObject propToRemove = _propOnTiles[_selectedTile];
            if (propToRemove != null) {
                Destroy(propToRemove);
                _propOnTiles[_selectedTile] = null;
                _propCounts[propType]--;
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
    }
}