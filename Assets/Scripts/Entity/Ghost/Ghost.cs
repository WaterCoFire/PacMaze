using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghost {
    /**
     * Manages the behaviour of the ghost.
     */
    public class Ghost : MonoBehaviour {
        private GameObject _pacman; // Pacman game object, what the ghost is hunting for
        
        // Detection Radius
        // Pacman will be chased when it is within this distance from a ghost
        // This distance varies according to difficulty
        // EASY: TODO
        // NORMAL: TODO
        // HARD: (infinite) TODO
        public float detectionRadius = 5f;

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
            float distance = Vector3.Distance(transform.position, _pacman.transform.position);
            Debug.Log("Distance: " + distance);

            if (distance <= detectionRadius) {
                if (!_isChasing) {
                    _isChasing = true;
                    Debug.Log("Pacman detected — starting chase!");
                }

                _agent.SetDestination(_pacman.transform.position);
            } else {
                if (_isChasing) {
                    _isChasing = false;
                    _agent.ResetPath();
                    Debug.Log("Pacman escaped — stopping.");
                }
            }
        }

        /**
         * Sets the pacman game object.
         * Used in PropGenerating when initializing the map.
         */
        public void SetPacman(GameObject pacman) {
            _pacman = pacman;
        }
    }
}