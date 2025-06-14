﻿using Entity.Map;
using PlayMap;
using UnityEngine;

namespace Entity.Ghostron.GhostronImpl {
    /**
     * Green Ghostron - Personality: Sensitive
     *
     * Only spawns if the number of Ghostrons set for the map is 5.
     * 
     * It likes to go to the general area of Pacboy when wandering.
     * Maybe because it can sensitively detect the location of Pacboy's scent.
     */
    public class GreenGhostron : Ghostron {
        // Wander interval of the green Ghostron
        public override float WanderInterval {
            get { return 12.0f; }
        }
        
        // Scared duration of the green Ghostron
        public override float ScaredDuration {
            get { return 8.0f; }
        }
        
        // Minimum wander duration of the green Ghostron
        public override float MinimumWanderDuration {
            // Easy: 6
            // Normal: 4
            // Hard: 3
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 6f;
                    case DifficultyType.Normal:
                        return 4f;
                    case DifficultyType.Hard:
                        return 3f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising Ghostrons: " + PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }
        
        // Maximum chase duration of the green Ghostron
        public override float MaximalChaseDuration {
            // Easy: 12
            // Normal, Hard: 18
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 12f;
                    case DifficultyType.Normal:
                        return 18f;
                    case DifficultyType.Hard:
                        return 18f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising Ghostrons: " + PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }
        
        // The four quadrant middle points, as potential positions
        private readonly Vector3[] _potentialPositions = {
            new(-9, 0, -9), new(-9, 0, 9), new(9, 0, -9), new(9, 0, 9)
        };

        private int _positionIndex = -1; // Index of the current target position, 0-3

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Green Ghostron:
         * --- Normal Wander ---
         * Go to the middle point of the four quadrants of the map that is the nearest to the Pacboy.
         * If the green Ghostron already arrives there, go to another random quadrant middle point.
         * --- Scared Wander ---
         * Go to the point that is the furthest.
         */
        public override Vector3 GenerateWanderingTarget() {
            // When scared
            if (isScared) {
                if (pacboy != null) {
                    // Find the corner that is the furthest away from the Pacboy
                    Vector3 furthestPosition = _potentialPositions[0];
                    int index = 0;
                    float maxDistance = Vector3.Distance(furthestPosition, pacboy.transform.position);

                    for (int i = 0; i < 4; i++) {
                        float distance = Vector3.Distance(_potentialPositions[i], pacboy.transform.position);
                        if (distance > maxDistance) {
                            furthestPosition = _potentialPositions[i];
                            index = i;
                            maxDistance = distance;
                        }
                    }

                    // Duplicate target avoiding logic
                    if (index != _positionIndex) {
                        // If the new position is different, return this position
                        _positionIndex = index;
                        return furthestPosition;
                    } else {
                        // Get another random position at the corner
                        int randIndex;
                        while (true) {
                            var rand = Random.Range(0, 4);
                            if (rand != _positionIndex) {
                                randIndex = rand;
                                break;
                            }
                        }

                        _positionIndex = randIndex;
                        furthestPosition = _potentialPositions[randIndex];
                        return furthestPosition;
                    }
                }

                return transform.position;
            }

            // When not scared
            if (pacboy != null) {
                // Find the corner that is the nearest from the Pacboy
                Vector3 nearestPosition = _potentialPositions[0];
                int index = 0;
                float minDistance = Vector3.Distance(nearestPosition, pacboy.transform.position);

                for (int i = 0; i < 4; i++) {
                    float distance = Vector3.Distance(_potentialPositions[i], pacboy.transform.position);
                    if (distance < minDistance) {
                        nearestPosition = _potentialPositions[i];
                        index = i;
                        minDistance = distance;
                    }
                }

                // Duplicate target avoiding logic
                if (index != _positionIndex) {
                    // If the new position is different, return this position
                    _positionIndex = index;
                    return nearestPosition;
                } else {
                    // Get another random position at the corner
                    int randIndex;
                    while (true) {
                        var rand = Random.Range(0, 4);
                        if (rand != _positionIndex) {
                            randIndex = rand;
                            break;
                        }
                    }

                    _positionIndex = randIndex;
                    nearestPosition = _potentialPositions[randIndex];
                    return nearestPosition;
                }
            }

            return transform.position;
        }
    }
}