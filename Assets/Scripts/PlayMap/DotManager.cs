using System;
using System.Collections.Generic;
using Entity.Dot;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayMap {
    /**
     * Manages all the dots in the play map.
     * The player wins when all the dots are eaten by pacman (disappear).
     */
    public class DotManager : MonoBehaviour {
        // List of all the dot index
        private List<int> _dots = new();
        
        // Status indicating a dot is eaten
        // Only check the number of dots if this is true, for better performance
        private bool _newDotEaten;
        
        // Singleton instance
        public static DotManager Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("DotManager AWAKE");
            // Set singleton instance
            Instance = this;
        }
        
        // START FUNCTION
        private void Start() {
            Debug.Log("DotManager START");
        }

        // UPDATE FUNCTION
        // Keeps checking if there is any dot left
        // If all dots are eaten, game over and the player wins
        private void Update() {
            // Only check if a new dot is eaten
            if (!_newDotEaten) return;

            if (_dots.Count == 0) {
                Debug.Log("No dots left, player should win");
                // TODO corresponding logic
            } else {
                Debug.Log($"Dot num checked, still {_dots.Count} dots to be eaten");
            }

            // Dot eaten status resetting
            _newDotEaten = false;
        }

        /**
         * Resets the dot index list.
         * Used in PropGenerator when initializing the map.
         */
        public void ResetDots() {
            // Empty the index list
            _dots.Clear();
            
            // Dot eaten status resetting
            _newDotEaten = false;
        }

        /**
         * Add a new dot index into the list.
         * Used in PropGenerator when initializing the map.
         */
        public void AddDot(GameObject newDot) {
            // Get the new index of this dot
            // Calculated by the number of elements in the dot index list
            int newIndex = _dots.Count;
            
            // Set the dot's index
            newDot.GetComponent<Dot>().SetIndex(newIndex);
            
            _dots.Add(newIndex); // Add dot index the list
        }

        /**
         * Removes a dot object from the list by index.
         * Called by Dot class when it collides with pacman (being eaten).
         */
        public void RemoveDot(int eatenDotIndex) {
            if (!_dots.Remove(eatenDotIndex)) {
                // Remove failed because this index isn't found
                Debug.LogError($"Remove dot error: Dot index: {eatenDotIndex} is not found!");
                // return;
            }

            // Indicate a new dot is eaten
            _newDotEaten = true;
        }
    }
}