using System.Collections.Generic;
using Entity.Map;
using UnityEngine;

namespace MapEditor {
    public class TileChecker : MonoBehaviour {
        // Materials
        public Material tileNormalMaterial;
        public Material tileErrorMaterial;

        private GameObject[,] _allTileGameObjects;

        // Four directions: up, down, left, right
        private readonly int[] _dx = { -1, 1, 0, 0 };
        private readonly int[] _dy = { 0, 0, -1, 1 };

        public bool invalidTilesDisplaying;

        // Singleton instance
        public static TileChecker Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("TileChecker AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            // By default, no invalid tiles are being displayed
            invalidTilesDisplaying = false;
            
            // Initialise the tile game object arrays
            _allTileGameObjects = new GameObject[11, 11];

            // Find the Floor root game object
            GameObject floorRoot = GameObject.Find("Floor");
            
            // Load tile game objects
            for (int column = 0; column < 11; column++) {
                string tileColumnName = $"FloorColumn{column + 1}";
                Transform tileColumn = floorRoot.transform.Find(tileColumnName);
                if (tileColumn == null) {
                    Debug.LogError($"Error: Not found {tileColumnName}");
                    continue;
                }

                for (int row = 0; row < 11; row++) {
                    string tileName = $"BlockPlane{row + 1}";
                    Transform tile = tileColumn.Find(tileName);
                    if (tile == null) {
                        Debug.LogError($"Error: Not found {tileColumnName}/{tileName}");
                        continue;
                    }

                    _allTileGameObjects[row, column] = tile.gameObject;
                }
            }
        }

        /**
         * Check the legality of all the tiles:
         * Reachability, Ease of Reachability, No dead end
         * Called by MapEditor when the player attempts to Save & Quit.
         * Returns:
         * The list of all invalid tiles.
         * If all tiles are valid, the list is null.
         */
        public bool CheckTileLegality(WallData wallData) {
            // Creating a 11x11 array of access states
            bool[,] visited = new bool[11, 11];
            int[,] distance = new int[11, 11]; // Record the shortest distance for each grid

            // Store tiles that are inaccessible or at a distance greater than 22
            List<(int, int)> invalidTiles = new List<(int, int)>();

            // Breadth-first search (BFS) queue
            // initialised by adding the centre (5, 5) to the queue
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((5, 5));
            visited[5, 5] = true;
            distance[5, 5] = 0;

            // BFS Search
            while (queue.Count > 0) {
                var (x, y) = queue.Dequeue();

                // If the current distance is greater than 22, it is marked as unreachable
                if (distance[x, y] > 22) {
                    invalidTiles.Add((x, y));
                    continue;
                }

                // Get horizontal wall status and vertical wall status from wall data
                bool[,] horizontalWallStatus = wallData.HorizontalWallStatus;
                bool[,] verticalWallStatus = wallData.VerticalWallStatus;

                // Check the num of its neighbours, if there's only one, it is a dead end tile
                int neighboursCount = 0;
                for (int i = 0; i < 4; i++) {
                    int nx = x + _dx[i];
                    int ny = y + _dy[i];

                    if (nx >= 0 && nx < 11 && ny >= 0 && ny < 11) {
                        if (i == 0 && x > nx && !horizontalWallStatus[nx, y]) // Wall above
                            neighboursCount++;
                        if (i == 1 && x < nx && !horizontalWallStatus[x, y]) // Wall below
                            neighboursCount++;
                        if (i == 2 && y > ny && !verticalWallStatus[x, ny]) // Wall at the left
                            neighboursCount++;
                        if (i == 3 && y < ny && !verticalWallStatus[x, y]) // Wall at the right
                            neighboursCount++;
                    }
                }

                // If there is only one neighbour, then it is invalid
                if (neighboursCount < 2) {
                    invalidTiles.Add((x, y));
                }

                // Check four directions (up, down, left, right)
                for (int i = 0; i < 4; i++) {
                    int nx = x + _dx[i];
                    int ny = y + _dy[i];

                    // Check if the new location is within the map
                    if (nx >= 0 && nx < 11 && ny >= 0 && ny < 11 && !visited[nx, ny]) {
                        // Check if the new tile is reachable from this tile
                        bool isBlocked = false;
                        if (i == 0 && x > nx && horizontalWallStatus[nx, y]) // Wall above
                            isBlocked = true;
                        if (i == 1 && x < nx && horizontalWallStatus[x, y]) // Wall below
                            isBlocked = true;
                        if (i == 2 && y > ny && verticalWallStatus[x, ny]) // Wall at the left
                            isBlocked = true;
                        if (i == 3 && y < ny && verticalWallStatus[x, y]) // Wall at the right
                            isBlocked = true;

                        // If not blocked by a wall, it is accessible
                        if (!isBlocked) {
                            visited[nx, ny] = true;
                            distance[nx, ny] = distance[x, y] + 1;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }

            // Iterate over all the tiles
            // and find the ones that are inaccessible, dead ends, or have a distance greater than 22
            for (int i = 0; i < 11; i++) {
                for (int j = 0; j < 11; j++) {
                    if (!visited[i, j] || distance[i, j] > 22) {
                        invalidTiles.Add((i, j));
                    }
                }
            }

            if (invalidTiles.Count > 0) {
                // If invalid tiles exist
                // Display them & return false to stop the Save & Quit logic
                DisplayInvalidTiles(invalidTiles);
                return false;
            }

            // All tiles valid, return true
            return true;
        }


        /**
         * Displays invalid tiles (using error materials).
         * Called by Map Editor when unreachable tiles are detected.
         */
        private void DisplayInvalidTiles(List<(int, int)> invalidTileCoordinates) {
            // Null check
            if (invalidTileCoordinates == null) {
                Debug.LogError("Error: Invalid tile coordinates list is null!");
                return;
            }

            // For debug use
            Debug.Log("The following tiles are invalid:");
            foreach (var tile in invalidTileCoordinates) {
                Debug.Log($"({tile.Item1}, {tile.Item2})");
            }

            // Display all of them in the map editor scene
            foreach (var coordinate in invalidTileCoordinates) {
                // Get Row No. and Column No.
                int row = coordinate.Item1;
                int column = coordinate.Item2;

                // Change them into error material (red)
                _allTileGameObjects[row, column].GetComponent<MeshRenderer>().material = tileErrorMaterial;
            }
        }

        /**
         * Resets the colour of all the tiles.
         * (Set all tiles' materials to normal one)
         */
        public void ClearTileDisplay() {
            if (!invalidTilesDisplaying) {
                return;
            }

            foreach (var tile in _allTileGameObjects) {
                tile.GetComponent<MeshRenderer>().material = tileNormalMaterial;
            }

            invalidTilesDisplaying = false;
        }
    }
}