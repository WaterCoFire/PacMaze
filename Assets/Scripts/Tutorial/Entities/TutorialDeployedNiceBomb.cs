using Sound;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Deployed Nice Bomb in tutorial
     */
    public class TutorialDeployedNiceBomb : MonoBehaviour {
        /**
         * Unity event: When the deployed bomb collides with a game object
         * Checks if the game object is a Ghostron
         * If so, call the GhostronHit() function
         */
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Ghostron")) {
                // Play explode sound
                SoundManager.Instance.PlaySoundOnce(SoundType.NiceBombExplode);
                
                GhostronHit();
                Destroy(gameObject); // Destroy this Deployed Nice Bomb
            }
        }

        /**
         * A Ghostron hits this Deployed Nice Bomb.
         */
        private void GhostronHit() {
            TutorialController.Instance.KillDemoTwoGhostrons();
        }
    }
}