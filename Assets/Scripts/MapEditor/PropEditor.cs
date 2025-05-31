using System.Collections.Generic;
using Entity.Map;
using Entity.Prop;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    /**
     * Used for tile & prop edit.
     *
     * PacMaze Game Props:
     *
     * BASIC
     * - Ghostron Spawn Point
     * - Pacboy Spawn Point
     *
     * GOOD ONES
     * - Power Pellet
     * - Fast Wheel
     * - Nice Bomb
     *
     * BAD ONES
     * - Slow Wheel
     * - Bad Cherry
     *
     * RANDOM
     * - Lucky Dice
     */
    public class PropEditor : MonoBehaviour {
        private Vector3 _selectedTileVector3; // the selected tile's vector
        private bool _tileSelected; // if a tile is selected

        // Materials
        public Material tileNormalMaterial;
        public Material tileHighlightMaterial;

        private GameObject _lastSelectedTile; // Last selected tile (to change that to normal material)

        // All prop models
        public GameObject ghostronSpawnPrefab;
        public GameObject pacboySpawnPrefab;
        public GameObject powerPelletPrefab;
        public GameObject fastWheelPrefab;
        public GameObject niceBombPrefab;
        public GameObject slowWheelPrefab;
        public GameObject badCherryPrefab;
        public GameObject luckyDicePrefab;

        // Remove fixed prop button
        public Button removeButton;

        /* All prop operation UI elements are designed in Prop UI Definitions */
        // Define UI elements and settings for each prop type
        public List<PropUIDefinition> propUIDefinitions;

        // Mapping from PropType to PropUIDefinition
        private Dictionary<PropType, PropUIDefinition> _propDefinitionsDict;

        // The editing panel, active when a tile is selected
        public GameObject propEditPanel;

        // The prompt shown when currently no tile is selected (when the player just enters this mode)
        public GameObject tileNotSelectedPrompt;

        // FIXED prop game objects on every tile
        private readonly Dictionary<Vector3, GameObject> _propObjectOnTiles = new();

        // FIXED counts of all the props
        private Dictionary<PropType, int> _fixedPropCounts;

        // TOTAL counts of all the props - including FIXED and RANDOM ones
        private Dictionary<PropType, int> _totalPropCounts;

        private readonly Vector3 _gridStart = new(-15, 0, 15); // The top left corner
        private readonly float _gridSpacing = 3.0f; // The length of each tile
        private readonly int _gridSize = 11; // Map grid size
        private bool _propMode;

        // Singleton instance
        public static PropEditor Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("TileChecker AWAKE");
            // Set singleton instance
            Instance = this;

            // Initialise the Dictionary<PropType, PropUIDefinition>
            InitPropDefinitionsDict();
        }

        // START FUNCTION
        private void Start() {
            // Set button action listeners
            SetButtonActionListener();
        }

        /**
         * Sets the prop data. Used in MapEditor class.
         * Information to be set: propOnTiles, fixedPropCounts, totalPropCounts
         */
        public void SetPropData(PropData propData) {
            // Prop count data update
            _fixedPropCounts = propData.FixedPropCounts;
            _totalPropCounts = propData.TotalPropCounts;

            // Prop place info update
            // & Place props on the map
            foreach (var kvp in propData.PropOnTiles) {
                GameObject prefab = GetPrefabByPropType(kvp.Value);

                if (prefab == null) {
                    Debug.LogError($"WARNING Invalid prop type: {prefab}");
                    return;
                }

                GameObject newProp = Instantiate(prefab, kvp.Key, Quaternion.identity);
                _propObjectOnTiles[kvp.Key] = newProp;
            }

            // UI update
            UpdateAllUI();
        }

        /**
         * Enters/Quits the prop editing mode. Used in MapEditor class.
         * PARAM resetSelectedTile:
         *
         * - true (in most cases):
         * Set the tile that is last selected to normal material, aka cancel the highlight effect.
         *
         * - false (only used when showing invalid tile effect):
         * Do not do so. This is because this could override the invalid "red" effect
         * if the last selected tile happens to be invalid.
         *
         */
        public void SetPropMode(bool enter, bool resetSelectedTile) {
            if (enter) {
                // Enter
                _propMode = true;
                _tileSelected = false;

                propEditPanel.SetActive(false);
                tileNotSelectedPrompt.SetActive(true);
                // PropsButtonInit();

                InitAllFixedPlacementButtons(); // General init for spawn buttons
            } else {
                // Quit
                _propMode = false;

                // Change the material of the last tile back to the normal one
                // ONLY DO SO if resetSelectedTile is true 
                if (resetSelectedTile && _lastSelectedTile != null) {
                    var renderer = _lastSelectedTile.GetComponent<Renderer>();
                    if (renderer != null) renderer.material = tileNormalMaterial;
                }

                // PropsButtonInit();

                InitAllFixedPlacementButtons(); // General init for spawn buttons
            }
        }

        // UPDATE FUNCTION
        void Update() {
            if (!_propMode) return;
            // Handle mouse press
            if (Input.GetMouseButtonDown(0)) {
                SelectTile();
            }
        }

        /**
         * Handle the tile selection.
         */
        private void SelectTile() {
            // Compute the target tile
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                Vector3 tilePosition = SnapToGrid(hit.point);

                // Check if any tile is successfully clicked
                if (!hit.collider.CompareTag("Tile")) {
                    Debug.Log("Warning: Clicked but not found a tile!");
                    return;
                }

                // Play click sound
                SoundManager.Instance.PlaySoundOnce(SoundType.Click);

                if (tilePosition != _selectedTileVector3 || !_tileSelected) {
                    // Display the prop edit panel
                    tileNotSelectedPrompt.SetActive(false);
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

                // UI update:
                // Update based on whether selected tile has a prop
                UpdateFixedPlacementButtonsState();
            }
        }

        /**
         * Convert arbitrary coordinates (mouse click) to the nearest grid point.
         */
        private Vector3 SnapToGrid(Vector3 position) {
            int xIndex = Mathf.RoundToInt((position.x - _gridStart.x) / _gridSpacing);
            int zIndex = Mathf.RoundToInt((position.z - _gridStart.z) / -_gridSpacing);

            xIndex = Mathf.Clamp(xIndex, 0, _gridSize - 1);
            zIndex = Mathf.Clamp(zIndex, 0, _gridSize - 1);

            return new Vector3(_gridStart.x + xIndex * _gridSpacing, 0, _gridStart.z - zIndex * _gridSpacing);
        }

        /**
         * Operation when the player clicks a prop's place button
         * to set a fixed prop on the currently selected tile.
         */
        private void PlaceFixedPropOnSelectedTile(PropType propType) {
            if (!_tileSelected || !_propDefinitionsDict.TryGetValue(propType, out PropUIDefinition def)) {
                Debug.LogError("Tile not selected or this prop type does not exist!");
                return;
            }

            // Check if tile is already occupied
            if (_propObjectOnTiles.ContainsKey(_selectedTileVector3) &&
                _propObjectOnTiles[_selectedTileVector3] != null) {
                Debug.LogError($"Tile {_selectedTileVector3} is already occupied!");
                return;
            }

            // Check if this prop is unique (Pacboy Spawn Point)
            if (def.isUniquePlacement && _fixedPropCounts[propType] >= 1) {
                Debug.LogError($"Only one {propType} can be placed!");
                return;
            }

            // Check if the fixed number of this prop reaches the total number
            if (!def.isUniquePlacement && _fixedPropCounts[propType] >= _totalPropCounts[propType]) {
                Debug.LogError(
                    $"Cannot place more {propType}. Fixed count ({_fixedPropCounts[propType]}) would exceed total count ({_totalPropCounts[propType]}).");
                return;
            }

            // Check for undefined prefab
            if (def.prefab == null) {
                Debug.LogError($"Prefab for {propType} is missing!");
                return;
            }

            // All valid
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Place fixed prop on the tile
            GameObject newProp = Instantiate(def.prefab, _selectedTileVector3, Quaternion.identity);
            _propObjectOnTiles[_selectedTileVector3] = newProp;
            _fixedPropCounts[propType]++;

            UpdatePropUI(propType); // Update UI for this specific prop
            UpdateFixedPlacementButtonsState(); // Update general placement buttons (e.g. enable remove)
        }

        /**
         * Operation when the player clicks the "Remove" button
         * to remove the currently placed FIXED prop on the selected tile.
         */
        private void RemovePropFromSelectedTile() {
            if (!_tileSelected || !_propObjectOnTiles.TryGetValue(_selectedTileVector3, out GameObject propToRemove) ||
                propToRemove == null) {
                Debug.LogError("No prop to remove on selected tile or tile not selected.");
                return;
            }

            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            PropType removedPropType = GetPropType(propToRemove); // Get the PropType of the fixed prop to be removed
            Destroy(propToRemove);
            _propObjectOnTiles.Remove(_selectedTileVector3);

            if (removedPropType != PropType.None) {
                _fixedPropCounts[removedPropType]--;
                UpdatePropUI(removedPropType);
            }

            UpdateFixedPlacementButtonsState(); // Update UI
        }

        /**
         * Adjust the total number of a prop.
         * Called when the player clicks the Add/Subtract button of a prop.
         */
        private void AdjustTotalPropCount(PropType propType, int delta) {
            if (!_propDefinitionsDict.TryGetValue(propType, out PropUIDefinition def)) return;

            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Get the current total number of this type of prop
            int currentTotal = _totalPropCounts[propType];

            // Make sure the new total does not exceed the boundaries (although the UI should be able to handle this)
            int newTotal = Mathf.Clamp(currentTotal + delta, def.minTotalCount, def.maxTotalCount);

            // Update the new total number of this prop
            _totalPropCounts[propType] = newTotal;

            UpdatePropUI(propType);
        }

        /**
         * Update all UI elements for prop editing.
         */
        private void UpdateAllUI() {
            foreach (PropType type in _propDefinitionsDict.Keys) {
                UpdatePropUI(type);
            }

            UpdateFixedPlacementButtonsState(); // For Remove button and overall state
        }

        /**
         * Update the UI for a specific prop type.
         * (Fixed place button, add button, subtract button, total count)
         */
        private void UpdatePropUI(PropType propType) {
            if (!_propDefinitionsDict.TryGetValue(propType, out PropUIDefinition def)) return;

            // Update Count Text
            if (def.countText != null) {
                def.countText.text = $"Total:{_totalPropCounts[propType]}\nFixed:{_fixedPropCounts[propType]}";
            }

            // Update Add Button
            if (def.addButton != null) {
                // Decide if this button is active or not by judging if the total number can be added more
                bool canAddMore = _totalPropCounts[propType] < def.maxTotalCount;
                def.addButton.gameObject.SetActive(canAddMore);
            }

            // Update Minus Button
            if (def.subtractButton != null) {
                // Decide if this button is active or not by judging:
                // - If the total number can be subtracted more (no if it already drops to the minimum number)
                // - If the total number is already down to the fixed number
                bool canRemove = _totalPropCounts[propType] > def.minTotalCount;
                bool totalNotEqualToFixed = _totalPropCounts[propType] > _fixedPropCounts[propType];
                def.subtractButton.gameObject.SetActive(canRemove && totalNotEqualToFixed);
            }

            // Update Fixed Place Button interactable state
            if (def.fixedPlaceButton != null) {
                // if (_tileSelected ||
                //     !_propObjectOnTiles.TryGetValue(_selectedTileVector3, out GameObject propToRemove) ||
                //     propToRemove == null) {
                //     // Not interactable if 
                //     Debug.LogError("No prop to remove on selected tile or tile not selected.");
                //     return;
                // }

                if (def.isUniquePlacement) {
                    // For Pacboy Spawn Point (only a total of one allowed)
                    def.fixedPlaceButton.interactable = _fixedPropCounts[propType] < 1;
                } else {
                    // For all other props
                    if (_tileSelected &&
                        _propObjectOnTiles.TryGetValue(_selectedTileVector3, out GameObject propToRemove) &&
                        propToRemove != null) {
                        // If the current tile is occupied, the button should be not interactable
                        def.fixedPlaceButton.interactable = false;
                    } else {
                        // Otherwise, if the prop total number is currently bigger than fixed number, make it interactable
                        def.fixedPlaceButton.interactable = _fixedPropCounts[propType] < _totalPropCounts[propType];
                    }
                }
            }
        }

        /**
         * Initialise the states of all the fixed prop placing buttons.
         */
        private void InitAllFixedPlacementButtons() {
            foreach (var pair in _propDefinitionsDict) {
                if (pair.Value.fixedPlaceButton != null) {
                    // Initially, buttons might be non-interactable until a tile is selected
                    pair.Value.fixedPlaceButton.interactable = false;
                }
            }

            if (removeButton != null) removeButton.interactable = false;
        }

        /**
         * Updates the states of all the fixed prop placing buttons.
         */
        private void UpdateFixedPlacementButtonsState() {
            // Check if currently the selected prop has a fixed prop set on it or not
            bool tileHasProp = _propObjectOnTiles.ContainsKey(_selectedTileVector3) &&
                               _propObjectOnTiles[_selectedTileVector3] != null;

            removeButton.interactable = tileHasProp;

            foreach (var pair in _propDefinitionsDict) {
                PropType type = pair.Key;
                PropUIDefinition def = pair.Value;

                if (tileHasProp) {
                    def.fixedPlaceButton.interactable = false; // Can't place on an occupied tile
                } else {
                    // Tile is empty, enable spawn if counts allow
                    bool canPlaceMoreFixed = def.isUniquePlacement
                        ? _fixedPropCounts[type] < 1
                        : _fixedPropCounts[type] < _totalPropCounts[type];
                    def.fixedPlaceButton.interactable = canPlaceMoreFixed;
                }
            }
        }

        /**
         * Initialises the dictionary mapping PropType to UI definitions.
         */
        private void InitPropDefinitionsDict() {
            _propDefinitionsDict = new Dictionary<PropType, PropUIDefinition>();
            foreach (var definition in propUIDefinitions) {
                if (definition.propType != PropType.None && !_propDefinitionsDict.ContainsKey(definition.propType)) {
                    _propDefinitionsDict.Add(definition.propType, definition);
                } else {
                    Debug.LogWarning($"Duplicate or None PropType found in propUIDefinitions: {definition.propType}");
                }
            }
        }

        /**
         * Sets the Action Listeners for all the UI buttons.
         */
        private void SetButtonActionListener() {
            foreach (var definition in propUIDefinitions) {
                if (definition.fixedPlaceButton != null) {
                    definition.fixedPlaceButton.onClick.RemoveAllListeners(); // Clear existing
                    definition.fixedPlaceButton.onClick.AddListener(() =>
                        PlaceFixedPropOnSelectedTile(definition.propType));
                }

                if (definition.addButton != null) {
                    definition.addButton.onClick.RemoveAllListeners();
                    definition.addButton.onClick.AddListener(() => AdjustTotalPropCount(definition.propType, 1));
                }

                if (definition.subtractButton != null) {
                    definition.subtractButton.onClick.RemoveAllListeners();
                    definition.subtractButton.onClick.AddListener(() => AdjustTotalPropCount(definition.propType, -1));
                }
            }

            if (removeButton != null) {
                removeButton.onClick.RemoveAllListeners();
                removeButton.onClick.AddListener(RemovePropFromSelectedTile);
            }
        }

        /**
         * Obtains the corresponding GameObject by PropType.
         */
        private GameObject GetPrefabByPropType(PropType propType) {
            return propType switch {
                PropType.Ghostron => ghostronSpawnPrefab,
                PropType.Pacboy => pacboySpawnPrefab,
                PropType.PowerPellet => powerPelletPrefab,
                PropType.FastWheel => fastWheelPrefab,
                PropType.NiceBomb => niceBombPrefab,
                PropType.SlowWheel => slowWheelPrefab,
                PropType.BadCherry => badCherryPrefab,
                PropType.LuckyDice => luckyDicePrefab,
                _ => null
            };
        }

        /**
         * Obtains the corresponding PropType name by GameObject name.
         */
        private PropType GetPropType(GameObject prop) {
            string propName = CleanName(prop.name);
            if (propName.Contains(nameof(PropType.Pacboy))) return PropType.Pacboy;
            if (propName.Contains(nameof(PropType.Ghostron))) return PropType.Ghostron;
            if (propName.Contains(nameof(PropType.PowerPellet))) return PropType.PowerPellet;
            if (propName.Contains(nameof(PropType.FastWheel))) return PropType.FastWheel;
            if (propName.Contains(nameof(PropType.NiceBomb))) return PropType.NiceBomb;
            if (propName.Contains(nameof(PropType.SlowWheel))) return PropType.SlowWheel;
            if (propName.Contains(nameof(PropType.BadCherry))) return PropType.BadCherry;
            if (propName.Contains(nameof(PropType.LuckyDice))) return PropType.LuckyDice;
            // Add other mappings as needed
            Debug.LogError($"PropType not found for GameObject name: {propName}. Returning None.");
            return PropType.None;
        }

        /**
         * Checks if the condition is satisfied for save & quit, returns:
         * - true if all set
         * - false if the Pacboy spawn point is not set
         */
        public bool CheckCondition() {
            // Must have a Pacboy spawn point
            if (_fixedPropCounts[PropType.Pacboy] == 0) return false;
            return true;
        }

        /**
         * Remove the "(Clone)" at the end of the game object name if it exists.
         * Used when trying to destroy an object.
         */
        private string CleanName(string nameToBeCleaned) {
            const string cloneTag = "(Clone)";
            return nameToBeCleaned.EndsWith(cloneTag)
                ? nameToBeCleaned.Remove(nameToBeCleaned.Length - 7)
                : nameToBeCleaned;
        }

        /**
         * Obtains the data about the props.
         * Called in MapEditor.
         */
        public PropData GetPropData() {
            // Transform from Dictionary<Vector3, GameObject> to Dictionary<Vector3, PropType>
            Dictionary<Vector3, PropType> propOnTiles = new Dictionary<Vector3, PropType>();

            foreach (var kvp in _propObjectOnTiles) {
                propOnTiles.Add(kvp.Key, GetPropType(kvp.Value));
            }

            // Construct a new PropData
            return new PropData(propOnTiles, _fixedPropCounts, _totalPropCounts);
        }
    }


    /**
     * The UI setting unit for each prop.
     */
    [System.Serializable]
    public class PropUIDefinition {
        public PropType propType;

        [Tooltip("Prefab to instantiate when placing this prop.")]
        public GameObject prefab;

        [Header("UI Elements")] public Button fixedPlaceButton;
        public Button addButton;
        public Button subtractButton;
        public TMP_Text countText;

        [Header("Count Configuration")] public int minTotalCount;
        public int maxTotalCount;
        public bool isUniquePlacement; // e.g., Pacboy can only be placed once
    }
}