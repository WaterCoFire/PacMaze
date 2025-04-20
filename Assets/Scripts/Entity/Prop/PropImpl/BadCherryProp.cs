using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Prop.PropImpl {
    /**
     * BAD CHERRY
     * Spawns one more ghostron.
     * (spawns randomly)
     */
    public class BadCherryProp : Prop {
        // Prefab of all types of ghostrons
        // Every time a ghostron with a random color is generated
        public GameObject redGhostronPrefab;
        public GameObject blueGhostronPrefab;
        public GameObject yellowGhostronPrefab;
        public GameObject greenGhostronPrefab;
        public GameObject pinkGhostronPrefab;

        // Override
        public override void OnPicked(GameObject pacman) {
            Debug.Log("BAD CHERRY picked");
            // Spawn a new ghostron at random place
            GameObject newGhostron = GetRandomGhostronType();
            
            // Add to ghostron manager
            GhostronManager.Instance.AddGhostron(newGhostron);
            GhostronManager.Instance.SetPacman(pacman);

            Instantiate(newGhostron, GetRandomSpawnPoint(), Quaternion.identity);
        }

        /**
         * Generates a random position for another ghostron to spawn on.
         */
        private Vector3 GetRandomSpawnPoint() {
            // Possible x/z axis coordinate values of the position
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
            // ALTHOUGH UNLIKELY
            Debug.LogError("Invalid bad cherry ghostron spawn position generated!");
            return transform.position;
        }

        /**
         * Returns a random type (color) of ghostron.
         */
        private GameObject GetRandomGhostronType() {
            int rand = Random.Range(0, 5); // Random number
            switch (rand) {
                case 0:
                    return redGhostronPrefab;
                case 1:
                    return blueGhostronPrefab;
                case 2:
                    return yellowGhostronPrefab;
                case 3:
                    return greenGhostronPrefab;
                case 4:
                    return pinkGhostronPrefab;
                default:
                    Debug.LogError("Invalid number generated when getting a random ghostron color!");
                    return null;
            }
        }
    }
}