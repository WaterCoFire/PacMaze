using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron {
    /**
     * Manages the behaviour of the ghost.
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
        private readonly float _wanderInterval = 20.0f; // Interval of switching a wandering target

        private NavMeshAgent _agent; // NavMesh agent

        private bool _isChasing; // Status indicating if the ghostron is wandering or chasing pacman

        // Scared logic variables
        private bool _isScared; // Status indicating if the pacman is scared (when pacman eats a power pellet)
        private float _scaredTimer; // Timer of the ghostron feeling scared
        private readonly float _scaredDuration = 6.0f; // Duration of the "scared" effect of ghostrons
        private bool _isCaught; // Status indicating if the pacman has caught the ghostron when it is scared

        private readonly float
            _scaredWarningTime = 2.0f; // The time before the scared state ends to start warning (in secs)

        // The original material of the ghostron (the normal color)
        public Material originalMaterial;

        // Material when the ghostron is scared
        public Material scaredMaterial;

        // Ghostron animation logic (walking/spawning animation etc.)
        private Animator _animator; // Animator

        // Animation speed when the ghostron is in different statuses
        private readonly float _normalAnimationSpeed = 0.6f; // Wandering
        private readonly float _chaseAnimationSpeed = 1f; // Chasing pacman
        private readonly float _scaredAnimationSpeed = 0.3f; // Scared

        // START FUNCTION
        void Start() {
            Debug.Log("Ghostron START");

            // Check if the ghostron has all necessary components
            if (gameObject.GetComponent<NavMeshAgent>() == null ||
                gameObject.GetComponent<BoxCollider>() == null ||
                gameObject.GetComponent<Animator>() == null) {
                Debug.LogError("Ghostron start error: Essential components missing!");
                return;
            }

            // Set the ghost as a trigger
            gameObject.GetComponent<BoxCollider>().isTrigger = true;

            // Bind the NavMeshAgent component
            _agent = GetComponent<NavMeshAgent>();

            // Bind the animator
            _animator = gameObject.GetComponent<Animator>();

            // Set to the original material
            SetOriginalMaterial();
        }

        // UPDATE FUNCTION
        void Update() {
            // Get the animator state info
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // Caught logic
            if (_isCaught) {
                if (stateInfo.IsName("anim_close")) {
                    // Caught ghostron has finished the closing animation
                    if (stateInfo.normalizedTime >= 1f) {
                        Debug.Log("Caught ghostron finished closing animation.");

                        // Let the ghostron play initializing animation there
                        _animator.SetBool("Open_Anim", true);
                        _animator.SetBool("Walk_Anim", true);
                    }
                }
            }

            // Check if the ghostron is in initializing (opening itself) animation
            // Not in walking animation status means that the ghostron is initializing
            // Directly return if so
            if (!stateInfo.IsName("anim_Walk_Loop")) return;

            // Check the distance between this ghostron and pacman target
            float distance = Vector3.Distance(gameObject.transform.position, _pacman.transform.position);

            // Only chases pacman if the ghostron is close enough & not scared
            if (distance <= _detectionRadius && !_isScared) {
                // Chase the pacman
                if (!_isChasing) {
                    _isChasing = true;
                    Debug.Log("Pacman detected, starting chase!");
                }

                // Set the chasing speed
                _agent.speed = _chaseSpeed;

                // Set the animator speed
                _animator.speed = _chaseAnimationSpeed;

                // Set the chase target (real time position of the pacman)
                _agent.SetDestination(_pacman.transform.position);
            } else if (!_isCaught) {
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

            // Update the scared timer if the ghost is currently scared
            if (_isScared) {
                _scaredTimer += Time.deltaTime;

                // Check if the scared ghostron is in the last two seconds of the scared state
                if (_scaredTimer >= _scaredDuration - _scaredWarningTime) {
                    // Warning player about scared status ending soon
                    if (_scaredTimer < _scaredDuration) {
                        Debug.LogWarning("Warning: Ghostron scared state will end soon!");
                        // Make the skin swap between normal and scared materials
                        SetRandomMaterialDuringScared();
                    }
                }

                // If the timer reaches the scared duration time, stop being scared
                if (_scaredTimer >= _scaredDuration) {
                    SetScared(false);
                }
            }
        }

        /**
         * Sets the scared status of the ghost.
         */
        public void SetScared(bool isScared) {
            if (isScared) {
                // Now the ghostron becomes scared
                _scaredTimer = 0f;
                _isScared = true;
                // Change skin
                SetScaredMaterial();
                // Change movement speed
                _agent.speed = _scaredSpeed;
                // Change animator speed
                _animator.speed = _scaredAnimationSpeed;
            } else {
                // Now the ghostron is no longer scared
                _scaredTimer = 0f;
                _isScared = false;
                _isCaught = false;
                // Change back skin
                SetOriginalMaterial();
                // Change animator speed
                _animator.speed = _normalAnimationSpeed;
            }
        }

        /**
         * Randomly switch the materials between original and scared materials
         * during the last 2 seconds of the scared state.
         */
        private void SetRandomMaterialDuringScared() {
            if (!_isScared) {
                Debug.LogError("Ghostron is not scared but random skin setting function called.");
            }

            if (_scaredTimer % 0.5f < 0.25f) {
                // Alternating the material every 0.25 seconds
                SetOriginalMaterial();
            } else {
                SetScaredMaterial();
            }
        }

        /**
         * Unity event: When ghostron collides with another game object
         * Only pacman is cared here
         */
        private void OnTriggerEnter(Collider other) {
            // If the other game object is not pacman then do nothing
            if (!other.CompareTag("Pacman")) return;

            // It is pacman
            if (_isScared) {
                // The ghostron is scared currently
                // This means that the ghostron is caught by the pacman
                if (_isCaught) return; // No action if the ghostron is just caught
                Debug.Log("Ghostron caught by Pacman!");
                _agent.speed = 0f;

                // Corresponding animation
                // Ghostron "closes" itself
                _animator.speed = _normalAnimationSpeed;
                _animator.SetBool("Open_Anim", false);
                _animator.SetBool("Walk_Anim", false);

                _isCaught = true; // Update status

                // Give the pacman 200 score points
                PlayMapController.Instance.AddScore(200);

                // After isCaught is updated:
                // The Update() function keeps tracking if the "closing" animation of the ghostron is over or not
                // After that animation is over, ghostron will "open" again
            } else {
                // The ghostron is not currently scared
                // This means the game should be over
                Debug.LogWarning("Pacman got caught! GAME OVER!");
                GhostronManager.Instance.PacmanCaught();
            }
        }

        /**
         * The wandering logic.
         * (Re)start a wander process.
         */
        private void Wander() {
            _wanderTimer += Time.deltaTime;

            // Set wandering speed
            _agent.speed = _normalSpeed;

            // Set animator speed
            _animator.speed = _normalAnimationSpeed;

            // Wandering destination setting
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
         * Used in GhostronManager when initializing the map / adding new ghostron due to bad cherry.
         */
        public void SetPacman(GameObject pacman) {
            Debug.LogWarning("NEW GHOSTRON PACMAN SET");
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

        /**
         * Change the "skin" of the ghostron to purple (scared).
         */
        private void SetScaredMaterial() {
            // Check if both materials are properly set
            if (scaredMaterial == null || originalMaterial == null) {
                Debug.LogError("Error: Original/Scared material not properly set!");
                return;
            }

            // Get all MeshRenderers in children
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);

            // Change all the original materials in all MeshRenderers to scared ones
            foreach (MeshRenderer renderer in renderers) {
                Material[] currentMats = renderer.sharedMaterials;
                bool hasChange = false;
                Material[] newMats = new Material[currentMats.Length];

                for (int i = 0; i < currentMats.Length; i++) {
                    if (currentMats[i] == originalMaterial) {
                        newMats[i] = scaredMaterial;
                        hasChange = true;
                    } else {
                        newMats[i] = currentMats[i];
                    }
                }

                if (hasChange) {
                    renderer.sharedMaterials = newMats;
                }
            }
        }

        /**
         * Change the "skin" of the ghostron back to the original one.
         */
        private void SetOriginalMaterial() {
            // Check if both materials are properly set
            if (scaredMaterial == null || originalMaterial == null) {
                Debug.LogError("Error: Original/Scared material not properly set!");
                return;
            }

            // Get all MeshRenderers in children
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);

            // Change all the scared materials in all MeshRenderers to original ones
            foreach (MeshRenderer renderer in renderers) {
                Material[] currentMats = renderer.sharedMaterials;
                bool hasChange = false;
                Material[] newMats = new Material[currentMats.Length];

                for (int i = 0; i < currentMats.Length; i++) {
                    if (currentMats[i] == scaredMaterial) {
                        newMats[i] = originalMaterial;
                        hasChange = true;
                    } else {
                        newMats[i] = currentMats[i];
                    }
                }

                if (hasChange) {
                    renderer.sharedMaterials = newMats;
                }
            }
        }
    }
}