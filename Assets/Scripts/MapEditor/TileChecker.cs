using System.Collections.Generic;
using Entity.Map;
using UnityEngine;
using UnityEngine.Serialization;

namespace MapEditor {
    public class TileChecker : MonoBehaviour {
        // Materials
        public Material tileNormalMaterial;
        public Material tileErrorMaterial;

        // All tile game objects
        public GameObject[] tileRow1;
        public GameObject[] tileRow2;
        public GameObject[] tileRow3;
        public GameObject[] tileRow4;
        public GameObject[] tileRow5;
        public GameObject[] tileRow6;
        public GameObject[] tileRow7;
        public GameObject[] tileRow8;
        public GameObject[] tileRow9;
        public GameObject[] tileRow10;
        public GameObject[] tileRow11;

        // Four directions: up, down, left, right
        private readonly int[] _dx = { -1, 1, 0, 0 };
        private readonly int[] _dy = { 0, 0, -1, 1 };

        private readonly List<GameObject[]> _allTiles = new();

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

            // Update the list storing all tile game objects
            _allTiles.Clear();
            _allTiles.Add(tileRow1);
            _allTiles.Add(tileRow2);
            _allTiles.Add(tileRow3);
            _allTiles.Add(tileRow4);
            _allTiles.Add(tileRow5);
            _allTiles.Add(tileRow6);
            _allTiles.Add(tileRow7);
            _allTiles.Add(tileRow8);
            _allTiles.Add(tileRow9);
            _allTiles.Add(tileRow10);
            _allTiles.Add(tileRow11);
        }

        /**
         * Check the reachability of all the tiles.
         * Called by MapEditor when the player attempts to Save & Quit.
         * Returns:
         * The list of all invalid tiles.
         * If all tiles are valid, the list is null.
         */
        public bool CheckTileAccessibility(WallData wallData) {
            // Creating an 11x11 array of access states
            bool[,] visited = new bool[11, 11];
            int[,] distance = new int[11, 11]; // Record the shortest distance for each grid

            // Store tiles that are inaccessible or at a distance greater than 22
            List<(int, int)> unreachableTiles = new List<(int, int)>();

            // Breadth-first search (BFS) queue
            // initialised by adding the center (5, 5) (aka x = 6, y = 6) to the queue
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((5, 5));
            visited[5, 5] = true;
            distance[5, 5] = 0;

            // BFS Search
            while (queue.Count > 0) {
                var (x, y) = queue.Dequeue();

                // If the current distance is greater than 22, it is marked as unreachable
                if (distance[x, y] > 22) {
                    unreachableTiles.Add((x, y));
                    continue;
                }

                // Get horizontal wall status and vertical wall status from wall data
                bool[,] horizontalWallStatus = wallData.HorizontalWallStatus;
                bool[,] verticalWallStatus = wallData.VerticalWallStatus;

                // Check four directions (up, down, left, right)
                for (int i = 0; i < 4; i++) {
                    int nx = x + _dx[i];
                    int ny = y + _dy[i];

                    // Check if the new location is within the map
                    if (nx >= 0 && nx < 11 && ny >= 0 && ny < 11 && !visited[nx, ny]) {
                        // Determine if a wall is in the way
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
            // and find the ones that are inaccessible or have a distance greater than 22
            for (int i = 0; i < 11; i++) {
                for (int j = 0; j < 11; j++) {
                    if (!visited[i, j] || distance[i, j] > 22) {
                        unreachableTiles.Add((i, j));
                    }
                }
            }

            if (unreachableTiles.Count > 0) {
                // If invalid tiles exist
                // Display them & return false to stop the Save & Quit logic
                DisplayInvalidTiles(unreachableTiles);
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
            Debug.Log("The following tiles are inaccessible or the path exceeds 22:");
            foreach (var tile in invalidTileCoordinates) {
                Debug.Log($"({tile.Item1}, {tile.Item2})");
            }

            // Display all of them in the map editor scene
            foreach (var coordinate in invalidTileCoordinates) {
                // Get Row No. and Column No.
                int row = coordinate.Item1;
                int column = coordinate.Item2;

                // Change them into error material (red)
                _allTiles[row][column].GetComponent<MeshRenderer>().material = tileErrorMaterial;
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

            foreach (var tileRow in _allTiles) {
                foreach (var tile in tileRow) {
                    tile.GetComponent<MeshRenderer>().material = tileNormalMaterial;
                }
            }

            invalidTilesDisplaying = false;
        }

        // /* TEST TEST TEST TEST TEST ONLY!!!!!!! */
        //     // Initialise the wall state (all walls do not "have" walls)
        //     bool[,] horizontalWalls = new bool[10, 11]; // HW: 10 rows, 11 walls per row
        //     bool[,] verticalWalls = new bool[11, 10]; // VW: 11 rows, 10 walls per row
        //     
        //     // Set some walls as "having" walls
        //     horizontalWalls[5, 5] = true; // Horizontal wall at (6, 6)
        //     horizontalWalls[9, 1] = true;
        //     horizontalWalls[9, 2] = true;
        //     horizontalWalls[9, 3] = true;
        //     horizontalWalls[9, 4] = true;
        //     horizontalWalls[9, 5] = true;
        //     horizontalWalls[9, 6] = true;
        //     
        //     verticalWalls[1, 0] = true;
        //     verticalWalls[2, 0] = true;
        //     verticalWalls[3, 0] = true;
        //     verticalWalls[4, 0] = true;
        //     verticalWalls[5, 0] = true;
        //     verticalWalls[6, 0] = true;
        //     verticalWalls[7, 0] = true;
        //     verticalWalls[8, 0] = true;
        //     verticalWalls[9, 0] = true;
        //     verticalWalls[10, 0] = true;
        //     
        //     verticalWalls[0, 1] = true;
        //     verticalWalls[1, 1] = true;
        //     verticalWalls[2, 1] = true;
        //     verticalWalls[3, 1] = true;
        //     verticalWalls[4, 1] = true;
        //     verticalWalls[5, 1] = true;
        //     verticalWalls[6, 1] = true;
        //     verticalWalls[7, 1] = true;
        //     verticalWalls[8, 1] = true;
        //     
        //     verticalWalls[1, 2] = true;
        //     verticalWalls[2, 2] = true;
        //     verticalWalls[3, 2] = true;
        //     verticalWalls[4, 2] = true;
        //     verticalWalls[5, 2] = true;
        //     verticalWalls[6, 2] = true;
        //     verticalWalls[7, 2] = true;
        //     verticalWalls[8, 2] = true;
        //     verticalWalls[9, 2] = true;
        //     
        //     verticalWalls[0, 3] = true;
        //     verticalWalls[1, 3] = true;
        //     verticalWalls[2, 3] = true;
        //     verticalWalls[3, 3] = true;
        //     verticalWalls[4, 3] = true;
        //     verticalWalls[5, 3] = true;
        //     verticalWalls[6, 3] = true;
        //     verticalWalls[7, 3] = true;
        //     verticalWalls[8, 3] = true;
        //     
        //     verticalWalls[0, 4] = true;
        //     verticalWalls[1, 4] = true;
        //     verticalWalls[2, 4] = true;
        //     verticalWalls[3, 4] = true;
        //     verticalWalls[5, 4] = true;
        //     verticalWalls[7, 4] = true;
        //     verticalWalls[8, 4] = true;
        //     verticalWalls[9, 4] = true;
        //     
        //     verticalWalls[1, 5] = true;
        //     verticalWalls[2, 5] = true;
        //     verticalWalls[3, 5] = true;
        //     verticalWalls[4, 5] = true;
        //     verticalWalls[5, 5] = true;
        //     verticalWalls[6, 5] = true;
        //     verticalWalls[7, 5] = true;
        //     verticalWalls[8, 5] = true;
        //     
        //     verticalWalls[0, 6] = true;
        //     verticalWalls[1, 6] = true;
        //     verticalWalls[2, 6] = true;
        //     verticalWalls[3, 6] = true;
        //     verticalWalls[4, 6] = true;
        //     verticalWalls[5, 6] = true;
        //     verticalWalls[6, 6] = true;
        //     verticalWalls[7, 6] = true;
        //     verticalWalls[8, 6] = true;
        //     verticalWalls[9, 6] = true;
        //
        //     WallData test = new WallData(horizontalWalls, verticalWalls);
        //
        //     // Perform check
        //     CheckTileAccessibility(test);
    }
}