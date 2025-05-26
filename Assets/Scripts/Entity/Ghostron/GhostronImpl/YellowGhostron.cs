using Entity.Map;
using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron.GhostronImpl {
    public class YellowGhostron : Ghostron {
        // Wander interval of the yellow Ghostron
        public override float WanderInterval {
            get { return 30.0f; }
        }

        // Scared duration of the yellow Ghostron
        public override float ScaredDuration {
            get { return 8.0f; }
        }

        // Minimum wander duration of the yellow Ghostron
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
                        Debug.LogError("Error: Invalid difficulty when initialising Ghostrons: " + PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }

        // Maximum chase duration of the yellow Ghostron
        public override float MaximalChaseDuration {
            // Easy: 40
            // Normal, Hard: 50
            get {
                switch (PlayMapController.Instance.GetDifficulty()) {
                    case DifficultyType.Easy:
                        return 40f;
                    case DifficultyType.Normal:
                        return 50f;
                    case DifficultyType.Hard:
                        return 50f;
                    default:
                        Debug.LogError("Error: Invalid difficulty when initialising Ghostrons: " + PlayMapController.Instance.GetDifficulty());
                        return 0f;
                }
            }
        }
        
        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Yellow Ghostron:
         * WHEN IN NORMAL WANDER
         * Always go to the position that the Pacboy has been to.
         * WHEN SCARED
         * Go to a random position.
         */
        public override Vector3 GenerateWanderingTarget() {
            // When scared
            if (isScared) {
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

            // When not scared
            if (pacboy != null) {
                return pacboy.transform.position;
            }

            return transform.position;
        }
    }
}