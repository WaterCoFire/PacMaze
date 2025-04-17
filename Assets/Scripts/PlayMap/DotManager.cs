using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayMap {
    /**
     * Manages all the dots in the play map.
     * The player wins when all the dots are eaten by pacman (disappear).
     */
    public class DotManager : MonoBehaviour {
        // List of all the dots
        private List<GameObject> _dots = new();
        
        // Status indicating a dot is eaten
        // Only check the number of dots if this is true, for better performance
        private bool _newDotEaten;
        
        // START FUNCTION
        private void Start() {
            Debug.Log("DotManager START");

            // Initialize the ghost list
            _dots.Clear();

            // Dot eaten status resetting
            _newDotEaten = false;
        }

        // UPDATE FUNCTION
        // Keeps checking if there is any dots left
        // If all dots are eaten, game over and the player wins
        private void Update() {
            // Only check if a new dot is eaten
            if (!_newDotEaten) return;

            if (_dots.Count == 0) {
                Debug.Log("No dots left, player should win");
                // TODO corresponding logic
            } else {
                Debug.Log("Dot num checked, still more dots to be eaten");
            }

            // Dot eaten status resetting
            _newDotEaten = false;
        }

        /**
         * Add a new dot (game object) info into the list.
         * Used in PropGenerator when initializing the map.
         */
        public void AddDot(GameObject newDot) {
            _dots.Add(newDot); // Add into the list
        }

        /**
         * Removes a dot object from the list.
         * Called by Dot class when it collides with pacman (being eaten).
         */
        public void RemoveDot(GameObject eatenDot) {
            if (!_dots.Remove(eatenDot)) {
                // Remove failed because game object isn't existing
                Debug.LogError("Remove dot error: Dot GameObject is not found!");
                return;
            }

            // Indicate a new dot is eaten
            _newDotEaten = true;
        }
    }
}