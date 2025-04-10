using System.Collections.Generic;
using Entity.Map;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Manages the wall editor.
 */
public class WallEditor : MonoBehaviour {
    private MeshRenderer _previousMeshRenderer;
    private bool _previousStatus;

    private bool[,] _horizontalWallStatus = new bool[10, 11];
    private bool[,] _verticalWallStatus = new bool[11, 10];

    public Material normalMaterial; // Existing (physical) wall default material
    public Material highlightMaterial; // Existing wall highlight material
    public Material missingMaterial; // Missing wall default material
    public Material ghostMaterial; // Missing wall highlight material

    public GameObject[] horizontalWalls1;
    public GameObject[] horizontalWalls2;
    public GameObject[] horizontalWalls3;
    public GameObject[] horizontalWalls4;
    public GameObject[] horizontalWalls5;
    public GameObject[] horizontalWalls6;
    public GameObject[] horizontalWalls7;
    public GameObject[] horizontalWalls8;
    public GameObject[] horizontalWalls9;
    public GameObject[] horizontalWalls10;

    public GameObject[] verticalWalls1;
    public GameObject[] verticalWalls2;
    public GameObject[] verticalWalls3;
    public GameObject[] verticalWalls4;
    public GameObject[] verticalWalls5;
    public GameObject[] verticalWalls6;
    public GameObject[] verticalWalls7;
    public GameObject[] verticalWalls8;
    public GameObject[] verticalWalls9;
    public GameObject[] verticalWalls10;
    public GameObject[] verticalWalls11;

    private bool _wallMode;

    private Dictionary<GameObject, (bool isHorizontal, int row, int column)> _wallLookup = new();

    // Start is called before the first frame update
    void Start() {
        _wallMode = false;
    }

    public void SetWallData(WallData wallData) {
        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[0, column] = wallData.HorizontalWallStatus[0, column];
            _wallLookup[horizontalWalls1[column]] = (true, 0, column);
            
            // Set the wall material on the map
            horizontalWalls1[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[0, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[1, column] = wallData.HorizontalWallStatus[1, column];
            _wallLookup[horizontalWalls2[column]] = (true, 1, column);
            
            // Set the wall material on the map
            horizontalWalls2[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[1, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[2, column] = wallData.HorizontalWallStatus[2, column];
            _wallLookup[horizontalWalls3[column]] = (true, 2, column);
            
            // Set the wall material on the map
            horizontalWalls3[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[2, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[3, column] = wallData.HorizontalWallStatus[3, column];
            _wallLookup[horizontalWalls4[column]] = (true, 3, column);
            
            // Set the wall material on the map
            horizontalWalls4[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[3, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[4, column] = wallData.HorizontalWallStatus[4, column];
            _wallLookup[horizontalWalls5[column]] = (true, 4, column);
            
            // Set the wall material on the map
            horizontalWalls5[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[4, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[5, column] = wallData.HorizontalWallStatus[5, column];
            _wallLookup[horizontalWalls6[column]] = (true, 5, column);
            
            // Set the wall material on the map
            horizontalWalls6[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[5, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[6, column] = wallData.HorizontalWallStatus[6, column];
            _wallLookup[horizontalWalls7[column]] = (true, 6, column);
            
            // Set the wall material on the map
            horizontalWalls7[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[6, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[7, column] = wallData.HorizontalWallStatus[7, column];
            _wallLookup[horizontalWalls8[column]] = (true, 7, column);
            
            // Set the wall material on the map
            horizontalWalls8[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[7, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[8, column] = wallData.HorizontalWallStatus[8, column];
            _wallLookup[horizontalWalls9[column]] = (true, 8, column);
            
            // Set the wall material on the map
            horizontalWalls9[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[8, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 11; column++) {
            _horizontalWallStatus[9, column] = wallData.HorizontalWallStatus[9, column];
            _wallLookup[horizontalWalls10[column]] = (true, 9, column);
            
            // Set the wall material on the map
            horizontalWalls10[column].GetComponent<MeshRenderer>().material =
                _horizontalWallStatus[9, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[0, column] = wallData.VerticalWallStatus[0, column];
            _wallLookup[verticalWalls1[column]] = (false, 0, column);
            
            // Set the wall material on the map
            verticalWalls1[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[0, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[1, column] = wallData.VerticalWallStatus[1, column];
            _wallLookup[verticalWalls2[column]] = (false, 1, column);
            
            // Set the wall material on the map
            verticalWalls2[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[1, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[2, column] = wallData.VerticalWallStatus[2, column];
            _wallLookup[verticalWalls3[column]] = (false, 2, column);
            
            // Set the wall material on the map
            verticalWalls3[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[2, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[3, column] = wallData.VerticalWallStatus[3, column];
            _wallLookup[verticalWalls4[column]] = (false, 3, column);
            
            // Set the wall material on the map
            verticalWalls4[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[3, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[4, column] = wallData.VerticalWallStatus[4, column];
            _wallLookup[verticalWalls5[column]] = (false, 4, column);
            
            // Set the wall material on the map
            verticalWalls5[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[4, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[5, column] = wallData.VerticalWallStatus[5, column];
            _wallLookup[verticalWalls6[column]] = (false, 5, column);
            
            // Set the wall material on the map
            verticalWalls6[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[5, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[6, column] = wallData.VerticalWallStatus[6, column];
            _wallLookup[verticalWalls7[column]] = (false, 6, column);
            
            // Set the wall material on the map
            verticalWalls7[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[6, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[7, column] = wallData.VerticalWallStatus[7, column];
            _wallLookup[verticalWalls8[column]] = (false, 7, column);
            
            // Set the wall material on the map
            verticalWalls8[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[7, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[8, column] = wallData.VerticalWallStatus[8, column];
            _wallLookup[verticalWalls9[column]] = (false, 8, column);
            
            // Set the wall material on the map
            verticalWalls9[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[8, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[9, column] = wallData.VerticalWallStatus[9, column];
            _wallLookup[verticalWalls10[column]] = (false, 9, column);
            
            // Set the wall material on the map
            verticalWalls10[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[9, column] ? normalMaterial : missingMaterial;
        }

        for (int column = 0; column < 10; column++) {
            _verticalWallStatus[10, column] = wallData.VerticalWallStatus[10, column];
            _wallLookup[verticalWalls11[column]] = (false, 10, column);
            
            // Set the wall material on the map
            verticalWalls11[column].GetComponent<MeshRenderer>().material =
                _verticalWallStatus[10, column] ? normalMaterial : missingMaterial;
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