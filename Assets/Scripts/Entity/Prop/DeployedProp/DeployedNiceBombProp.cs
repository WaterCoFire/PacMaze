using PlayMap;
using PlayMap.UI;
using UnityEngine;

namespace Entity.Prop.DeployedProp {
    /**
     * DEPLOYED NICE BOMB
     * Placed in the map when the player presses F (default) to deploy a nice bomb.
     * When hit by the Ghostron, two nearest Ghostrons will be killed.
     * (which means the one that hits the bomb and the other which is the nearest to that bomb)
     */
    public class DeployedNiceBombProp : MonoBehaviour {
        /**
         * Unity event: When the deployed bomb collides with a game object
         * Checks if the game object is a Ghostron
         * If so, call the GhostronHit() function
         */
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Ghostron")) {
                GhostronHit();
                Destroy(gameObject); // Destroy this deployed bomb
            }
        }

        /**
         * A Ghostron hits this Deployed Nice Bomb.
         */
        private void GhostronHit() {
            Debug.Log("Ghostron hits the deployed bomb");
            int ghostronsKilled = 0; // Count of how many Ghostrons are killed, can be 1/2

            Vector3 bombPosition = gameObject.transform.position;

            // Kill this Ghostron
            if (GhostronManager.Instance.KillNearestGhostron(bombPosition)) {
                ghostronsKilled++;
            }

            // Kill another Ghostron which is nearest to this bomb
            if (GhostronManager.Instance.KillNearestGhostron(bombPosition)) {
                ghostronsKilled++;
            }

            switch (ghostronsKilled) {
                case 1:
                    // Only one Ghostron killed
                    // Give the Pacboy 200 score points
                    PlayMapController.Instance.AddScore(200);
                    
                    // Prompt the player
                    GamePlayUI.Instance.NewInfo("Boom! A Ghostron stepped on the bomb you deployed and disappeared!", Color.green);
                    
                    break;
                case 2:
                    // Two Ghostrons killed
                    // Give the Pacboy 500 score points
                    PlayMapController.Instance.AddScore(500);
                    
                    // Prompt the player
                    GamePlayUI.Instance.NewInfo("Boom! A Ghostron stepped on the bomb you deployed and two Ghostrons are gone!", Color.green);
                    break;
            }
        }
    }
}