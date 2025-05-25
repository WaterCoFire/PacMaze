using Entity.Map;
using PlayMap;
using UnityEngine;

namespace Entity.Ghostron.GhostronImpl {
    public class BlueGhostron : Ghostron {
        // Wander interval of the blue ghostron
        public override float WanderInterval {
            get { return 15.0f; }
        }

        // Scared duration of the blue ghostron
        public override float ScaredDuration {
            get { return 8.0f; }
        }

        // Minimum wander duration of the blue ghostron
        public override float MinimumWanderDuration {
            // Easy: 10
            // Normal: 8
            // Hard: 7
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 10f;
                    case DifficultyType.Normal:
                        return 8f;
                    case DifficultyType.Hard:
                        return 7f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising ghostrons: " + PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }

        // Maximum chase duration of the blue ghostron
        public override float MaximalChaseDuration {
            // Easy: 8
            // Normal, Hard: 12
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 8f;
                    case DifficultyType.Normal:
                        return 12f;
                    case DifficultyType.Hard:
                        return 12f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising ghostrons: " + PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }

        // The four corners, as potential positions
        private readonly Vector3[] _potentialPositions = {
            new(-15, 0, -15), new(-15, 0, 15), new(15, 0, -15), new(15, 0, 15)
        };

        private int _positionIndex = -1; // Index of the current target position, 0-3

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Blue Ghostron:
         * WHEN IN NORMAL WANDER & WHEN SCARED
         * Go to the map corner which is the furthest away from the Pacboy.
         * If the blue ghostron already arrives there, go to another random corner.
         */
        public override Vector3 GenerateWanderingTarget() {
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
    }
}