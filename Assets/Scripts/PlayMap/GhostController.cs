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
        private List<GameObject> _ghosts;

        // Normal wandering speed of ghosts
        // TODO Decide them
        private float _ghostNormalSpeed;

        // Chasing speeds of ghosts, by difficulty
        // TODO Decide them
        private float _ghostEasyChaseSpeed;
        private float _ghostNormalChaseSpeed;
        private float _ghostHardChaseSpeed;

        // Detection radius of ghosts, by difficulty
        // TODO Decide them
        private float _ghostEasyDetectionRadius;
        private float _ghostNormalDetectionRadius;
        private float _ghostHardDetectionRadius;

        // Difficulty of the current game
        private char _difficulty;

        // START FUNCTION
        private void Start() {
            Debug.Log("GhostController START");

            // Initialize the ghost list
            _ghosts = new List<GameObject>();
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