using Entity.Map;
using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron.GhostronImpl {
    public class PinkGhostron : Ghostron {
        // Wander interval of the pink Ghostron
        public override float WanderInterval {
            get { return 20.0f; }
        }

        // Scared duration of the pink Ghostron
        public override float ScaredDuration {
            get { return 8.0f; }
        }

        // Minimum wander duration of the pink Ghostron
        public override float MinimumWanderDuration {
            // Easy: 14
            // Normal: 10
            // Hard: 8
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 14f;
                    case DifficultyType.Normal:
                        return 10f;
                    case DifficultyType.Hard:
                        return 8f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising Ghostrons: " +
                                       PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }

        // Maximum chase duration of the pink Ghostron
        public override float MaximalChaseDuration {
            // Easy: 5
            // Normal, Hard: 8
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 5f;
                    case DifficultyType.Normal:
                        return 8f;
                    case DifficultyType.Hard:
                        return 8f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising Ghostrons: " +
                                       PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }

        private bool _isCenterTarget; // Status indicating if the last target is center point or not

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Pink Ghostron:
         * WHEN IN NORMAL WANDER
         * Head towards to the center point.
         * If the pink Ghostron already arrives there, go to a random position.
         * WHEN SCARED
         * Head towards the center point (And not willing to leave)
         */
        public override Vector3 GenerateWanderingTarget() {
            // When scared
            if (isScared) {
                _isCenterTarget = true;
                Vector3 centerPosition = new(0, 0, 0);
                return centerPosition;
            }

            // When not scared
            if (!_isCenterTarget) {
                // Go to the center
                _isCenterTarget = true;
                Vector3 centerPosition = new(0, 0, 0);
                return centerPosition;
            } else {
                // Go to a random position
                _isCenterTarget = false;

                // Possible x/z axis coordinate values of the target
                int[] possibleValues = { -15, -12, -9, -6, -3, 0, 3, 6, 9, 12, 15 };

                // Generate random x/z axis coordinates
                int randX = possibleValues[Random.Range(0, possibleValues.Length)];
                int randZ = possibleValues[Random.Range(0, possibleValues.Length)];
                Vector3 potentialPosition = new Vector3(randX, 0, randZ);

                // Check if this location is a valid walkable point
                if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
                    // Return this position if it is valid
                    return hit.position;
                }


                // Return the current position of it is not valid (no moving)
                return transform.position;
            }
        }
    }
}