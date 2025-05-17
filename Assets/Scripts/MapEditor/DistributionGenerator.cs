using System;
using System.Collections.Generic;

namespace MapEditor {
    public class DistributionGenerator {
        private static Dictionary<(int, int), int> _distributedNeighbourNums;
        private static readonly Random Random = new();
        
        /* Probabilities */
        private const int OutermostTileThreeNeighbourProbability = 10;
        private const int ThreeNeighbourPropagationProbability = 50;

        public static Dictionary<(int, int), int> GenerateDistributedNeighbourNums() {
            // Initialise the distributed neighbour nums list
            _distributedNeighbourNums = new Dictionary<(int, int), int>();
            for (int x = 0; x < 11; x++) {
                for (int y = 0; y < 11; y++) {
                    _distributedNeighbourNums.Add((x, y), 0);
                }
            }
            
            // Pattern 1
            DetermineOutermostRingTiles();
            
            // Pattern 2 & 3
            DetermineInnerTiles();
            
            // Pattern 4
            PlaceAdjacentFourNeighbourTilesInCentre();

            // Return the final map tile neighbour information
            return _distributedNeighbourNums;
        }

        private static void DetermineOutermostRingTiles() {
            // Find all the tiles in the outermost ring first
            List<(int, int)> outerTiles = new List<(int, int)>();

            for (int x = 0; x < 11; x++) {
                for (int y = 0; y < 11; y++) {
                    if (x == 0 || x == 10 || y == 0 || y == 10) {
                        outerTiles.Add((x, y));
                    }
                }
            }

            // Shuffle the order
            Shuffle(outerTiles);

            int threeNeighbourTilesCount = 0;
            foreach (var tile in outerTiles) {
                // If there are already four 3-neighbour tiles, all the remaining should be 2-neighbour
                if (threeNeighbourTilesCount >= 4) {
                    _distributedNeighbourNums[tile] = 2;
                    continue;
                }

                // Check that the tile has no adjacent 3-neighbour tiles
                // If so, it has a probability to be 3-neighbour
                if (!TestHasAdjacentThreeNeighbourTiles(tile) &&
                    Random.Next(100) < OutermostTileThreeNeighbourProbability) {
                    _distributedNeighbourNums[tile] = 3;
                    threeNeighbourTilesCount++;
                } else {
                    _distributedNeighbourNums[tile] = 2;
                }
            }
        }

        private  static double TestWeightedThreeNeighbourProb((int, int) tile) {
            // Calculate the Manhattan distance
            int distance = Math.Max(Math.Abs(tile.Item1 - 5), Math.Abs(tile.Item2 - 5));
            
            // Return the probability
            return 100 * Math.Min(0.9, Math.Max(0.1, 0.75 - 0.12 * distance));
        }

        private static void DetermineInnerTiles() {
            for (int x = 1; x <= 9; x++) {
                for (int y = 1; y <= 9; y++) {
                    if (_distributedNeighbourNums[(x, y)] != 0) {
                        // Skip processed tiles
                        continue;
                    }

                    // Obtain the 3-neighbour probability of the current tile
                    if (Random.Next(100) < TestWeightedThreeNeighbourProb((x, y))) {
                        _distributedNeighbourNums[(x, y)] = 3;
                        
                        // Local propagation mechanism
                        if (Random.Next(100) < ThreeNeighbourPropagationProbability) {
                            List<(int, int)> directions = new List<(int, int)>();
                            directions.Add((-1, 0));
                            directions.Add((1, 0));
                            directions.Add((0, -1));
                            directions.Add((0, 1));
                            Shuffle(directions);
                            
                            // Get a random direction (U/D/L/R)
                            foreach (var randomDirection in directions) {
                                int nx = x + randomDirection.Item1;
                                int ny = y + randomDirection.Item2;

                                if (_distributedNeighbourNums[(nx, ny)] == 0) {
                                    _distributedNeighbourNums[(nx, ny)] = 3;
                                    break;
                                }
                            }
                        } else {
                            _distributedNeighbourNums[(x, y)] = 2;
                        }
                    }
                }
            }
        }

        private static void PlaceAdjacentFourNeighbourTilesInCentre() {
            // Find all the centre tiles first
            List<(int, int)> centreTiles = new List<(int, int)>();

            for (int x = 4; x <= 6; x++) {
                for (int y = 4; y <= 6; y++) {
                    centreTiles.Add((x, y));
                }
            }
            
            // Shuffle the order
            Shuffle(centreTiles);
            
            // Randomly set two adjacent tiles as 4-neighbour
            foreach (var tile in centreTiles) {
                List<(int, int)> twoDirections = new List<(int, int)>();
                twoDirections.Add((1, 0));
                twoDirections.Add((0, 1));
                Shuffle(twoDirections);

                // Get a random direction (D/R)
                foreach (var randomDirection in twoDirections) {
                    int nx = tile.Item1 + randomDirection.Item1;
                    int ny = tile.Item2 + randomDirection.Item2;
                    
                    // Check if nx and ny is still a centre tile
                    if (nx >= 4 && nx <= 6 && ny >= 4 && ny <= 6) {
                        // Set both tiles as 4-neighbour
                        _distributedNeighbourNums[tile] = 4;
                        _distributedNeighbourNums[(nx, ny)] = 4;
                        return;
                    }
                }
            }
        }

        /**
         * Check that if the tile coordinate given has adjacent tiles that are already distributed as 3-neighbour.
         * Called by DetermineOutermostRingTiles() to check the outermost tiles.
         * So this function is ONLY able to process outermost tiles.
         */
        private static bool TestHasAdjacentThreeNeighbourTiles((int, int) tile) {
            if (tile.Item1 == 0) {
                if (tile.Item2 == 0) {
                    // Case: x = 0, y = 0, check the tiles at: down/right
                    return _distributedNeighbourNums[(tile.Item1 + 1, tile.Item2)] == 3 ||
                            _distributedNeighbourNums[(tile.Item1, tile.Item2 + 1)] == 3;
                } else if (tile.Item2 == 10) {
                    // Case: x = 0, y = 10, check the tiles at: down/left
                    return _distributedNeighbourNums[(tile.Item1 + 1, tile.Item2)] == 3 ||
                           _distributedNeighbourNums[(tile.Item1, tile.Item2 - 1)] == 3;
                } else {
                    // Case: x = 0, y = 1-9, check the tiles at: left/right
                    return _distributedNeighbourNums[(tile.Item1, tile.Item2 - 1)] == 3 ||
                           _distributedNeighbourNums[(tile.Item1, tile.Item2 + 1)] == 3;
                }
            } else if (tile.Item1 == 10) {
                if (tile.Item2 == 0) {
                    // Case: x = 10, y = 0, check the tiles at: up/right
                    return _distributedNeighbourNums[(tile.Item1 - 1, tile.Item2)] == 3 ||
                           _distributedNeighbourNums[(tile.Item1, tile.Item2 + 1)] == 3;
                } else if (tile.Item2 == 10) {
                    // Case: x = 10, y = 10, check the tiles at: up/left
                    return _distributedNeighbourNums[(tile.Item1 - 1, tile.Item2)] == 3 ||
                           _distributedNeighbourNums[(tile.Item1, tile.Item2 - 1)] == 3;
                } else {
                    // Case: x = 10, y = 1-9, check the tiles at: left/right
                    return _distributedNeighbourNums[(tile.Item1, tile.Item2 - 1)] == 3 ||
                           _distributedNeighbourNums[(tile.Item1, tile.Item2 + 1)] == 3;
                }
            } else {
                // Case: x = 1-9, y = 0 or 10, check the tiles at: up/down
                return _distributedNeighbourNums[(tile.Item1 - 1, tile.Item2)] == 3 ||
                       _distributedNeighbourNums[(tile.Item1 + 1, tile.Item2)] == 3;
            }
        }

        private static void Shuffle<T>(List<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}