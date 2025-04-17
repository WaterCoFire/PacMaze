using System;
using System.Collections.Generic;
using Entity.Ghost;
using UnityEngine;

namespace PlayMap {
    /**
     * Manages all the ghosts in each game.
     */
    public class GhostController : MonoBehaviour {
        // The list of all the active ghosts
        private List<GameObject> _ghosts = new();

        // Normal wandering speed of ghosts
        // TODO PROVISIONAL
        private float _ghostNormalSpeed = 2.0f;

        // Chasing speeds of ghosts, by difficulty
        // TODO PROVISIONAL
        private float _ghostEasyChaseSpeed = 3.0f;
        private float _ghostNormalChaseSpeed = 4.0f;
        private float _ghostHardChaseSpeed = 5.05f;

        // Detection radius of ghosts, by difficulty
        // TODO PROVISIONAL
        private float _ghostEasyDetectionRadius = 10.0f;
        private float _ghostNormalDetectionRadius = 20.0f;
        private float _ghostHardDetectionRadius = 100.0f;

        // Difficulty of the current game
        private char _difficulty;

        // START FUNCTION
        private void Start() {
            Debug.Log("GhostController START");

            // Initialize the ghost list
            _ghosts.Clear();
        }

        /**
         * Sets the difficulty of the current game.
         * Decides the chasing speed and detection radius of the ghosts.
         */
        public void SetDifficulty(char difficulty) {
            _difficulty = difficulty;
        }

        /**
         * Add a new ghost information.
         */
        public bool AddGhost(GameObject newGhost) {
            // Set the params of the ghost according to difficulty
            switch (_difficulty) {
                case 'E':
                    // EASY
                    newGhost.GetComponent<Ghost>().SetGhostParams(_ghostNormalSpeed, _ghostEasyChaseSpeed,
                        _ghostEasyDetectionRadius);
                    break;
                case 'N':
                    // NORMAL
                    newGhost.GetComponent<Ghost>().SetGhostParams(_ghostNormalSpeed, _ghostNormalChaseSpeed,
                        _ghostNormalDetectionRadius);
                    break;
                case 'H':
                    // HARD
                    newGhost.GetComponent<Ghost>().SetGhostParams(_ghostNormalSpeed, _ghostHardChaseSpeed,
                        _ghostHardDetectionRadius);
                    break;
                default:
                    // INVALID DIFFICULTY - PROMPT ERROR
                    Debug.LogError("Invalid difficulty when adding ghost information: " + _difficulty);
                    return false;
            }

            // Add to the ghost list
            _ghosts.Add(newGhost);
            return true;
        }

        /**
         * Sets the pacman info that all the ghosts chase.
         */
        public void SetPacman(GameObject pacman) {
            foreach (var ghost in _ghosts) {
                ghost.GetComponent<Ghost>().SetPacman(pacman);
            }
        }
    }
}