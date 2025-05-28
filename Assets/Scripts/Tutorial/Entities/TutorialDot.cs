using Sound;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Dot in tutorial
     */
    public class TutorialDot : MonoBehaviour {
        /**
         * Event when the dot collides with other game objects
         * (All objects except Pacboy should be ignored)
         */
        private void OnTriggerEnter(Collider other) {
            // If the other object is not Pacboy, ignore it
            if (!other.CompareTag("Pacboy")) return;
            
            // Play dot eaten sound
            SoundManager.Instance.PlaySoundOnce(SoundType.EatDot);
            
            // Destroy the game object of this dot
            // So that it disappears from the scene
            Destroy(gameObject);
        }
    }
}