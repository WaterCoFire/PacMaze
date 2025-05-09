using System.Collections.Generic;
using Entity.Map;
using UnityEngine;


namespace MapEditor {
    /**
     * Manages the wall editor.
     */
    public class WallEditor : MonoBehaviour {
        private MeshRenderer _previousMeshRenderer;
        private bool _previousStatus;

        private readonly bool[,] _horizontalWallStatus = new bool[10, 11];
        private readonly bool[,] _verticalWallStatus = new bool[11, 10];

        public Material normalMaterial; // Existing (physical) wall default material
        public Material highlightMaterial; // Existing wall highlight material
        public Material missingMaterial; // Missing wall default material
        public Material ghostMaterial; // Missing wall highlight material

        public GameObject[,] HorizontalWalls;
        public GameObject[,] VerticalWalls;

        private bool _wallMode;

        private readonly Dictionary<GameObject, (bool isHorizontal, int row, int column)> _wallLookup = new();

        // Singleton instance
        public static WallEditor Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("TileChecker AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // Start is called before the first frame update
        void Start() {
            _wallMode = false;

            // Initialise the two wall game object arrays
            HorizontalWalls = new GameObject[10, 11];
            VerticalWalls = new GameObject[11, 10];

            // Find the Walls root game object
            GameObject wallsRoot = GameObject.Find("Walls");
            Debug.Log(wallsRoot);

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

                    HorizontalWalls[row, col] = hWall.gameObject;
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

                    VerticalWalls[row, col] = vWall.gameObject;
                }
            }
        }

        public void SetWallData(WallData wallData) {
            // Horizontal walls
            for (int row = 0; row < 10; row++) {
                for (int column = 0; column < 11; column++) {
                    _horizontalWallStatus[row, column] = wallData.HorizontalWallStatus[row, column];
                    _wallLookup[HorizontalWalls[row, column]] = (true, row, column);

                    // Set the wall material on the map
                    HorizontalWalls[row, column].GetComponent<MeshRenderer>().material =
                        _horizontalWallStatus[row, column] ? normalMaterial : missingMaterial;
                }
            }

            // Vertical walls
            for (int row = 0; row < 11; row++) {
                for (int column = 0; column < 10; column++) {
                    _verticalWallStatus[row, column] = wallData.VerticalWallStatus[row, column];
                    _wallLookup[VerticalWalls[row, column]] = (false, row, column);

                    // Set the wall material on the map
                    VerticalWalls[row, column].GetComponent<MeshRenderer>().material =
                        _verticalWallStatus[row, column] ? normalMaterial : missingMaterial;
                }
            }
        }

        // Enters the wall editing mode. Used in MapEditor class.
        public void EnterWallMode() {
            _wallMode = true;
            _previousMeshRenderer = null;
        }

        // Quits the prop editing mode. Used in MapEditor class.
        public void QuitWallMode() {
            _wallMode = false;

            // Set tile material to normal
            TileChecker.Instance.ClearTileDisplay();

            // Set the material of the previous selected wall
            if (_previousMeshRenderer != null) {
                _previousMeshRenderer.material = _previousStatus ? normalMaterial : missingMaterial;
            }
        }

        // Update is called once per frame
        void Update() {
            if (!_wallMode) return;
            HandleMouseInput();
        }

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

        private void ToggleWall(GameObject wall, bool isHorizontal, int row, int column) {
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
    }
}