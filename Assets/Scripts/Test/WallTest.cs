using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Test {
    public class WallTest : MonoBehaviour {
        public GameObject[,] HorizontalWalls;
        public GameObject[,] VerticalWalls;

        private List<(int, int)> _allTiles;
        private Dictionary<(int, int), List<(int, int)>> _preferredNeighbours;
        private Dictionary<(int, int), int> _currentNeighbourCounts;
        private Dictionary<(int, int), int> _distributedNeighbourNums;
        private bool[,] _horizontalWallStatus;
        private bool[,] _verticalWallStatus;

        Random _random = new();

        // Four directions: up, down, left, right
        private readonly int[] _dx = { -1, 1, 0, 0 };
        private readonly int[] _dy = { 0, 0, -1, 1 };

        private void Start() {
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

                for (int column = 0; column < 11; column++) {
                    string hWallName = $"HWall_{column + 1}";
                    Transform hWall = hRow.Find(hWallName);
                    if (hWall == null) {
                        Debug.LogError($"Error: Not found {hRowName}/{hWallName}");
                        continue;
                    }

                    HorizontalWalls[row, column] = hWall.gameObject;
                }
            }

            // Load vertical walls
            for (int column = 0; column < 10; column++) {
                string vColName = $"VWall_Column_{column + 1}";
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

                    VerticalWalls[row, column] = vWall.gameObject;
                }
            }
            
            // DO BRAID TEST
            BraidTest();
        }

        private void BraidTest() {
            // Initialise all tiles list
            _allTiles = new List<(int, int)>();
            for (int x = 0; x < 11; x++) {
                for (int y = 0; y < 11; y++) {
                    _allTiles.Add((x, y));
                }
            }

            // Initialise all wall status (should not be existing, so false)
            _horizontalWallStatus = new bool[10, 11];
            _verticalWallStatus = new bool[11, 10];

            TestInitCurrentNeighbourCounts();
            TestInitPreferredNeighbours();
            TestInitDistributedNums();

            Shuffle(_allTiles);

            HandleThreeNeighbourTiles();
            HandleNonCentreTwoNeighbourTiles();
            HandleUnprocessedCentreTiles();
            
            // Horizontal walls
            for (int row = 0; row < 10; row++) {
                for (int column = 0; column < 11; column++) {
                    HorizontalWalls[row, column].SetActive(_horizontalWallStatus[row, column]);
                }
            }
           
            // Vertical walls
            for (int row = 0; row < 11; row++) {
                for (int column = 0; column < 10; column++) {
                    VerticalWalls[row, column].SetActive(_verticalWallStatus[row, column]);
                }
            }
        }

        private void HandleThreeNeighbourTiles() {
            // Find all the 3-neighbour tiles first
            List<(int, int)> allThreeNeighbourTiles = new List<(int, int)>();
            foreach (var tile in _allTiles) {
                if (_distributedNeighbourNums[tile] == 3) {
                    allThreeNeighbourTiles.Add(tile);
                }
            }

            foreach (var threeNeighbourTile in allThreeNeighbourTiles) {
                // Obtain all adjacent tiles and shuffle them
                List<(int, int)> adjacentTiles = GetAdjacentTiles(threeNeighbourTile);
                Shuffle(adjacentTiles);

                foreach (var adjacentTile in adjacentTiles) {
                    // Stop if neighbour num is already down to the distributed value
                    if (_currentNeighbourCounts[threeNeighbourTile] <= _distributedNeighbourNums[threeNeighbourTile]) {
                        break;
                    }

                    if (_distributedNeighbourNums[adjacentTile] == 2) {
                        // Case: the iterated adjacent tile is 2-neighbour
                        // Ignore if this adjacent tile cannot lose any more neighbour
                        if (_currentNeighbourCounts[adjacentTile] <= _distributedNeighbourNums[adjacentTile]) {
                            continue;
                        }

                        // Valid 2-neighbour adjacent tiles will be disconnected
                        Disconnect(threeNeighbourTile, adjacentTile);
                        _currentNeighbourCounts[threeNeighbourTile]--;
                        _currentNeighbourCounts[adjacentTile]--;

                        // Check the connectivity of the map
                        if (!Flood()) {
                            // If this disconnection affects the connectivity, undo it
                            Connect(threeNeighbourTile, adjacentTile);
                            _currentNeighbourCounts[threeNeighbourTile]++;
                            _currentNeighbourCounts[adjacentTile]++;
                        }
                    } else {
                        // Case: the iterated adjacent tile is 3- or 4-neighbour
                        // Ignore if this adjacent tile cannot lose any more neighbour
                        if (_currentNeighbourCounts[adjacentTile] <= _distributedNeighbourNums[adjacentTile]) {
                            continue;
                        }

                        // Valid adjacent 3-neighbour tiles have only a small probability of not being neighbours
                        if (_random.Next(100) < 10) {
                            // Disconnect the two tile
                            Disconnect(threeNeighbourTile, adjacentTile);
                            _currentNeighbourCounts[threeNeighbourTile]--;
                            _currentNeighbourCounts[adjacentTile]--;

                            // Check the connectivity of the map
                            if (!Flood()) {
                                // If this disconnection affects the connectivity, undo it
                                Connect(threeNeighbourTile, adjacentTile);
                                _currentNeighbourCounts[threeNeighbourTile]++;
                                _currentNeighbourCounts[adjacentTile]++;
                            }
                        }
                    }
                }
            }
        }

        private void HandleNonCentreTwoNeighbourTiles() {
            // Find all the 2-neighbour tiles that is not in the centre first
            List<(int, int)> allNonCentreTwoNeighbourTiles = new List<(int, int)>();
            foreach (var tile in _allTiles) {
                if (_distributedNeighbourNums[tile] == 2) {
                    if ((tile.Item1 < 4 || tile.Item1 > 6) && (tile.Item2 < 4 || tile.Item2 > 6)) {
                        allNonCentreTwoNeighbourTiles.Add(tile);
                    }
                }
            }

            foreach (var twoNeighbourTile in allNonCentreTwoNeighbourTiles) {
                // Obtain all adjacent tiles and shuffle them
                List<(int, int)> adjacentTiles = GetAdjacentTiles(twoNeighbourTile);
                Shuffle(adjacentTiles);

                foreach (var adjacentTile in adjacentTiles) {
                    // Stop if neighbour num is already down to the distributed value
                    if (_currentNeighbourCounts[twoNeighbourTile] <= _distributedNeighbourNums[twoNeighbourTile]) {
                        break;
                    }

                    if (!_preferredNeighbours[twoNeighbourTile].Contains(adjacentTile)) {
                        // Case: the iterated adjacent tile is not the ideal neighbour
                        // Ignore if this adjacent tile cannot lose any more neighbour
                        if (_currentNeighbourCounts[adjacentTile] <= _distributedNeighbourNums[adjacentTile]) {
                            continue;
                        }
                        
                        // Valid adjacent non-ideal tiles have a big probability of not being neighbours
                        if (_random.Next(100) < 75) {
                            // Disconnect the two tile
                            Disconnect(twoNeighbourTile, adjacentTile);
                            _currentNeighbourCounts[twoNeighbourTile]--;
                            _currentNeighbourCounts[adjacentTile]--;
                            
                            // Check the connectivity of the map
                            if (!Flood()) {
                                // If this disconnection affects the connectivity, undo it
                                Connect(twoNeighbourTile, adjacentTile);
                                _currentNeighbourCounts[twoNeighbourTile]++;
                                _currentNeighbourCounts[adjacentTile]++;
                            }
                        }
                    }
                }

                if (_currentNeighbourCounts[twoNeighbourTile] > _distributedNeighbourNums[twoNeighbourTile]) {
                    // If the current number is still more than its distributed num, sacrifice ideal neighbours
                    foreach (var adjacentTile in adjacentTiles) {
                        // Stop if neighbour num is already down to the distributed value
                        if (_currentNeighbourCounts[twoNeighbourTile] <= _distributedNeighbourNums[twoNeighbourTile]) {
                            break;
                        }

                        // Find ideal neighbours
                        if (_preferredNeighbours[twoNeighbourTile].Contains(adjacentTile)) {
                            // Case: the iterated adjacent tile is not the ideal neighbour
                            // Ignore if this adjacent tile cannot lose any more neighbour
                            if (_currentNeighbourCounts[adjacentTile] <= _distributedNeighbourNums[adjacentTile]) {
                                continue;
                            }
                        
                            // Disconnect the two tile
                            Disconnect(twoNeighbourTile, adjacentTile);
                            _currentNeighbourCounts[twoNeighbourTile]--;
                            _currentNeighbourCounts[adjacentTile]--;
                            
                            // Check the connectivity of the map
                            if (!Flood()) {
                                // If this disconnection affects the connectivity, undo it
                                Connect(twoNeighbourTile, adjacentTile);
                                _currentNeighbourCounts[twoNeighbourTile]++;
                                _currentNeighbourCounts[adjacentTile]++;
                            }
                        }
                    }
                }
            }
        }

        private void HandleUnprocessedCentreTiles() {
            // Find all the unprocessed centre tiles first
            // This means all 2-neighbour and 4-neighbour tiles in it
            List<(int, int)> allUnprocessedCentreTiles = new List<(int, int)>();
            for (int x = 4; x <= 6; x++) {
                for (int y = 4; y <= 6; y++) {
                    if (_distributedNeighbourNums[(x, y)] != 3) {
                        allUnprocessedCentreTiles.Add((x, y));
                    }
                }
            }

            foreach (var unprocessedCentreTile in allUnprocessedCentreTiles) {
                // Obtain all adjacent tiles and shuffle them
                List<(int, int)> adjacentTiles = GetAdjacentTiles(unprocessedCentreTile);
                Shuffle(adjacentTiles);
                
                // Iterate through the shuffled adjacent tiles to randomly disconnect tiles
                foreach (var adjacentTile in adjacentTiles) {
                    // Stop if neighbour num is already down to the distributed value
                    if (_currentNeighbourCounts[unprocessedCentreTile] <= _distributedNeighbourNums[unprocessedCentreTile]) {
                        break;
                    }
                    
                    // Ignore if this adjacent tile cannot lose any more neighbour
                    if (_currentNeighbourCounts[adjacentTile] <= _distributedNeighbourNums[adjacentTile]) {
                        continue;
                    }

                    // Disconnect the two tile
                    Disconnect(unprocessedCentreTile, adjacentTile);
                    _currentNeighbourCounts[unprocessedCentreTile]--;
                    _currentNeighbourCounts[adjacentTile]--;
                            
                    // Check the connectivity of the map
                    if (!Flood()) {
                        // If this disconnection affects the connectivity, undo it
                        Connect(unprocessedCentreTile, adjacentTile);
                        _currentNeighbourCounts[unprocessedCentreTile]++;
                        _currentNeighbourCounts[adjacentTile]++;
                    }
                }
            }
        }

        private List<(int, int)> GetAdjacentTiles((int, int) tile) {
            List<(int, int)> adjacentTiles = new List<(int, int)>();

            if (tile.Item1 == 0) {
                if (tile.Item2 == 0) {
                    // Case: x = 0, y = 0
                    // 2 adjacent directions: Down/Right
                    adjacentTiles.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                    adjacentTiles.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                } else if (tile.Item2 == 10) {
                    // Case: x = 0, y = 10
                    // 2 adjacent directions: Down/Left
                    adjacentTiles.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                    adjacentTiles.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                } else {
                    // Case: x = 0, y = 1-9
                    // 3 adjacent directions: Down/Left/Right
                    adjacentTiles.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                    adjacentTiles.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                    adjacentTiles.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                }
            } else if (tile.Item1 == 10) {
                if (tile.Item2 == 0) {
                    // Case: x = 10, y = 0
                    // 2 adjacent directions: Up/Right
                    adjacentTiles.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                    adjacentTiles.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                } else if (tile.Item2 == 10) {
                    // Case: x = 10, y = 10
                    // 2 adjacent directions: Up/Left
                    adjacentTiles.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                    adjacentTiles.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                } else {
                    // Case: x = 10, y = 1-9
                    // 3 adjacent directions: Up/Left/Right
                    adjacentTiles.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                    adjacentTiles.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                    adjacentTiles.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                }
            } else {
                if (tile.Item2 == 0) {
                    // Case: x = 1-9, y = 0
                    // 3 adjacent directions: Up/Down/Right
                    adjacentTiles.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                    adjacentTiles.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                    adjacentTiles.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                } else if (tile.Item2 == 10) {
                    // Case: x = 1-9, y = 10
                    // 3 adjacent directions: Up/Down/Left
                    adjacentTiles.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                    adjacentTiles.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                    adjacentTiles.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                } else {
                    // Case: x = 1-9, y = 1-9
                    // 4 adjacent directions: Up/Down/Left/Right
                    adjacentTiles.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                    adjacentTiles.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                    adjacentTiles.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                    adjacentTiles.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                }
            }

            return adjacentTiles;
        }

        private void Disconnect((int, int) tileA, (int, int) tileB) {
            if (tileA.Item1 == tileB.Item1) {
                // Case: Both tile have the save x-coordinate
                // So the wall to be built should be a vertical one
                int y1 = Math.Min(tileA.Item2, tileB.Item2);
                int y2 = Math.Max(tileA.Item2, tileB.Item2);
                if (y1 + 1 != y2) {
                    // Show error if the difference between the two y-coordinates is bigger than one
                    Debug.LogError("Error: Invalid tile coordinates passed.");
                    return;
                }

                if (_verticalWallStatus[tileA.Item1, y1]) {
                    // Show error if this wall is already present
                    Debug.LogError("Error: A present wall is set to be built again!");
                    return;
                }

                // Build this wall
                _verticalWallStatus[tileA.Item1, y1] = true;
            } else if (tileA.Item2 == tileB.Item2) {
                // Case: Both tile have the save y-coordinate
                // So the wall to be built should be a horizontal one
                int x1 = Math.Min(tileA.Item1, tileB.Item1);
                int x2 = Math.Max(tileA.Item1, tileB.Item1);
                if (x1 + 1 != x2) {
                    // Show error if the difference between the two x-coordinates is bigger than one
                    Debug.LogError("Error: Invalid tile coordinates passed.");
                    return;
                }

                if (_horizontalWallStatus[x1, tileA.Item2]) {
                    // Show error if this wall is already present
                    Debug.LogError("Error: A present wall is set to be built again!");
                    return;
                }

                // Build this wall
                _horizontalWallStatus[x1, tileA.Item2] = true;
            } else {
                // Case: x- and y-coordinates are all different
                // Show error
                Debug.LogError("Error: Invalid tile coordinates passed.");
            }
        }

        private void Connect((int, int) tileA, (int, int) tileB) {
            if (tileA.Item1 == tileB.Item1) {
                // Case: Both tile have the save x-coordinate
                // So the wall to be broken should be a vertical one
                int y1 = Math.Min(tileA.Item2, tileB.Item2);
                int y2 = Math.Max(tileA.Item2, tileB.Item2);
                if (y1 + 1 != y2) {
                    // Show error if the difference between the two y-coordinates is bigger than one
                    Debug.LogError("Error: Invalid tile coordinates passed.");
                    return;
                }

                if (!_verticalWallStatus[tileA.Item1, y1]) {
                    // Show error if this wall is already absent
                    Debug.LogError("Error: An absent wall is set to be broken again!");
                    return;
                }

                // Break this wall
                _verticalWallStatus[tileA.Item1, y1] = false;
            } else if (tileA.Item2 == tileB.Item2) {
                // Case: Both tile have the save y-coordinate
                // So the wall to be broken should be a horizontal one
                int x1 = Math.Min(tileA.Item1, tileB.Item1);
                int x2 = Math.Max(tileA.Item1, tileB.Item1);
                if (x1 + 1 != x2) {
                    // Show error if the difference between the two x-coordinates is bigger than one
                    Debug.LogError("Error: Invalid tile coordinates passed.");
                    return;
                }

                if (!_horizontalWallStatus[x1, tileA.Item2]) {
                    // Show error if this wall is already absent
                    Debug.LogError("Error: An absent wall is set to be broken again!");
                    return;
                }

                // Break this wall
                _horizontalWallStatus[x1, tileA.Item2] = false;
            } else {
                // Case: x- and y-coordinates are all different
                // Show error
                Debug.LogError("Error: Invalid tile coordinates passed.");
            }
        }

        private bool Flood() {
            // Creating an 11x11 array of access states
            bool[,] visited = new bool[11, 11];
            int[,] distance = new int[11, 11]; // Record the shortest distance for each grid

            // Breadth-first search (BFS) queue
            // initialised by adding the centre (5, 5) to the queue
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((5, 5));
            visited[5, 5] = true;
            distance[5, 5] = 0;

            // BFS Search
            while (queue.Count > 0) {
                var (x, y) = queue.Dequeue();

                // Get horizontal wall status and vertical wall status from wall data
                bool[,] horizontalWallStatus = _horizontalWallStatus;
                bool[,] verticalWallStatus = _verticalWallStatus;

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

            // Iterate over all the tiles and find the ones that are inaccessible
            // If found, return false
            for (int i = 0; i < 11; i++) {
                for (int j = 0; j < 11; j++) {
                    if (!visited[i, j]) {
                        return false;
                    }
                }
            }

            // All tiles valid, return true
            return true;
        }

        private void TestInitCurrentNeighbourCounts() {
            _currentNeighbourCounts = new Dictionary<(int, int), int>();

            // Iterate through all tiles
            foreach (var tile in _allTiles) {
                if (tile.Item1 == 0 || tile.Item1 == 10) {
                    if (tile.Item2 == 0 || tile.Item2 == 10) {
                        // Case: x = 0 or 10, y = 0 or 10, 2 neighbours at the beginning
                        _currentNeighbourCounts.Add(tile, 2);
                    } else {
                        // Case: x = 0 or 10, y = 1-9, 3 neighbours at the beginning
                        _currentNeighbourCounts.Add(tile, 3);
                    }
                } else {
                    if (tile.Item2 == 0 || tile.Item2 == 10) {
                        // Case: x = 1-9, y = 0 or 10, 3 neighbours at the beginning
                        _currentNeighbourCounts.Add(tile, 3);
                    } else {
                        // Case: x = 1-9, y = 1-9, 4 neighbours at the beginning
                        _currentNeighbourCounts.Add(tile, 4);
                    }
                }
            }
        }

        private void TestInitPreferredNeighbours() {
            _preferredNeighbours = new Dictionary<(int, int), List<(int, int)>>();

            // Iterate through all tiles
            foreach (var tile in _allTiles) {
                // The list that stores a tile's two preferred neighbours (if it is 2-neighbour)
                List<(int, int)> tilePreferredNeighbours = new List<(int, int)>();

                // Calculate the two preferred neighbours based on coordinates
                if (tile.Item1 == tile.Item2) {
                    if (tile.Item1 <= 3) {
                        // Case: x = y = 0/1/2/3, Down/Right
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                    } else {
                        // Case: x = y = 7/8/9/10, Up/Left
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                    }
                } else if (tile.Item1 + tile.Item2 == 10) {
                    if (tile.Item1 <= 3) {
                        // Case: x + y = 10 and x = 0/1/2/3, Down/Left
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[1]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[2]));
                    } else {
                        // Case: x + y = 10 and x = 7/8/9/10, Up/Right
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[0], tile.Item2 + _dy[0]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[3], tile.Item2 + _dy[3]));
                    }
                } else if (tile.Item1 == 0 || tile.Item1 == 10) {
                    // Case: x = 0 or 10, Left/Right
                    tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[2]));
                    tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[3]));
                } else if (tile.Item1 == 1 || tile.Item1 == 9) {
                    if (tile.Item2 == 0 || tile.Item2 == 10) {
                        // Case: x = 1 or 9, y = 0 or 10, Up/Down
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[0]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[1]));
                    } else {
                        // Case: x = 1 or 9, y = 2-8, Left/Right
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[2]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[3]));
                    }
                } else if (tile.Item1 == 2 || tile.Item1 == 8) {
                    if (tile.Item2 <= 1 || tile.Item2 >= 9) {
                        // Case: x = 2 or 8, y = 0-1 or 9-10, Up/Down
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[0]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[1]));
                    } else {
                        // Case: x = 2 or 8, y = 3-7, Left/Right
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[2]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[3]));
                    }
                } else if (tile.Item1 == 3 || tile.Item1 == 7) {
                    if (tile.Item2 <= 2 || tile.Item2 >= 8) {
                        // Case: x = 3 or 7, y = 0-2 or 8-10, Up/Down
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[0]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[1]));
                    } else {
                        // Case: x = 3 or 7, y = 4-6, Left/Right
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[2]));
                        tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[3]));
                    }
                } else {
                    // Case: x = 4-6, Up/Down
                    tilePreferredNeighbours.Add((tile.Item1 + _dx[1], tile.Item2 + _dy[0]));
                    tilePreferredNeighbours.Add((tile.Item1 + _dx[2], tile.Item2 + _dy[1]));
                }

                _preferredNeighbours.Add(tile, tilePreferredNeighbours);
            }
        }

        private void TestInitDistributedNums() {
            _distributedNeighbourNums = new Dictionary<(int, int), int>();

            foreach (var tile in _allTiles) {
                _distributedNeighbourNums.Add(tile, 2);
            }

            _distributedNeighbourNums[(0, 4)] = 3;
            _distributedNeighbourNums[(0, 9)] = 3;

            _distributedNeighbourNums[(1, 8)] = 3;

            _distributedNeighbourNums[(2, 2)] = 3;
            _distributedNeighbourNums[(2, 4)] = 3;
            _distributedNeighbourNums[(2, 7)] = 3;
            _distributedNeighbourNums[(2, 8)] = 3;

            _distributedNeighbourNums[(3, 2)] = 3;
            _distributedNeighbourNums[(3, 3)] = 3;
            _distributedNeighbourNums[(3, 5)] = 3;
            _distributedNeighbourNums[(3, 6)] = 3;
            _distributedNeighbourNums[(3, 8)] = 3;

            _distributedNeighbourNums[(4, 1)] = 3;
            _distributedNeighbourNums[(4, 2)] = 3;
            _distributedNeighbourNums[(4, 3)] = 3;
            _distributedNeighbourNums[(4, 4)] = 3;
            _distributedNeighbourNums[(4, 5)] = 3;
            _distributedNeighbourNums[(4, 8)] = 3;

            _distributedNeighbourNums[(5, 1)] = 3;
            _distributedNeighbourNums[(5, 2)] = 3;
            _distributedNeighbourNums[(5, 3)] = 3;
            _distributedNeighbourNums[(5, 4)] = 4;
            _distributedNeighbourNums[(5, 5)] = 4;
            _distributedNeighbourNums[(5, 6)] = 3;
            _distributedNeighbourNums[(5, 7)] = 3;

            _distributedNeighbourNums[(6, 2)] = 3;
            _distributedNeighbourNums[(6, 3)] = 3;
            _distributedNeighbourNums[(6, 6)] = 3;
            _distributedNeighbourNums[(6, 8)] = 3;

            _distributedNeighbourNums[(7, 2)] = 3;
            _distributedNeighbourNums[(7, 3)] = 3;
            _distributedNeighbourNums[(7, 4)] = 3;
            _distributedNeighbourNums[(7, 5)] = 3;
            _distributedNeighbourNums[(7, 6)] = 3;
            _distributedNeighbourNums[(7, 8)] = 3;

            _distributedNeighbourNums[(8, 4)] = 3;
            _distributedNeighbourNums[(8, 6)] = 3;
            _distributedNeighbourNums[(8, 8)] = 3;
            _distributedNeighbourNums[(8, 9)] = 3;

            _distributedNeighbourNums[(9, 3)] = 3;
            _distributedNeighbourNums[(9, 4)] = 3;
            _distributedNeighbourNums[(9, 10)] = 3;

            _distributedNeighbourNums[(10, 3)] = 3;
        }

        public void Shuffle<T>(List<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = _random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}