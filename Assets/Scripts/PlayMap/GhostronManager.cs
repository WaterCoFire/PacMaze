using System.Collections.Generic;
using Entity.Ghostron;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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

        // Pacman object setting
        private GameObject _pacman;

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
        public void AddGhostron(GameObject newGhostron) {
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
                    return;
            }

            // Add to the ghostron list
            _ghostrons.Add(newGhostron);
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
         *
         * RETURNS:
         * - true if a ghostron is successfully killed
         * - false otherwise (when there is no ghostron left / unexpected things happen)
         */
        public bool KillNearestGhostron(Vector3 position) {
            if (_ghostrons.Count == 0) {
                Debug.LogWarning("Killing the nearest ghostron: NO GHOSTRON LEFT!");
                return false;
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
                return true;
            }
            
            Debug.LogError("An error occurred when killing the nearest ghostron! (nearest = null)");
            return false;
        }

        /**
         * Spawns a new ghostron with random color at random place.
         */
        public void NewRandomGhostron() {
            // --- Position logic ---
            Vector3 randomPosition;

            // Generate random position
            // Possible x/z axis coordinate values of the position
            int[] possibleValues = { -15, -12, -9, -6, -3, 0, 3, 6, 9, 12, 15 };

            // Generate random x/z axis coordinates
            int randX = possibleValues[Random.Range(0, possibleValues.Length)];
            int randZ = possibleValues[Random.Range(0, possibleValues.Length)];
            Vector3 potentialPosition = new Vector3(randX, 0, randZ);

            // Check if this location is a valid walkable point
            if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
                // Return this position if it is valid
                randomPosition = hit.position;
            } else {
                // Directly return if it is not valid (no moving)
                // ALTHOUGH UNLIKELY
                Debug.LogError("Invalid spawn position generated!");
                return;
            }

            // --- Ghostron type logic ---
            GameObject newGhostron;
            int rand = Random.Range(0, 5); // Random number
            switch (rand) {
                case 0:
                    newGhostron = Instantiate(GetComponent<PropGenerator>().redGhostronPrefab, randomPosition,
                        Quaternion.identity);
                    break;
                case 1:
                    newGhostron = Instantiate(GetComponent<PropGenerator>().blueGhostronPrefab, randomPosition,
                        Quaternion.identity);
                    break;
                case 2:
                    newGhostron = Instantiate(GetComponent<PropGenerator>().yellowGhostronPrefab, randomPosition,
                        Quaternion.identity);
                    break;
                case 3:
                    newGhostron = Instantiate(GetComponent<PropGenerator>().greenGhostronPrefab, randomPosition,
                        Quaternion.identity);
                    break;
                case 4:
                    newGhostron = Instantiate(GetComponent<PropGenerator>().pinkGhostronPrefab, randomPosition,
                        Quaternion.identity);
                    break;
                default:
                    Debug.LogError("Invalid number generated when getting a random ghostron color!");
                    return;
            }

            // Set the pacman target
            newGhostron.GetComponent<Ghostron>().SetPacman(_pacman);
            // Add to the list
            AddGhostron(newGhostron);
        }

        /**
         * Sets the pacman info that all the ghostrons chase.
         */
        public void SetPacman(GameObject pacman) {
            _pacman = pacman;
            foreach (var ghostron in _ghostrons) {
                ghostron.GetComponent<Ghostron>().SetPacman(_pacman);
            }
        }

        /**
         * Action when a ghostron catches the pacman.
         * Make the game over (and the player loses).
         */
        public void PacmanCaught() {
            PlayMapController.Instance.Lose();
        }
    }
}