using System.Collections.Generic;
using Entity.Ghostron;
using Entity.Map;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PlayMap {
    /**
     * Manages all the Ghostrons in each game.
     */
    public class GhostronManager : MonoBehaviour {
        // The list of all the active Ghostrons
        private readonly List<GameObject> _ghostrons = new();

        // Normal wandering speed of Ghostrons
        private readonly float _ghostronNormalSpeed = 2.0f;

        // Scared speed of Ghostrons (when Pacboy eats a power pellet)
        private readonly float _ghostronScaredSpeed = 1.0f;

        // Chasing speeds of Ghostrons, by difficulty
        private readonly float _ghostronEasyChaseSpeed = 3.2f;
        private readonly float _ghostronNormalChaseSpeed = 4.5f;
        private readonly float _ghostronHardChaseSpeed = 6f;

        // Detection radius of Ghostrons, by difficulty
        private readonly float _ghostronEasyDetectionRadius = 10.0f;
        private readonly float _ghostronNormalDetectionRadius = 20.0f;
        private readonly float _ghostronHardDetectionRadius = 25.0f;

        // Difficulty of the current game
        private DifficultyType _difficulty;

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
         * Decides the chasing speed and detection radius of the Ghostrons.
         */
        public void SetDifficulty(DifficultyType difficulty) {
            _difficulty = difficulty;
        }

        /**
         * Resets the Ghostrons list.
         * Used in PropGenerator when initialising the map.
         */
        public void ResetGhostrons() {
            // Empty the Ghostron list
            _ghostrons.Clear();
        }

        /**
         * Add a new Ghostron information.
         */
        public void AddGhostron(GameObject newGhostron) {
            // Set the params of the Ghostron according to difficulty
            switch (_difficulty) {
                case DifficultyType.Easy:
                    // EASY
                    newGhostron.GetComponent<Ghostron>().SetGhostronParams(_ghostronNormalSpeed, _ghostronScaredSpeed,
                        _ghostronEasyChaseSpeed,
                        _ghostronEasyDetectionRadius);
                    break;
                case DifficultyType.Normal:
                    // NORMAL
                    newGhostron.GetComponent<Ghostron>().SetGhostronParams(_ghostronNormalSpeed, _ghostronScaredSpeed,
                        _ghostronNormalChaseSpeed,
                        _ghostronNormalDetectionRadius);
                    break;
                case DifficultyType.Hard:
                    // HARD
                    newGhostron.GetComponent<Ghostron>().SetGhostronParams(_ghostronNormalSpeed, _ghostronScaredSpeed,
                        _ghostronHardChaseSpeed,
                        _ghostronHardDetectionRadius);
                    break;
                default:
                    // INVALID DIFFICULTY - PROMPT ERROR
                    Debug.LogError("Invalid difficulty when adding Ghostron information: " + _difficulty);
                    return;
            }

            // Add to the Ghostron list
            _ghostrons.Add(newGhostron);
        }

        /**
         * Scares all the Ghostrons.
         * Called when the Pacboy eats a power pellet.
         */
        public void ScareAllGhostrons() {
            foreach (var ghostron in _ghostrons) {
                // Scare each of them
                ghostron.GetComponent<Ghostron>().Scare();
            }
        }

        /**
         * Kills the nearest Ghostron to the position given.
         * Used when a Nice Bomb is used OR deployed and then hit by a Ghostron:
         * - USED:
         * Obtain the nearest Ghostron to the Pacboy and kill it
         * - DEPLOYED & HIT:
         * Obtain the two nearest Ghostrons to the deployed bomb and kill them
         * (which means the Ghostron hitting the deployed bomb & another nearest Ghostron)
         *
         * RETURNS:
         * - true if a Ghostron is successfully killed
         * - false otherwise (when there is no Ghostron left / unexpected things happen)
         */
        public bool KillNearestGhostron(Vector3 position) {
            if (_ghostrons.Count == 0) {
                return false;
            }

            GameObject nearest = null;
            float minDist = float.MaxValue;

            // Check for the nearest Ghostron
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
                return true;
            }

            Debug.LogError("An error occurred when killing the nearest Ghostron! (nearest = null)");
            return false;
        }

        /**
         * Spawns the Tenacious Ghostron at random place.
         * Called when a Bad Cherry is picked by the Pacboy.
         */
        public void SpawnTenaciousGhostron() {
            // Generate random position for the Tenacious Ghostron to spawn
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

            // Spawn new Tenacious Ghostron (oh no for Pacboy!)
            GameObject newTenaciousGhostronPrefab = GhostronFactory.Instance.GetGhostron(GhostronType.Tenacious);
            GameObject newTenaciousGhostron = Instantiate(newTenaciousGhostronPrefab, randomPosition,
                Quaternion.identity);

            // Set its Pacboy target (oh no again!)
            newTenaciousGhostron.GetComponent<Ghostron>().SetPacboy(PlayMapController.Instance.GetPacboy());
            
            // Add the Tenacious Ghostron to the list
            AddGhostron(newTenaciousGhostron);
        }

        /**
         * Sets the Pacboy info that all the Ghostrons chase.
         */
        public void SetPacboy(GameObject pacboy) {
            foreach (var ghostron in _ghostrons) {
                ghostron.GetComponent<Ghostron>().SetPacboy(pacboy);
            }
        }

        /**
         * Action when a Ghostron catches the pacboy.
         * Make the game over (and the player loses).
         */
        public void PacboyCaught() {
            PlayMapController.Instance.Lose();
        }
        
        /**
         * Sets the status of Crazy Party.
         * Called by EventManager when the Crazy Party should be on/off.
         */
        public void SetCrazyParty(bool on) {
            foreach (var ghostron in _ghostrons) {
                // Set the status of all the Ghostrons
                ghostron.GetComponent<Ghostron>().SetCrazyParty(on);
            }
        }
    }
}