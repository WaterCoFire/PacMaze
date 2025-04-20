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
            Vector3 bombPosition = gameObject.transform.position;
            // Kill this ghostron
            GhostronManager.Instance.KillNearestGhostron(bombPosition);
            // Kill another ghostron which is nearest to this bomb
            GhostronManager.Instance.KillNearestGhostron(bombPosition);
        }
    }
}