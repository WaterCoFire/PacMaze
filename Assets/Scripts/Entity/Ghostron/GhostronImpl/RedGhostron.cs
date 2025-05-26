using Entity.Map;
using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron.GhostronImpl {
    public class RedGhostron : Ghostron {
        // Wander interval of the red Ghostron
        public override float WanderInterval {
            get { return 20.0f; }
        }

        // Scared duration of the red Ghostron
        public override float ScaredDuration {
            get { return 8.0f; }
        }

        // Minimum wander duration of the red Ghostron
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

        // Maximum chase duration of the red Ghostron
        public override float MaximalChaseDuration {
            // Easy: 25
            // Normal, Hard: 30
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 25f;
                    case DifficultyType.Normal:
                        return 30f;
                    case DifficultyType.Hard:
                        return 30f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising Ghostrons: " + PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Red Ghostron:
         * WHEN IN NORMAL WANDER & WHEN SCARED
         * Go to a random position.
         */
        public override Vector3 GenerateWanderingTarget() {
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