using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghost {
    /**
     * Manages the behaviour of the ghost.
     */
    public class Ghost : MonoBehaviour {
        private GameObject _pacman; // Pacman game object, what the ghost is hunting for
        
        // Wandering speed of the ghost
        private float _normalSpeed;
        
        // Chasing Speed
        // This speed varies according to difficulty
        // Specific numbers are set in GhostManager
        private float _chaseSpeed;
        
        // Detection Radius
        // Pacman will be chased when it is within this distance from a ghost
        // This distance varies according to difficulty
        // Specific numbers are set in GhostManager
        private float _detectionRadius;

        private NavMeshAgent _agent; // NavMesh agent
        private bool _isChasing; // Status indicating if the ghost is wandering or chasing _pacman

        // START FUNCTION
        void Start() {
            Debug.Log("Ghost START");
            
            // Binding the NavMeshAgent component
            _agent = GetComponent<NavMeshAgent>();
            
            // if (_pacman == null) {
            //     GameObject pacmanObj = GameObject.FindWithTag("Player");
            //     if (pacmanObj != null) {
            //         _pacman = pacmanObj;
            //     } else {
            //         Debug.LogError("Pacman not found by ghost!");
            //     }
            // }
        }

        // UPDATE FUNCTION
        void Update() {
            // Check the distance between this ghost and pacman target
            float distance = Vector3.Distance(transform.position, _pacman.transform.position);
            // Debug.Log("Distance: " + distance);

            if (distance <= _detectionRadius) {
                // Chase the pacman
                if (!_isChasing) {
                    _isChasing = true;
                    // Debug.Log("Pacman detected — starting chase!");
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
                    // Debug.Log("Pacman escaped — stopping.");
                }
            }
        }

        /**
         * Sets the pacman game object.
         * Used in GhostManager when initializing the map.
         */
        public void SetPacman(GameObject pacman) {
            _pacman = pacman;
        }
        
        /**
         * Sets the normal speed, chasing speed and detection radius of the ghost.
         * Used in GhostManager when initializing the map.
         */
        public void SetGhostParams(float normalSpeed, float chaseSpeed, float detectionRadius) {
            _normalSpeed = normalSpeed;
            _chaseSpeed = chaseSpeed;
            _detectionRadius = detectionRadius;
        }
    }
}