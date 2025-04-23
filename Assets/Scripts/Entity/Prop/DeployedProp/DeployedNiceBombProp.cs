using PlayMap;
using UnityEngine;

namespace Entity.Prop.DeployedProp {
    /**
     * DEPLOYED NICE BOMB
     * Placed in the map when the player presses F (default) to deploy a nice bomb.
     * When hit by the ghostron, two nearest ghostrons will be killed.
     * (which means the one that hits the bomb and the other which is the nearest to that bomb)
     */
    public class DeployedNiceBombProp : MonoBehaviour {
        /**
         * Unity event: When the deployed bomb collides with a game object
         * Checks if the game object is a ghostron
         * If so, call the GhostronHit() function
         */
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Ghostron")) {
                GhostronHit();
                Destroy(gameObject); // Destroy this deployed bomb
            }
        }

        /**
         * A ghostron hits this deployed nice bomb
         */
        private void GhostronHit() {
            Debug.Log("Ghostron hits the deployed bomb");
            int ghostronsKilled = 0; // Count of how many ghostrons are killed, can be 1/2

            Vector3 bombPosition = gameObject.transform.position;

            // Kill this ghostron
            if (GhostronManager.Instance.KillNearestGhostron(bombPosition)) {
                ghostronsKilled++;
            }

            // Kill another ghostron which is nearest to this bomb
            if (GhostronManager.Instance.KillNearestGhostron(bombPosition)) {
                ghostronsKilled++;
            }

            switch (ghostronsKilled) {
                case 1:
                    // Only one ghostron killed
                    // Give the pacman 200 score points
                    PlayMapController.Instance.AddScore(200);
                    break;
                case 2:
                    // Two ghostrons killed
                    // Give the pacman 500 score points
                    PlayMapController.Instance.AddScore(500);
                    break;
                default:
                    Debug.LogError("Unexpected amount of ghostrons killed by deployed bomb: " + ghostronsKilled);
                    break;
            }
        }
    }
}