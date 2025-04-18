using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron {
    /**
     * Manages the behaviour of the ghost.
     *
     * TODO: Models now resembles gfhosts in classical pacman game too much
     * Maybe find some new models online
     */
    public class Ghostron : MonoBehaviour {
        private GameObject _pacman; // Pacman game object, what the ghostron is hunting for

        // Normal wandering speed of the ghostron
        private float _normalSpeed;

        // Speed when the ghostron is scared (pacman eats a power pellet)
        private float _scaredSpeed;

        // Chasing Speed
        // This speed varies according to difficulty
        // Specific numbers are set in GhostronManager
        private float _chaseSpeed;

        // Detection Radius
        // Pacman will be chased when it is within this distance from a ghostron
        // This distance varies according to difficulty
        // Specific numbers are set in GhostronManager
        private float _detectionRadius;

        // Wandering logic variables
        private float _wanderTimer; // Timer of the current wandering
        private readonly float _wanderInterval = 15.0f; // Interval of switching a wandering target

        private NavMeshAgent _agent; // NavMesh agent
        private bool _isChasing; // Status indicating if the ghostron is wandering or chasing _pacman

        // The original material of the ghostron (the normal color)
        private Material _originalMaterial;

        // Material when the ghostron is scared
        public Material scaredMaterial;

        // START FUNCTION
        void Start() {
            Debug.Log("Ghostron START");

            // Bind the NavMeshAgent component
            _agent = GetComponent<NavMeshAgent>();

            // Bind the original material
            _originalMaterial = gameObject.GetComponent<SkinnedMeshRenderer>().material;
        }

        // UPDATE FUNCTION
        void Update() {
            // Check the distance between this ghostron and pacman target
            float distance = Vector3.Distance(transform.position, _pacman.transform.position);

            if (distance <= _detectionRadius) {
                // Chase the pacman
                if (!_isChasing) {
                    _isChasing = true;
                    Debug.Log("Pacman detected, starting chase!");
                }

                // Set the chasing speed
                _agent.speed = _chaseSpeed;

                // Set the chase target (real time position of the pacman)
                _agent.SetDestination(_pacman.transform.position);
            } else {
                // Pacman is out of the chasing detection radius
                // Impossible if in HARD game mode because the radius is long enough
                if (_isChasing) {
                    _isChasing = false;
                    _agent.ResetPath();
                    Debug.Log("Pacman escaped, now wandering.");
                }
                
                // Start to wander
                Wander();
            }
        }

        /**
         * The wandering logic.
         * (Re)start a wander process.
         */
        private void Wander() {
            _wanderTimer += Time.deltaTime;

            _agent.speed = _normalSpeed;

            if (!_agent.hasPath || _agent.remainingDistance < 0.5f || _wanderTimer >= _wanderInterval) {
                Vector3 newDestination = GenerateRandomWanderingTarget();
                _agent.SetDestination(newDestination);
                _wanderTimer = 0f;
            }
        }

        /**
         * Generates a random position.
         * Used for getting a target when the ghostron is wandering.
         */
        private Vector3 GenerateRandomWanderingTarget() {
            // Possible x/z axis coordinate values of the target
            int[] possibleValues = { -15, -12, -9, -6, -3, 0, 3, 6, 9, 12, 15 };

            
            // Generate random x/z axis coordinates
            int randX = possibleValues[Random.Range(0, possibleValues.Length)];
            int randZ = possibleValues[Random.Range(0, possibleValues.Length)];
            Vector3 potentialPosition = new Vector3(randX, 0, randZ);
            
            // Check if this location is a valid walkable point
            if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
                // Return this position if it is valid
                return hit.position;
            }

            // Return the current position of it is not valid (no moving)
            // ALTHOUGH UNLIKELY
            Debug.LogError("Invalid wandering target position generated!");
            return transform.position;
        }

        /**
         * Sets the pacman game object.
         * Used in GhostronManager when initializing the map.
         */
        public void SetPacman(GameObject pacman) {
            _pacman = pacman;
        }

        /**
         * Sets the normal speed, scared speed, chasing speed and detection radius of the ghostron.
         * Used in GhostronManager when initializing the map.
         */
        public void SetGhostronParams(float normalSpeed, float scaredSpeed, float chaseSpeed, float detectionRadius) {
            _normalSpeed = normalSpeed;
            _scaredSpeed = scaredSpeed;
            _chaseSpeed = chaseSpeed;
            _detectionRadius = detectionRadius;
        }
    }
}