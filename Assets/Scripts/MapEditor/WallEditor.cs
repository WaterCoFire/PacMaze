using System.Collections.Generic;
using Entity.Map;
using Sound;
using UnityEngine;
using UnityEngine.UI;


namespace MapEditor {
    /**
     * Manages the wall editor.
     */
    public class WallEditor : MonoBehaviour {
        private MeshRenderer _previousMeshRenderer;
        private bool _previousStatus;

        // Current status of horizontal walls and vertical walls
        private readonly bool[,] _horizontalWallStatus = new bool[10, 11];
        private readonly bool[,] _verticalWallStatus = new bool[11, 10];
        
        // Random Generation Button
        public Button randomGenerationButton;
        
        // Clear All Walls Button
        public Button clearAllWallsButton;

        public Material normalMaterial; // Existing (physical) wall default material
        public Material highlightMaterial; // Existing wall highlight material
        public Material missingMaterial; // Missing wall default material
        public Material ghostMaterial; // Missing wall highlight material

        // Wall GameObject arrays
        private GameObject[,] _horizontalWalls;
        private GameObject[,] _verticalWalls;

        private bool _wallMode;

        // Dictionary for looking up a wall by GameObject
        private readonly Dictionary<GameObject, (bool isHorizontal, int row, int column)> _wallLookup = new();

        // Singleton instance
        public static WallEditor Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        void Start() {
            _wallMode = false;

            // Set button action listeners
            SetButtonActionListener();

            // Initialise the two wall game object arrays
            _horizontalWalls = new GameObject[10, 11];
            _verticalWalls = new GameObject[11, 10];

            // Find the Walls root game object
            GameObject wallsRoot = GameObject.Find("Walls");

            // Load horizontal walls
            for (int row = 0; row < 10; row++) {
                string hRowName = $"HWall_Row_{row + 1}";
                Transform hRow = wallsRoot.transform.Find(hRowName);
                if (hRow == null) {
                    Debug.LogError($"Error: Not found {hRowName}");
                    continue;
                }

                for (int col = 0; col < 11; col++) {
                    string hWallName = $"HWall_{col + 1}";
                    Transform hWall = hRow.Find(hWallName);
                    if (hWall == null) {
                        Debug.LogError($"Error: Not found {hRowName}/{hWallName}");
                        continue;
                    }

                    _horizontalWalls[row, col] = hWall.gameObject;
                }
            }

            // Load vertical walls
            for (int col = 0; col < 10; col++) {
                string vColName = $"VWall_Column_{col + 1}";
                Transform vCol = wallsRoot.transform.Find(vColName);
                if (vCol == null) {
                    Debug.LogError($"Error: Not found {vColName}");
                    continue;
                }

                for (int row = 0; row < 11; row++) {
                    string vWallName = $"VWall_{row + 1}";
                    Transform vWall = vCol.Find(vWallName);
                    if (vWall == null) {
                        Debug.LogError($"Error: Not found {vColName}/{vWallName}");
                        continue;
                    }

                    _verticalWalls[row, col] = vWall.gameObject;
                }
            }
        }

        /**
         * Sets the prop data. Used in MapEditor class when initialising Map Editor.
         * Information to be set: horizontalWallStatus, verticalWallStatus
         */
        public void SetWallData(WallData wallData) {
            // Horizontal walls
            for (int row = 0; row < 10; row++) {
                for (int column = 0; column < 11; column++) {
                    _horizontalWallStatus[row, column] = wallData.HorizontalWallStatus[row, column];
                    _wallLookup[_horizontalWalls[row, column]] = (true, row, column);

                    // Set the wall material on the map
                    _horizontalWalls[row, column].GetComponent<MeshRenderer>().material =
                        _horizontalWallStatus[row, column] ? normalMaterial : missingMaterial;
                }
            }

            // Vertical walls
            for (int row = 0; row < 11; row++) {
                for (int column = 0; column < 10; column++) {
                    _verticalWallStatus[row, column] = wallData.VerticalWallStatus[row, column];
                    _wallLookup[_verticalWalls[row, column]] = (false, row, column);

                    // Set the wall material on the map
                    _verticalWalls[row, column].GetComponent<MeshRenderer>().material =
                        _verticalWallStatus[row, column] ? normalMaterial : missingMaterial;
                }
            }
        }

        // Enters/Quits the wall editing mode. Used in MapEditor class.
        public void SetWallMode(bool enter) {
            if (enter) {
                // Enter
                _wallMode = true;
                _previousMeshRenderer = null;
            } else {
                // Quit
                _wallMode = false;

                // Set tile material to normal
                TileChecker.Instance.ClearTileDisplay();

                // Set the material of the previous selected wall
                if (_previousMeshRenderer != null) {
                    _previousMeshRenderer.material = _previousStatus ? normalMaterial : missingMaterial;
                }
            }
        }

        // Update is called once per frame
        void Update() {
            if (!_wallMode) return;
            HandleMouseInput();
        }

        /**
         * Monitors the mouse behaviours.
         * For updating wall highlight effects or layout setting.
         */
        private void HandleMouseInput() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (_wallLookup.TryGetValue(hit.collider.gameObject, out var wallInfo)) {
                    GameObject wall = hit.collider.gameObject;
                    bool isHorizontal = wallInfo.isHorizontal;
                    int row = wallInfo.row;
                    int column = wallInfo.column;

                    SetWallHighlight(wall, isHorizontal, row, column);

                    if (Input.GetMouseButtonDown(0)) {
                        ToggleWall(wall, isHorizontal, row, column);
                    }
                }
            }
        }

        /**
         * Sets the highlight effect of a wall.
         */
        private void SetWallHighlight(GameObject wall, bool isHorizontal, int row, int column) {
            MeshRenderer renderer = wall.GetComponent<MeshRenderer>();

            if (_previousMeshRenderer != null && _previousMeshRenderer != renderer) {
                _previousMeshRenderer.material = _previousStatus ? normalMaterial : missingMaterial;
            }

            bool wallExists = isHorizontal ? _horizontalWallStatus[row, column] : _verticalWallStatus[row, column];

            if (wallExists) {
                // Debug.Log("EXIST");
                renderer.material = highlightMaterial; // Existing wall - highlight
                _previousStatus = true;
            } else {
                // Debug.Log("NOT EXIST");
                renderer.material = ghostMaterial; // Missing wall - highlight (ghost)
                _previousStatus = false;
            }

            _previousMeshRenderer = renderer;
        }

        /**
         * Action when player left-clicks on a wall.
         * Updates its status.
         */
        private void ToggleWall(GameObject wall, bool isHorizontal, int row, int column) {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Reset all tile materials if currently some invalid (unreachable) tiles are being displayed
            if (TileChecker.Instance.invalidTilesDisplaying) {
                TileChecker.Instance.ClearTileDisplay();
            }

            bool newState = !(isHorizontal ? _horizontalWallStatus[row, column] : _verticalWallStatus[row, column]);

            if (isHorizontal) {
                _horizontalWallStatus[row, column] = newState;
            } else {
                _verticalWallStatus[row, column] = newState;
            }

            MeshRenderer renderer = wall.GetComponent<MeshRenderer>();

            renderer.material = newState ? normalMaterial : ghostMaterial;

            _previousStatus = newState;

            // NOTICE - Missing wall is not completely inactive!
        }

        /**
         * Obtains the data about the walls.
         * Called in MapEditor.
         */
        public WallData GetWallData() {
            return new WallData(_horizontalWallStatus, _verticalWallStatus);
        }

        /**
         * Sets the action listeners of all the buttons.
         * (Random Generation & Clear All Walls)
         */
        private void SetButtonActionListener() {
            randomGenerationButton.onClick.AddListener(OnRandomGenerationButtonClick);
            clearAllWallsButton.onClick.AddListener(OnClearAllWallsButtonClick);
        }
        
        /* Action Listeners */
        // Random Generation Button
        private void OnRandomGenerationButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Obtain a random wall layout
            WallData randomWallData = RandomLayoutGenerator.Instance.GenerateWallLayout();
            
            // Update the wall data
            SetWallData(randomWallData);
        }

        // Clear All Walls Button
        private void OnClearAllWallsButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            _previousMeshRenderer = null;
            // Let all walls display the missing material
            foreach (var horizontalWall in _horizontalWalls) {
                horizontalWall.GetComponent<MeshRenderer>().material = missingMaterial;
            }
            
            foreach (var verticalWall in _verticalWalls) {
                verticalWall.GetComponent<MeshRenderer>().material = missingMaterial;
            }
            
            // Set all wall status to false (absent)
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 11; j++) {
                    _horizontalWallStatus[i, j] = false;
                }
            }
            
            for (int i = 0; i < 11; i++) {
                for (int j = 0; j < 10; j++) {
                    _verticalWallStatus[i, j] = false;
                }
            }
        }
    }
}