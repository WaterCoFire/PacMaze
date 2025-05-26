using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Deployed Nice Bomb in tutorial
     */
    public class TutorialDeployedNiceBomb : MonoBehaviour {
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
            TutorialController.Instance.KillDemoTwoGhostrons();
        }
    }
}