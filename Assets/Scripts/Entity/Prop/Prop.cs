using System;
using UnityEngine;

namespace Entity.Prop {
    /**
     * The abstract class for all six props:
     * - Power Pellet
     * - Fast Wheel
     * - Nice Bomb
     * - Slow Wheel
     * - Bad Cherry
     * - Lucky Dice
     */
    public abstract class Prop : MonoBehaviour {
        // Abstract function
        // Action when the prop is picked by the pacman
        public abstract void OnPicked(GameObject pacman);

        // START FUNCTION
        private void Start() {
            Debug.Log("Prop Abstract Class START");
        }

        /**
         * Unity event: When the prop collides with a game object
         * Checks if the game object is pacman
         * If so, call the OnPicked() logic
         */
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Pacman")) {
                OnPicked(other.gameObject); // Corresponding action
                Destroy(gameObject); // Destroy this game object
            }
        }
    }
}