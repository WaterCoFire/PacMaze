using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron {
    /**
     * Manages the behaviour of the ghost.
     */
    public abstract class Ghostron : MonoBehaviour {
        protected GameObject Pacboy; // Pacboy game object, what the ghostron is hunting for

        // Normal wandering speed of the ghostron
        private float _normalSpeed;

        // Speed when the ghostron is scared (Pacboy eats a power pellet)
        private float _scaredSpeed;

        // Chasing Speed
        // This speed varies according to difficulty
        // Specific numbers are set in GhostronManager
        private float _chaseSpeed;

        // Detection Radius
        // Pacboy will be chased when it is within this distance from a ghostron
        // This distance varies according to difficulty
        // Specific numbers are set in GhostronManager
        private float _detectionRadius;

        // Wandering logic variables
        private float _wanderTimer; // Timer of the current wandering

        // Interval of switching a wandering target
        protected virtual float WanderInterval {
            get { return 0f; }
        }

        private NavMeshAgent _agent; // NavMesh agent

        private bool _isChasing; // Status indicating if the ghostron is wandering or chasing Pacboy

        // Scared logic variables
        private bool _isScared; // Status indicating if the Pacboy is scared (when Pacboy eats a power pellet)
        private float _scaredTimer; // Timer of the ghostron feeling scared

        // Duration of the "scared" effect of ghostrons
        protected virtual float ScaredDuration {
            get { return 0f; }
        }

        private bool _isCaught; // Status indicating if the Pacboy has caught the ghostron when it is scared

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
        private readonly float _chaseAnimationSpeed = 1f; // Chasing Pacboy
        private readonly float _scaredAnimationSpeed = 0.3f; // Scared

        // Event logic - Crazy Party active/disactive (Double the speed)
        private bool _crazyParty;

        /**
         * Generates a position on the map.
         * Used for getting a target when the ghostron is wandering.
         * ABSTRACT FUNCTION - Different color ghostrons should have different behaviours
         */
        protected abstract Vector3 GenerateWanderingTarget();

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

            _crazyParty = false; // No Crazy Party event by default

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
                // Set scared status
                SetOriginalMaterial();
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

            // Check the distance between this ghostron and Pacboy target
            float distance = Vector3.Distance(gameObject.transform.position, Pacboy.transform.position);

            // Only chases Pacboy if the ghostron is close enough & not scared
            if (distance <= _detectionRadius && !_isScared) {
                // Chase the Pacboy
                if (!_isChasing) {
                    _isChasing = true;
                    Debug.Log("Pacboy detected, starting chase!");
                }

                // Set the chasing speed
                _agent.speed = _chaseSpeed;

                // Set the animator speed
                _animator.speed = _chaseAnimationSpeed;

                // Set the chase target (real time position of the Pacboy)
                _agent.SetDestination(Pacboy.transform.position);
            } else if (!_isCaught) {
                // Pacboy is out of the chasing detection radius
                // Impossible if in HARD game mode because the radius is long enough
                if (_isChasing) {
                    _isChasing = false;
                    _agent.ResetPath();
                    Debug.Log("Pacboy escaped, now wandering.");
                }

                // Start to wander
                Wander();
            }

            // Update the scared timer if the ghost is currently scared
            if (_isScared) {
                _scaredTimer += Time.deltaTime;

                // Check if the scared ghostron is in the last two seconds of the scared state
                if (_scaredTimer >= ScaredDuration - _scaredWarningTime) {
                    // Warning player about scared status ending soon
                    if (_scaredTimer < ScaredDuration) {
                        Debug.LogWarning("Warning: Ghostron scared state will end soon!");
                        // Make the skin swap between normal and scared materials
                        SetRandomMaterialDuringScared();
                    }
                }

                // If the timer reaches the scared duration time, stop being scared
                if (_scaredTimer >= ScaredDuration) {
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
         * Only Pacboy is cared here
         */
        private void OnTriggerEnter(Collider other) {
            // If the other game object is not Pacboy then do nothing
            if (!other.CompareTag("Pacboy")) return;

            // It is Pacboy
            if (_isScared) {
                // The ghostron is scared currently
                // This means that the ghostron is caught by the Pacboy
                if (_isCaught) return; // No action if the ghostron is just caught
                Debug.Log("Ghostron caught by Pacboy!");
                _agent.speed = 0f;

                // Corresponding animation
                // Ghostron "closes" itself
                _animator.speed = _normalAnimationSpeed;
                _animator.SetBool("Open_Anim", false);
                _animator.SetBool("Walk_Anim", false);

                _isCaught = true; // Update status

                // Give the Pacboy 200 score points
                PlayMapController.Instance.AddScore(200);

                // After isCaught is updated:
                // The Update() function keeps tracking if the "closing" animation of the ghostron is over or not
                // After that animation is over, ghostron will "open" again
            } else {
                // The ghostron is not currently scared
                // This means the game should be over
                Debug.LogWarning("Pacboy got caught! GAME OVER!");
                GhostronManager.Instance.PacboyCaught();
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
            if (!_agent.hasPath || _agent.remainingDistance < 0.5f || _wanderTimer >= WanderInterval) {
                Vector3 newDestination = GenerateWanderingTarget();
                _agent.SetDestination(newDestination);
                _wanderTimer = 0f;
            }
        }

        /**
         * Sets the Pacboy game object.
         * Used in GhostronManager when initializing the map / adding new ghostron due to bad cherry.
         */
        public void SetPacboy(GameObject pacboy) {
            Debug.LogWarning("NEW GHOSTRON PACBOY SET");
            Pacboy = pacboy;
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

        /**
         * Sets the status of Crazy Party.
         * Called by EventManager via GhostronManager when the Crazy Party should be on/off.
         */
        public void SetCrazyParty(bool on) {
            if (on) {
                if (_crazyParty) {
                    Debug.LogError("Error: Crazy Party is already active!");
                    return;
                }

                // Double the speed
                _crazyParty = true;
                _normalSpeed *= 2;
                _scaredSpeed *= 2;
                _chaseSpeed *= 2;
            } else {
                if (!_crazyParty) {
                    Debug.LogError("Error: Crazy Party is not active!");
                    return;
                }
                
                // Set back to normal speed
                _crazyParty = false;
                _normalSpeed /= 2;
                _scaredSpeed /= 2;
                _chaseSpeed /= 2;
            }
        }
    }
}