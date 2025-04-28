using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron.GhostronImpl {
    public class PinkGhostron : Ghostron {
        // Wander interval of the pink ghostron
        protected override float WanderInterval {
            get { return 8.0f; }
        }
        
        // Scared duration of the pink ghostron
        protected override float ScaredDuration {
            get { return 6.0f; }
        }
        
        private bool _isCenterTarget = false; // Status indicating if the last target is center point or not

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Pink Ghostron:
         * Head towards to the center point.
         * If the pink ghostron already arrives there, go to a random position.
         */
        protected override Vector3 GenerateWanderingTarget() {
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