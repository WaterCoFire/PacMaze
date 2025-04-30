using System.Collections.Generic;
using UnityEngine;

namespace MapEditor {
    public class TileChecker : MonoBehaviour {
        // Four directions: up, down, left, right
        private static readonly int[] Dx = { -1, 1, 0, 0 };
        private static readonly int[] Dy = { 0, 0, -1, 1 };

        private void Start() {
            /* TEST TEST TEST TEST TEST ONLY!!!!!!! */
            // Initialise the wall state (all walls do not "have" walls)
            bool[,] horizontalWalls = new bool[10, 11]; // HW: 10 rows, 11 walls per row
            bool[,] verticalWalls = new bool[11, 10]; // VW: 11 rows, 10 walls per row

            // Set some walls as "having" walls
            horizontalWalls[5, 5] = true; // Horizontal wall at (6, 6)
            horizontalWalls[9, 1] = true;
            horizontalWalls[9, 2] = true;
            horizontalWalls[9, 3] = true;
            horizontalWalls[9, 4] = true;
            horizontalWalls[9, 5] = true;
            horizontalWalls[9, 6] = true;

            verticalWalls[1, 0] = true;
            verticalWalls[2, 0] = true;
            verticalWalls[3, 0] = true;
            verticalWalls[4, 0] = true;
            verticalWalls[5, 0] = true;
            verticalWalls[6, 0] = true;
            verticalWalls[7, 0] = true;
            verticalWalls[8, 0] = true;
            verticalWalls[9, 0] = true;
            verticalWalls[10, 0] = true;

            verticalWalls[0, 1] = true;
            verticalWalls[1, 1] = true;
            verticalWalls[2, 1] = true;
            verticalWalls[3, 1] = true;
            verticalWalls[4, 1] = true;
            verticalWalls[5, 1] = true;
            verticalWalls[6, 1] = true;
            verticalWalls[7, 1] = true;
            verticalWalls[8, 1] = true;

            verticalWalls[1, 2] = true;
            verticalWalls[2, 2] = true;
            verticalWalls[3, 2] = true;
            verticalWalls[4, 2] = true;
            verticalWalls[5, 2] = true;
            verticalWalls[6, 2] = true;
            verticalWalls[7, 2] = true;
            verticalWalls[8, 2] = true;
            verticalWalls[9, 2] = true;

            verticalWalls[0, 3] = true;
            verticalWalls[1, 3] = true;
            verticalWalls[2, 3] = true;
            verticalWalls[3, 3] = true;
            verticalWalls[4, 3] = true;
            verticalWalls[5, 3] = true;
            verticalWalls[6, 3] = true;
            verticalWalls[7, 3] = true;
            verticalWalls[8, 3] = true;

            verticalWalls[0, 4] = true;
            verticalWalls[1, 4] = true;
            verticalWalls[2, 4] = true;
            verticalWalls[3, 4] = true;
            verticalWalls[5, 4] = true;
            verticalWalls[7, 4] = true;
            verticalWalls[8, 4] = true;
            verticalWalls[9, 4] = true;

            verticalWalls[1, 5] = true;
            verticalWalls[2, 5] = true;
            verticalWalls[3, 5] = true;
            verticalWalls[4, 5] = true;
            verticalWalls[5, 5] = true;
            verticalWalls[6, 5] = true;
            verticalWalls[7, 5] = true;
            verticalWalls[8, 5] = true;

            verticalWalls[0, 6] = true;
            verticalWalls[1, 6] = true;
            verticalWalls[2, 6] = true;
            verticalWalls[3, 6] = true;
            verticalWalls[4, 6] = true;
            verticalWalls[5, 6] = true;
            verticalWalls[6, 6] = true;
            verticalWalls[7, 6] = true;
            verticalWalls[8, 6] = true;
            verticalWalls[9, 6] = true;

            // Perform check
            var result = CheckTileAccessibility(horizontalWalls, verticalWalls);
            
            // Debug.Log("TEST111");

            if (result == null) {
                Debug.Log("All tiles are good");
            } else {
                Debug.Log("The following tiles are inaccessible or the path exceeds 20:");
                foreach (var tile in result) {
                    Debug.Log($"({tile.Item1}, {tile.Item2})");
                }
            }
        }
        
        /**
         * Check the reachability of all the tiles.
         * Called by MapEditor when the player attempts to Save & Quit.
         * Returns:
         * The list of all invalid tiles.
         * If all tiles are valid, the list is null.
         */
        public static List<(int, int)>
            CheckTileAccessibility(bool[,] horizontalWallStatus, bool[,] verticalWallStatus) {
            // Creating an 11x11 array of access states
            bool[,] visited = new bool[11, 11];
            int[,] distance = new int[11, 11]; // Record the shortest distance for each grid

            // Store tiles that are inaccessible or at a distance greater than 20
            List<(int, int)> unreachableTiles = new List<(int, int)>();

            // Breadth-first search (BFS) queue, initialised by adding the center (6, 6) to the queue
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((6, 6));
            visited[6, 6] = true;
            distance[6, 6] = 0;

            // BFS Search
            while (queue.Count > 0) {
                var (x, y) = queue.Dequeue();

                // If the current distance is greater than 20, it is marked as unreachable
                if (distance[x, y] > 17) {
                    unreachableTiles.Add((x, y));
                    continue;
                }

                // Check four directions (up, down, left, right)
                for (int i = 0; i < 4; i++) {
                    int nx = x + Dx[i];
                    int ny = y + Dy[i];

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
            // and find the ones that are inaccessible or have a distance greater than 20
            for (int i = 0; i < 11; i++) {
                for (int j = 0; j < 11; j++) {
                    if (!visited[i, j] || distance[i, j] > 17) {
                        unreachableTiles.Add((i, j));
                    }
                }
            }

            // Returns null if no invalid tile is found
            return unreachableTiles.Count > 0 ? unreachableTiles : null;
        }
    }
}