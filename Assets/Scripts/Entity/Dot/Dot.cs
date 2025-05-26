using PlayMap;
using UnityEngine;

namespace Entity.Dot {
    /**
     * Manages the behaviour of the dot in game map.
     */
    public class Dot : MonoBehaviour {
        // The index of this dot
        private int _index;

        // The dot manager component (should in the Map game object)
        private DotManager _dotManager;

        // START FUNCTION
        private void Start() {
            // Find the DotManager
            // It is set inside the Map game object
            GameObject mapObject = GameObject.Find("Map");
            if (mapObject != null) {
                _dotManager = DotManager.Instance;
            }

            // Check if the DotManager is set
            if (_dotManager == null) {
                Debug.LogError("Error when initialising dots: DotManager not found!");
            }

            // Check if the dot has Box Collider
            if (gameObject.GetComponent<BoxCollider>() == null) {
                Debug.LogError("Error when initialising dots: Dot has no BoxCollider!");
            }

            // Enable isTrigger of this dot
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }

        /**
         * Sets the index of this dot.
         * Used when initialising this dot in DotManager.
         */
        public void SetIndex(int index) {
            _index = index;
        }

        /**
         * Event when the dot collides with other game objects
         * (All objects except Pacboy should be ignored)
         */
        private void OnTriggerEnter(Collider other) {
            // If the other object is not Pacboy, ignore it
            if (!other.CompareTag("Pacboy")) return;

            // Do not remove if DotManager is still not found
            if (_dotManager == null) {
                Debug.LogWarning("No DotManager, can't remove dot");
                return;
            }

            // Remove this dot in DotManager
            _dotManager.RemoveDot(_index);

            // Give the Pacboy 10 score points
            PlayMapController.Instance.AddScore(10);

            // Destroy the game object of this dot
            // So that it disappears from the scene
            Destroy(this.gameObject);
        }
    }
}