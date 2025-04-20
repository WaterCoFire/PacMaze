using System.Collections.Generic;
using Entity.Ghostron;
using UnityEngine;

namespace PlayMap {
    /**
     * Manages all the ghostrons in each game.
     */
    public class GhostronManager : MonoBehaviour {
        // The list of all the active ghostrons
        private List<GameObject> _ghostrons = new();

        // Normal wandering speed of ghostrons
        private readonly float _ghostronNormalSpeed = 2.0f;

        // Scared speed of ghostrons (when pacman eats a power pellet)
        private readonly float _ghostronScaredSpeed = 1.0f;

        // Chasing speeds of ghostrons, by difficulty
        private readonly float _ghostronEasyChaseSpeed = 3.0f;
        private readonly float _ghostronNormalChaseSpeed = 4.0f;
        private readonly float _ghostronHardChaseSpeed = 5.05f;

        // Detection radius of ghostrons, by difficulty
        private readonly float _ghostronEasyDetectionRadius = 10.0f;
        private readonly float _ghostronNormalDetectionRadius = 20.0f;
        private readonly float _ghostronHardDetectionRadius = 30.0f;

        // Difficulty of the current game
        private char _difficulty;

        // Singleton instance
        public static GhostronManager Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("GhostronManager AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            Debug.Log("GhostronManager START");
        }

        /**
         * Sets the difficulty of the current game.
         * Decides the chasing speed and detection radius of the ghostrons.
         */
        public void SetDifficulty(char difficulty) {
            _difficulty = difficulty;
        }

        /**
         * Resets the ghostrons list.
         * Used in PropGenerator when initializing the map.
         */
        public void ResetGhostrons() {
            // Empty the ghostron list
            _ghostrons.Clear();
        }

        /**
         * Add a new ghostron information.
         */
        public bool AddGhostron(GameObject newGhostron) {
            // Set the params of the ghostron according to difficulty
            switch (_difficulty) {
                case 'E':
                    // EASY
                    newGhostron.GetComponent<Ghostron>().SetGhostronParams(_ghostronNormalSpeed, _ghostronScaredSpeed,
                        _ghostronEasyChaseSpeed,
                        _ghostronEasyDetectionRadius);
                    break;
                case 'N':
                    // NORMAL
                    newGhostron.GetComponent<Ghostron>().SetGhostronParams(_ghostronNormalSpeed, _ghostronScaredSpeed,
                        _ghostronNormalChaseSpeed,
                        _ghostronNormalDetectionRadius);
                    break;
                case 'H':
                    // HARD
                    newGhostron.GetComponent<Ghostron>().SetGhostronParams(_ghostronNormalSpeed, _ghostronScaredSpeed,
                        _ghostronHardChaseSpeed,
                        _ghostronHardDetectionRadius);
                    break;
                default:
                    // INVALID DIFFICULTY - PROMPT ERROR
                    Debug.LogError("Invalid difficulty when adding ghostron information: " + _difficulty);
                    return false;
            }

            // Add to the ghostron list
            _ghostrons.Add(newGhostron);
            return true;
        }

        /**
         * Scares all the ghostrons.
         * Called when the pacman eats a power pellet.
         */
        public void ScareAllGhostrons() {
            foreach (var ghostron in _ghostrons) {
                // Scare each of them
                ghostron.GetComponent<Ghostron>().SetScared(true);
            }
        }

        /**
         * Kills the nearest ghostron to the position given.
         * Used when a nice bomb is used OR deployed and then hit by a ghostron:
         * - USED:
         * Obtain the nearest ghostron to the pacman and kill it
         * - DEPLOYED & HIT:
         * Obtain the two nearest ghostron to the deployed bomb and kill them
         * (which means the ghostron hitting the deployed bomb & another nearest ghostron)
         */
        public void KillNearestGhostron(Vector3 position) {
            if (_ghostrons == null || _ghostrons.Count == 0) {
                Debug.LogWarning("Killing the nearest ghostron: NO GHOSTRON LEFT!");
            }

            GameObject nearest = null;
            float minDist = float.MaxValue;

            // Check for the nearest ghostron
            foreach (var ghost in _ghostrons) {
                float dist = Vector3.Distance(position, ghost.transform.position);
                if (dist < minDist) {
                    minDist = dist;
                    nearest = ghost;
                }
            }

            // Kill the nearest one
            if (nearest != null) {
                _ghostrons.Remove(nearest);
                Destroy(nearest);
                Debug.Log("Nearest ghostron destroyed!");
            }
        }

        /**
         * Sets the pacman info that all the ghostrons chase.
         */
        public void SetPacman(GameObject pacman) {
            foreach (var ghostron in _ghostrons) {
                ghostron.GetComponent<Ghostron>().SetPacman(pacman);
            }
        }
    }
}