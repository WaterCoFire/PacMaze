using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Manages the Dot game object in tutorial scene.
     */
    public class TutorialDot : MonoBehaviour {
        /**
         * Event when the dot collides with other game objects
         * (All objects except Pacboy should be ignored)
         */
        private void OnTriggerEnter(Collider other) {
            // If the other object is not Pacboy, ignore it
            if (!other.CompareTag("Pacboy")) return;
            
            // Destroy the game object of this dot
            // So that it disappears from the scene
            Destroy(this.gameObject);
        }
    }
}