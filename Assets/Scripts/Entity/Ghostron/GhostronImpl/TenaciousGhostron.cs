using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron.GhostronImpl {
    public class TenaciousGhostron : Ghostron {
        // Wander interval of the Tenacious Ghostron
        public override float WanderInterval {
            get { return 2.0f; }
        }

        // Scared duration of the Tenacious Ghostron
        // TENACIOUS GHOSTRON WILL NOT BE SCARED
        public override float ScaredDuration {
            get { return 0.0f; }
        }
        
        // Minimum wander duration of the Tenacious Ghostron
        // THE SAME in easy/normal/hard mode
        public override float MinimumWanderDuration {
            get { return 1.0f; }
        }
        
        // Maximum chase duration of the Tenacious Ghostron
        // THE SAME in easy/normal/hard mode
        public override float MaximalChaseDuration {
            get { return 10000.0f; }
        }

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Tenacious Ghostron:
         * WHEN IN NORMAL WANDER & WHEN SCARED
         * Always go to the position that the Pacboy has been to.
         * As the wander interval is short for Tenacious Ghostron,
         * this is almost the current location of the Pacboy.
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