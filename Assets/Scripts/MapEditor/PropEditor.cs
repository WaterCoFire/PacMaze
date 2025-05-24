using System.Collections.Generic;
using Entity.Map;
using Entity.Prop;
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

        // Props buttons
        public Button ghostronSpawnButton;
        public Button pacboySpawnButton;
        public Button powerPelletButton;
        public Button fastWheelButton;
        public Button niceBombButton;
        public Button slowWheelButton;
        public Button badCherryButton;
        public Button luckyDiceButton;
        public Button removeButton;

        // Add/Minus buttons
        public Button ghostronSpawnAdd;
        public Button ghostronSpawnMinus;
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
        public TMP_Text ghostronCountText;
        public TMP_Text powerPelletCountText;
        public TMP_Text fastWheelCountText;
        public TMP_Text niceBombCountText;
        public TMP_Text slowWheelCountText;
        public TMP_Text badCherryCountText;
        public TMP_Text luckyDiceCountText;

        public GameObject propEditPanel; // Only active when a tile is selected
        public GameObject tileNotSelectedPrompt;

        private bool _propMode;

        private readonly Dictionary<Vector3, GameObject> _propObjectOnTiles = new(); // Prop on every tile

        // FIXED counts of all the props
        private Dictionary<PropType, int> _fixedPropCounts;

        // TOTAL counts of all the props - including FIXED and RANDOM ones
        private Dictionary<PropType, int> _totalPropCounts;

        private readonly Vector3 _gridStart = new(-15, 0, 15); // The top left corner
        private readonly float _gridSpacing = 3.0f; // The length of each tile
        private readonly int _gridSize = 11; // Map grid size

        public static PropEditor Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("TileChecker AWAKE");
            // Set singleton instance
            Instance = this;
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
            LoadPropTotalCount();
            TotalNumberButtonUpdate();
        }

        // Enters the prop editing mode. Used in MapEditor class.
        public void EnterPropMode() {
            _propMode = true;
            _tileSelected = false;

            propEditPanel.SetActive(false);
            tileNotSelectedPrompt.SetActive(true);
            PropsButtonInit();
        }

        /**
         * Quits the prop editing mode. Used in MapEditor class.
         * PARAM resetSelectedTile:
         *
         * - true (in most cases):
         * Set the tile that is last selected to normal material, aka cancel the highlight effect.
         *
         * - false (only used when showing invalid tile effect):
         * Do not do so. This is because this could override the invalid "red" effect
         * if the last selected tile happens to be invalid.
         */
        public void QuitPropMode(bool resetSelectedTile) {
            _propMode = false;

            // Change the material of the last tile back to the normal one
            // ONLY DO SO if resetSelectedTile is true 
            if (resetSelectedTile && _lastSelectedTile != null) {
                var renderer = _lastSelectedTile.GetComponent<Renderer>();
                if (renderer != null) renderer.material = tileNormalMaterial;
            }

            PropsButtonInit();
        }

        private void Start() {
            SetButtonActionListener();
        }

        // Load the total count information UI text for all props
        private void LoadPropTotalCount() {
            ghostronCountText.text = "Total:" + _totalPropCounts[PropType.Ghostron];
            powerPelletCountText.text = "Total:" + _totalPropCounts[PropType.PowerPellet];
            fastWheelCountText.text = "Total:" + _totalPropCounts[PropType.FastWheel];
            niceBombCountText.text = "Total:" + _totalPropCounts[PropType.NiceBomb];
            slowWheelCountText.text = "Total:" + _totalPropCounts[PropType.SlowWheel];
            badCherryCountText.text = "Total:" + _totalPropCounts[PropType.BadCherry];
            luckyDiceCountText.text = "Total:" + _totalPropCounts[PropType.LuckyDice];
        }

        // Handle mouse press
        void Update() {
            if (!_propMode) return;
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
                if (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) || _propObjectOnTiles[_selectedTileVector3] == null) {
                    Debug.Log("UI: This tile does not have a valid prop.");
                    PropMissingButtonUpdate();
                } else {
                    Debug.Log("UI: This tile have a valid prop.");
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
        private bool PlaceProp(PropType propType) {
            if (!_tileSelected) return false;

            // if (!_propObjectOnTiles.ContainsKey(_selectedTileVector3)) {
            //     _propObjectOnTiles[_selectedTileVector3] = new GameObject();
            // }

            if (propType == PropType.Pacboy && _fixedPropCounts[PropType.Pacboy] >= 1) {
                Debug.Log("WARNING Only one Pacboy Spawn Point is allowed.");
                return false;
            }

            if (propType != PropType.Pacboy && _fixedPropCounts[propType] > _totalPropCounts[propType]) {
                Debug.Log($"WARNING Maximum of: {propType} reached (defined by the total number).");
                return false;
            }

            GameObject prefab = GetPrefabByPropType(propType);
            if (prefab == null) {
                Debug.LogError($"WARNING Invalid prop type: {propType}");
                return false;
            }

            GameObject newProp = Instantiate(prefab, _selectedTileVector3, Quaternion.identity);
            _propObjectOnTiles[_selectedTileVector3] = newProp;
            _fixedPropCounts[propType]++;

            // UI update
            PropPlacedButtonUpdate();

            return true;
        }

        // Handle removals
        private bool RemoveProp() {
            Debug.Log("Removing");

            if (!_tileSelected) {
                Debug.LogError("NO TILE SELECTED");
                return false;
            }

            if (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) || _propObjectOnTiles[_selectedTileVector3] == null) {
                Debug.LogError("TILE CONTAINS NO PROPS OR NULL");
                return false;
            }

            GameObject propToRemove = _propObjectOnTiles[_selectedTileVector3];
            if (propToRemove != null) {
                Destroy(propToRemove);
                _propObjectOnTiles[_selectedTileVector3] = null;

                PropType propType = GetPropType(propToRemove);

                Debug.Log("Prop type to be removed: " + propType);

                // Update the fixed prop counts
                _fixedPropCounts[propType]--;

                // Enable the corresponding minus button if the fixed count is already less than total number
                if (propType != PropType.Pacboy && _fixedPropCounts[propType] < _totalPropCounts[propType]) {
                    switch (propType) {
                        case PropType.Ghostron:
                            ghostronSpawnMinus.gameObject.SetActive(true);
                            break;
                        case PropType.PowerPellet:
                            powerPelletMinus.gameObject.SetActive(true);
                            break;
                        case PropType.FastWheel:
                            fastWheelMinus.gameObject.SetActive(true);
                            break;
                        case PropType.NiceBomb:
                            niceBombMinus.gameObject.SetActive(true);
                            break;
                        case PropType.SlowWheel:
                            slowWheelMinus.gameObject.SetActive(true);
                            break;
                        case PropType.BadCherry:
                            badCherryMinus.gameObject.SetActive(true);
                            break;
                        case PropType.LuckyDice:
                            luckyDiceMinus.gameObject.SetActive(true);
                            break;
                        default:
                            Debug.LogError("Invalid prop type: " + propType);
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

        // Checks if the condition is satisfied for save & quit, returns:
        // true if all set
        // false if the Pacboy spawn point is not set
        public bool CheckCondition() {
            // Must have a Pacboy spawn point
            if (_fixedPropCounts[PropType.Pacboy] == 0) return false;
            return true;
        }

        /* On click operations */
        // Props buttons
        private void OnPacboySpawnButtonClick() {
            PlaceProp(PropType.Pacboy);
        }

        private void OnGhostronSpawnButtonClick() {
            if (!PlaceProp(PropType.Ghostron)) {
                Debug.LogError("GhostronSpawn place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts[PropType.Ghostron] == _totalPropCounts[PropType.Ghostron]) {
                ghostronSpawnMinus.gameObject.SetActive(false);
            }
        }

        private void OnPowerPelletButtonClick() {
            if (!PlaceProp(PropType.PowerPellet)) {
                Debug.LogError("PowerPellet place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts[PropType.PowerPellet] == _totalPropCounts[PropType.PowerPellet]) {
                powerPelletMinus.gameObject.SetActive(false);
            }
        }

        private void OnFastWheelButtonClick() {
            if (!PlaceProp(PropType.FastWheel)) {
                Debug.LogError("FastWheel place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts[PropType.FastWheel] == _totalPropCounts[PropType.FastWheel]) {
                fastWheelMinus.gameObject.SetActive(false);
            }
        }

        private void OnNiceBombButtonClick() {
            if (!PlaceProp(PropType.NiceBomb)) {
                Debug.LogError("NiceBomb place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts[PropType.NiceBomb] == _totalPropCounts[PropType.NiceBomb]) {
                niceBombMinus.gameObject.SetActive(false);
            }
        }

        private void OnSlowWheelButtonClick() {
            if (!PlaceProp(PropType.SlowWheel)) {
                Debug.LogError("SlowWheel place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts[PropType.SlowWheel] == _totalPropCounts[PropType.SlowWheel]) {
                slowWheelMinus.gameObject.SetActive(false);
            }
        }

        private void OnBadCherryButtonClick() {
            if (!PlaceProp(PropType.BadCherry)) {
                Debug.LogError("BadCherry place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts[PropType.BadCherry] == _totalPropCounts[PropType.BadCherry]) {
                badCherryMinus.gameObject.SetActive(false);
            }
        }

        private void OnLuckyDiceButtonClick() {
            if (!PlaceProp(PropType.LuckyDice)) {
                Debug.LogError("LuckyDice place error!");
                return;
            }

            // Check if the number of fixed ones has reached the total number
            // If so, disable the minus button
            if (_fixedPropCounts[PropType.LuckyDice] == _totalPropCounts[PropType.LuckyDice]) {
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
        private void OnGhostronSpawnAddClick() {
            if (_totalPropCounts[PropType.Ghostron] >= 5) {
                Debug.LogError("Ghostron add error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.Ghostron]++;
            ghostronCountText.text = "Total:" + _totalPropCounts[PropType.Ghostron];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts[PropType.Ghostron] > _fixedPropCounts[PropType.Ghostron]) {
                // ghostronSpawnButton.interactable = true;
                if (_tileSelected && (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) ||
                                      _propObjectOnTiles[_selectedTileVector3] == null)) {
                    ghostronSpawnButton.interactable = true;
                }
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts[PropType.Ghostron] == 5) {
                ghostronSpawnAdd.gameObject.SetActive(false);
            }

            // If already more than one ghost or the number of fixed ones, enable the minus button
            if (_totalPropCounts[PropType.Ghostron] == 2 ||
                _totalPropCounts[PropType.Ghostron] > _fixedPropCounts[PropType.Ghostron]) {
                ghostronSpawnMinus.gameObject.SetActive(true);
            }
        }

        private void OnGhostronSpawnMinusClick() {
            if (_totalPropCounts[PropType.Ghostron] <= 1) {
                Debug.LogError("Ghostron minus minus error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.Ghostron]--;
            ghostronCountText.text = "Total:" + _totalPropCounts[PropType.Ghostron];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts[PropType.Ghostron] == _fixedPropCounts[PropType.Ghostron]) {
                ghostronSpawnButton.interactable = false;
            }

            // If reaches the minimum number 1 or the number of fixed ones, disable the minus button
            if (_totalPropCounts[PropType.Ghostron] == 1 ||
                _totalPropCounts[PropType.Ghostron] == _fixedPropCounts[PropType.Ghostron]) {
                ghostronSpawnMinus.gameObject.SetActive(false);
            }

            // If already less than five ghosts, enable the add button
            if (_totalPropCounts[PropType.Ghostron] == 4) {
                ghostronSpawnAdd.gameObject.SetActive(true);
            }
        }

        private void OnPowerPelletAddClick() {
            if (_totalPropCounts[PropType.PowerPellet] >= 5) {
                Debug.LogError("Power pellet add error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.PowerPellet]++;
            powerPelletCountText.text = "Total:" + _totalPropCounts[PropType.PowerPellet];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts[PropType.PowerPellet] > _fixedPropCounts[PropType.PowerPellet]) {
                // powerPelletButton.interactable = true;
                if (_tileSelected && (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) ||
                                      _propObjectOnTiles[_selectedTileVector3] == null)) {
                    powerPelletButton.interactable = true;
                }
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts[PropType.PowerPellet] == 5) {
                powerPelletAdd.gameObject.SetActive(false);
            }

            // If already more than zero power pellet or the number of fixed ones, enable the minus button
            if (_totalPropCounts[PropType.PowerPellet] == 1 ||
                _totalPropCounts[PropType.PowerPellet] > _fixedPropCounts[PropType.PowerPellet]) {
                powerPelletMinus.gameObject.SetActive(true);
            }
        }

        private void OnPowerPelletMinusClick() {
            if (_totalPropCounts[PropType.PowerPellet] <= 0) {
                Debug.LogError("Power pellet minus error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.PowerPellet]--;
            powerPelletCountText.text = "Total:" + _totalPropCounts[PropType.PowerPellet];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts[PropType.PowerPellet] == _fixedPropCounts[PropType.PowerPellet]) {
                powerPelletButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts[PropType.PowerPellet] == 0 ||
                _totalPropCounts[PropType.PowerPellet] == _fixedPropCounts[PropType.PowerPellet]) {
                powerPelletMinus.gameObject.SetActive(false);
            }

            // If already less than five power pellets, enable the add button
            if (_totalPropCounts[PropType.PowerPellet] == 4) {
                powerPelletAdd.gameObject.SetActive(true);
            }
        }

        private void OnFastWheelAddClick() {
            if (_totalPropCounts[PropType.FastWheel] >= 5) {
                Debug.LogError("Fast wheel add error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.FastWheel]++;
            fastWheelCountText.text = "Total:" + _totalPropCounts[PropType.FastWheel];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts[PropType.FastWheel] > _fixedPropCounts[PropType.FastWheel]) {
                // fastWheelButton.interactable = true;
                if (_tileSelected && (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) ||
                                      _propObjectOnTiles[_selectedTileVector3] == null)) {
                    fastWheelButton.interactable = true;
                }
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts[PropType.FastWheel] == 5) {
                fastWheelAdd.gameObject.SetActive(false);
            }

            // If already more than zero fast wheel or the number of fixed ones, enable the minus button
            if (_totalPropCounts[PropType.FastWheel] == 1 || _totalPropCounts[PropType.FastWheel] > _fixedPropCounts[PropType.FastWheel]) {
                fastWheelMinus.gameObject.SetActive(true);
            }
        }

        private void OnFastWheelMinusClick() {
            if (_totalPropCounts[PropType.FastWheel] <= 0) {
                Debug.LogError("Fast wheel minus error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.FastWheel]--;
            fastWheelCountText.text = "Total:" + _totalPropCounts[PropType.FastWheel];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts[PropType.FastWheel] == _fixedPropCounts[PropType.FastWheel]) {
                fastWheelButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts[PropType.FastWheel] == 0 || _totalPropCounts[PropType.FastWheel] == _fixedPropCounts[PropType.FastWheel]) {
                fastWheelMinus.gameObject.SetActive(false);
            }

            // If already less than five fast wheels, enable the add button
            if (_totalPropCounts[PropType.FastWheel] == 4) {
                fastWheelAdd.gameObject.SetActive(true);
            }
        }

        private void OnNiceBombAddClick() {
            if (_totalPropCounts[PropType.NiceBomb] >= 5) {
                Debug.LogError("Nice bomb add error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.NiceBomb]++;
            niceBombCountText.text = "Total:" + _totalPropCounts[PropType.NiceBomb];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts[PropType.NiceBomb] > _fixedPropCounts[PropType.NiceBomb]) {
                // niceBombButton.interactable = true;
                if (_tileSelected && (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) ||
                                      _propObjectOnTiles[_selectedTileVector3] == null)) {
                    niceBombButton.interactable = true;
                }
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts[PropType.NiceBomb] == 5) {
                niceBombAdd.gameObject.SetActive(false);
            }

            // If already more than zero nice bomb or the number of fixed ones, enable the minus button
            if (_totalPropCounts[PropType.NiceBomb] == 1 || _totalPropCounts[PropType.NiceBomb] > _fixedPropCounts[PropType.NiceBomb]) {
                niceBombMinus.gameObject.SetActive(true);
            }
        }

        private void OnNiceBombMinusClick() {
            if (_totalPropCounts[PropType.NiceBomb] <= 0) {
                Debug.LogError("Nice bomb minus error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.NiceBomb]--;
            niceBombCountText.text = "Total:" + _totalPropCounts[PropType.NiceBomb];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts[PropType.NiceBomb] == _fixedPropCounts[PropType.NiceBomb]) {
                niceBombButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts[PropType.NiceBomb] == 0 || _totalPropCounts[PropType.NiceBomb] == _fixedPropCounts[PropType.NiceBomb]) {
                niceBombMinus.gameObject.SetActive(false);
            }

            // If already less than five nice bombs, enable the add button
            if (_totalPropCounts[PropType.NiceBomb] == 4) {
                niceBombAdd.gameObject.SetActive(true);
            }
        }

        private void OnSlowWheelAddClick() {
            if (_totalPropCounts[PropType.SlowWheel] >= 5) {
                Debug.LogError("Slow wheel add error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.SlowWheel]++;
            slowWheelCountText.text = "Total:" + _totalPropCounts[PropType.SlowWheel];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts[PropType.SlowWheel] > _fixedPropCounts[PropType.SlowWheel]) {
                // slowWheelButton.interactable = true;
                if (_tileSelected && (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) ||
                                      _propObjectOnTiles[_selectedTileVector3] == null)) {
                    slowWheelButton.interactable = true;
                }
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts[PropType.SlowWheel] == 5) {
                slowWheelAdd.gameObject.SetActive(false);
            }

            // If already more than zero slow wheel or the number of fixed ones, enable the minus button
            if (_totalPropCounts[PropType.SlowWheel] == 1 || _totalPropCounts[PropType.SlowWheel] > _fixedPropCounts[PropType.SlowWheel]) {
                slowWheelMinus.gameObject.SetActive(true);
            }
        }

        private void OnSlowWheelMinusClick() {
            if (_totalPropCounts[PropType.SlowWheel] <= 0) {
                Debug.LogError("Slow wheel minus error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.SlowWheel]--;
            slowWheelCountText.text = "Total:" + _totalPropCounts[PropType.SlowWheel];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts[PropType.SlowWheel] == _fixedPropCounts[PropType.SlowWheel]) {
                slowWheelButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts[PropType.SlowWheel] == 0 || _totalPropCounts[PropType.SlowWheel] == _fixedPropCounts[PropType.SlowWheel]) {
                slowWheelMinus.gameObject.SetActive(false);
            }

            // If already less than five slow wheels, enable the add button
            if (_totalPropCounts[PropType.SlowWheel] == 4) {
                slowWheelAdd.gameObject.SetActive(true);
            }
        }

        private void OnBadCherryAddClick() {
            if (_totalPropCounts[PropType.BadCherry] >= 5) {
                Debug.LogError("Bad cherry add error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.BadCherry]++;
            badCherryCountText.text = "Total:" + _totalPropCounts[PropType.BadCherry];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts[PropType.BadCherry] > _fixedPropCounts[PropType.BadCherry]) {
                // badCherryButton.interactable = true;
                if (_tileSelected && (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) ||
                                      _propObjectOnTiles[_selectedTileVector3] == null)) {
                    badCherryButton.interactable = true;
                }
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts[PropType.BadCherry] == 5) {
                badCherryAdd.gameObject.SetActive(false);
            }

            // If already more than zero bad cherry or the number of fixed ones, enable the minus button
            if (_totalPropCounts[PropType.BadCherry] == 1 || _totalPropCounts[PropType.BadCherry] > _fixedPropCounts[PropType.BadCherry]) {
                badCherryMinus.gameObject.SetActive(true);
            }
        }

        private void OnBadCherryMinusClick() {
            if (_totalPropCounts[PropType.BadCherry] <= 0) {
                Debug.LogError("Bad cherry minus error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.BadCherry]--;
            badCherryCountText.text = "Total:" + _totalPropCounts[PropType.BadCherry];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts[PropType.BadCherry] == _fixedPropCounts[PropType.BadCherry]) {
                badCherryButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts[PropType.BadCherry] == 0 || _totalPropCounts[PropType.BadCherry] == _fixedPropCounts[PropType.BadCherry]) {
                badCherryMinus.gameObject.SetActive(false);
            }

            // If already less than five bad cherries, enable the add button
            if (_totalPropCounts[PropType.BadCherry] == 4) {
                badCherryAdd.gameObject.SetActive(true);
            }
        }

        private void OnLuckyDiceAddClick() {
            if (_totalPropCounts[PropType.LuckyDice] >= 5) {
                Debug.LogError("Lucky dice add error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.LuckyDice]++;
            luckyDiceCountText.text = "Total:" + _totalPropCounts[PropType.LuckyDice];

            // If the total count is becoming more than the fixed count, enable the prop button
            if (_totalPropCounts[PropType.LuckyDice] > _fixedPropCounts[PropType.LuckyDice]) {
                // luckyDiceButton.interactable = true;
                if (_tileSelected && (!_propObjectOnTiles.ContainsKey(_selectedTileVector3) ||
                                      _propObjectOnTiles[_selectedTileVector3] == null)) {
                    luckyDiceButton.interactable = true;
                }
            }

            // If reaches the maximum number 5, disable the add button
            if (_totalPropCounts[PropType.LuckyDice] == 5) {
                luckyDiceAdd.gameObject.SetActive(false);
            }

            // If already more than zero lucky dice or the number of fixed ones, enable the minus button
            if (_totalPropCounts[PropType.LuckyDice] == 1 || _totalPropCounts[PropType.LuckyDice] > _fixedPropCounts[PropType.LuckyDice]) {
                luckyDiceMinus.gameObject.SetActive(true);
            }
        }

        private void OnLuckyDiceMinusClick() {
            if (_totalPropCounts[PropType.LuckyDice] <= 0) {
                Debug.LogError("Lucky dice minus error!");
                return;
            }

            // Update count
            _totalPropCounts[PropType.LuckyDice]--;
            luckyDiceCountText.text = "Total:" + _totalPropCounts[PropType.LuckyDice];

            // If the total count is already equal to the fixed count, disable the prop button
            if (_totalPropCounts[PropType.LuckyDice] == _fixedPropCounts[PropType.LuckyDice]) {
                luckyDiceButton.interactable = false;
            }

            // If reaches the minimum number 0 or the number of fixed ones, disable the minus button
            if (_totalPropCounts[PropType.LuckyDice] == 0 || _totalPropCounts[PropType.LuckyDice] == _fixedPropCounts[PropType.LuckyDice]) {
                luckyDiceMinus.gameObject.SetActive(false);
            }

            // If already less than five lucky dices, enable the add button
            if (_totalPropCounts[PropType.LuckyDice] == 4) {
                luckyDiceAdd.gameObject.SetActive(true);
            }
        }

        // Button action listener setting
        private void SetButtonActionListener() {
            ghostronSpawnButton.onClick.AddListener(OnGhostronSpawnButtonClick);
            pacboySpawnButton.onClick.AddListener(OnPacboySpawnButtonClick);
            powerPelletButton.onClick.AddListener(OnPowerPelletButtonClick);
            fastWheelButton.onClick.AddListener(OnFastWheelButtonClick);
            niceBombButton.onClick.AddListener(OnNiceBombButtonClick);
            slowWheelButton.onClick.AddListener(OnSlowWheelButtonClick);
            badCherryButton.onClick.AddListener(OnBadCherryButtonClick);
            luckyDiceButton.onClick.AddListener(OnLuckyDiceButtonClick);

            removeButton.onClick.AddListener(OnRemoveButtonClick);

            ghostronSpawnAdd.onClick.AddListener(OnGhostronSpawnAddClick);
            ghostronSpawnMinus.onClick.AddListener(OnGhostronSpawnMinusClick);
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
            ghostronSpawnButton.interactable = false;
            pacboySpawnButton.interactable = false;
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

            // Check every prop except for Pacboy spawn:
            // Only if its current total count is more than the fixed count can its button be enabled
            if (_totalPropCounts[PropType.Ghostron] > _fixedPropCounts[PropType.Ghostron]) {
                ghostronSpawnButton.interactable = true;
            }

            if (_totalPropCounts[PropType.PowerPellet] > _fixedPropCounts[PropType.PowerPellet]) {
                powerPelletButton.interactable = true;
            }

            if (_totalPropCounts[PropType.FastWheel] > _fixedPropCounts[PropType.FastWheel]) {
                fastWheelButton.interactable = true;
            }

            if (_totalPropCounts[PropType.NiceBomb] > _fixedPropCounts[PropType.NiceBomb]) {
                niceBombButton.interactable = true;
            }

            if (_totalPropCounts[PropType.SlowWheel] > _fixedPropCounts[PropType.SlowWheel]) {
                slowWheelButton.interactable = true;
            }

            if (_totalPropCounts[PropType.BadCherry] > _fixedPropCounts[PropType.BadCherry]) {
                badCherryButton.interactable = true;
            }

            if (_totalPropCounts[PropType.LuckyDice] > _fixedPropCounts[PropType.LuckyDice]) {
                luckyDiceButton.interactable = true;
            }

            // Pacboy spawn button logic
            if (_fixedPropCounts[PropType.Pacboy] == 0) {
                pacboySpawnButton.interactable = true;
            }

            Debug.Log("UI updated: Only prop buttons available");
        }

        // Update the status of the total number add/minus buttons
        private void TotalNumberButtonUpdate() {
            // 1 Add buttons
            // Reaching the maximum number 5

            if (_totalPropCounts[PropType.Ghostron] == 5) {
                ghostronSpawnAdd.gameObject.SetActive(false);
            } else {
                ghostronSpawnAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.PowerPellet] == 5) {
                powerPelletAdd.gameObject.SetActive(false);
            } else {
                powerPelletAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.FastWheel] == 5) {
                fastWheelAdd.gameObject.SetActive(false);
            } else {
                fastWheelAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.NiceBomb] == 5) {
                niceBombAdd.gameObject.SetActive(false);
            } else {
                niceBombAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.SlowWheel] == 5) {
                slowWheelAdd.gameObject.SetActive(false);
            } else {
                slowWheelAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.BadCherry] == 5) {
                badCherryAdd.gameObject.SetActive(false);
            } else {
                badCherryAdd.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.LuckyDice] == 5) {
                luckyDiceAdd.gameObject.SetActive(false);
            } else {
                luckyDiceAdd.gameObject.SetActive(true);
            }

            // 2 Minus buttons
            // 2.1 At their minimum numbers (1 for ghost spawn and 0 for others)

            if (_totalPropCounts[PropType.Ghostron] == 1) {
                ghostronSpawnMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts[PropType.PowerPellet] == 0) {
                powerPelletMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts[PropType.FastWheel] == 0) {
                fastWheelMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts[PropType.NiceBomb] == 0) {
                niceBombMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts[PropType.SlowWheel] == 0) {
                slowWheelMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts[PropType.BadCherry] == 0) {
                badCherryMinus.gameObject.SetActive(false);
            }

            if (_totalPropCounts[PropType.LuckyDice] == 0) {
                luckyDiceMinus.gameObject.SetActive(false);
            }

            // 2.2 Equal to their fixed prop numbers

            if (_totalPropCounts[PropType.Ghostron] == _fixedPropCounts[PropType.Ghostron]) {
                ghostronSpawnMinus.gameObject.SetActive(false);
            } else {
                ghostronSpawnMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.PowerPellet] == _fixedPropCounts[PropType.PowerPellet]) {
                powerPelletMinus.gameObject.SetActive(false);
            } else {
                powerPelletMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.FastWheel] == _fixedPropCounts[PropType.FastWheel]) {
                fastWheelMinus.gameObject.SetActive(false);
            } else {
                fastWheelMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.NiceBomb] == _fixedPropCounts[PropType.NiceBomb]) {
                niceBombMinus.gameObject.SetActive(false);
            } else {
                niceBombMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.SlowWheel] == _fixedPropCounts[PropType.SlowWheel]) {
                slowWheelMinus.gameObject.SetActive(false);
            } else {
                slowWheelMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.BadCherry] == _fixedPropCounts[PropType.BadCherry]) {
                badCherryMinus.gameObject.SetActive(false);
            } else {
                badCherryMinus.gameObject.SetActive(true);
            }

            if (_totalPropCounts[PropType.LuckyDice] == _fixedPropCounts[PropType.LuckyDice]) {
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
            ghostronSpawnButton.interactable = false;
            pacboySpawnButton.interactable = false;
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
            // Transform from Dictionary<Vector3, GameObject> to Dictionary<Vector3, PropType>
            Dictionary<Vector3, PropType> propOnTiles = new Dictionary<Vector3, PropType>();

            foreach (var kvp in _propObjectOnTiles) {
                propOnTiles.Add(kvp.Key, GetPropType(kvp.Value));
            }
            
            // Construct a new PropData
            return new PropData(propOnTiles, _fixedPropCounts, _totalPropCounts);
        }
        
        /**
         * Obtains the corresponding PropType name by GameObject name.
         */
        private PropType GetPropType(GameObject prop) {
            string propName = CleanName(prop.name);
            if (propName.Contains(nameof(PropType.Pacboy))) return PropType.Pacboy;
            if (propName.Contains(nameof(PropType.Ghostron))) return PropType.Ghostron; // Or a more specific Ghostron type
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
    }
}