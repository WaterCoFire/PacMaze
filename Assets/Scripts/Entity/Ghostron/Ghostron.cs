using Entity.Ghostron.State.StateImpl;
using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron {
    /**
     * Manages the behaviour of the ghost.
     */
    public abstract class Ghostron : MonoBehaviour {
        public GhostronStateMachine StateMachine { get; private set; }
        
        public GameObject pacboy; // Pacboy game object, what the ghostron is hunting for

        // Normal wandering speed of the ghostron
        public float normalWanderSpeed;

        // Speed when the ghostron is scared (Pacboy eats a power pellet)
        public float scaredWanderSpeed;

        // Chasing Speed
        // This speed varies according to difficulty
        // Specific numbers are set in GhostronManager
        public float chaseSpeed;

        // Detection Radius
        // Pacboy will be chased when it is within this distance from a ghostron
        // This distance varies according to difficulty
        // Specific numbers are set in GhostronManager
        public float detectionRadius;

        // Interval of switching a wandering target
        // VIRTUAL - different for every implement class (ghostron type)
        public virtual float WanderInterval {
            get { return 0f; }
        }

        // Minimum wander duration
        // The ghostron must wander for at least this time for every wander
        // VIRTUAL - different for every implement class (ghostron type) & DIFFICULTY
        public virtual float MinimumWanderDuration { get; }

        // Maximum chase duration
        // When the chasing ghostron hits this time, it should begin wandering
        // VIRTUAL - different for every implement class (ghostron type) & DIFFICULTY
        public virtual float MaximalChaseDuration { get; }

        public NavMeshAgent agent; // NavMesh agent of the Ghostron

        // If the Ghostron is currently scared
        protected bool IsScared; // Status indicating if the Pacboy is scared (when Pacboy eats a power pellet)

        // Duration of the "scared" effect of ghostrons
        // VIRTUAL - different for every implement class (ghostron type)
        public virtual float ScaredDuration {
            get { return 0f; }
        }

        // The original material of the ghostron (the normal color)
        public Material originalMaterial;

        // Material when the ghostron is scared
        public Material scaredMaterial;

        // Ghostron animation logic (walking/spawning animation etc.)
        public Animator animator; // Animator

        // Event logic - Crazy Party active/disactive (Double the speed)
        private bool _crazyParty;

        /**
         * Generates a position on the map.
         * Used for getting a target when the ghostron is wandering.
         * ABSTRACT FUNCTION - Different color ghostrons should have different behaviours
         */
        public abstract Vector3 GenerateWanderingTarget();

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
            agent = GetComponent<NavMeshAgent>();

            // Bind the animator
            animator = gameObject.GetComponent<Animator>();

            // Set to the original material
            SetOriginalMaterial();
            
            // Initialise its State Machine
            StateMachine = new GhostronStateMachine(this);
            
            // Enter normal wander state by default
            StateMachine.ChangeState(new NormalWanderState());
        }

        // UPDATE FUNCTION
        void Update() {
            // Execute the Update() logic in the current state
            StateMachine.Update();
        }

        /**
         * Scare the Ghostron.
         */
        public void Scare() {
            IsScared = true;
            // Enter the scared wander state
            StateMachine.ChangeState(new ScaredWanderState());
        }

        /**
         * Unity event: When ghostron collides with another game object
         * Only Pacboy is cared here
         */
        private void OnTriggerEnter(Collider other) {
            // If the other game object is not Pacboy then do nothing
            if (!other.CompareTag("Pacboy")) return;

            if (IsScared) {
                // If the Ghostron is scared currently
                // Pacboy catches it, it should enter stall state
                StateMachine.ChangeState(new StallState());
            } else {
                // If the Ghostron is not currently scared
                // This means the game should be over
                Debug.LogWarning("Pacboy got caught! GAME OVER!");
                GhostronManager.Instance.PacboyCaught();
            }
        }

        /**
         * Sets the Pacboy game object.
         * Used in GhostronManager when initializing the map / adding new ghostron due to bad cherry.
         */
        public void SetPacboy(GameObject pacboy) {
            Debug.Log("NEW GHOSTRON PACBOY SET");
            this.pacboy = pacboy;
        }

        /**
         * Sets the normal speed, scared speed, chasing speed and detection radius of the ghostron.
         * Used in GhostronManager when initializing the map.
         */
        public void SetGhostronParams(float normalSpeed, float scaredSpeed, float chaseSpeed, float detectionRadius) {
            normalWanderSpeed = normalSpeed;
            scaredWanderSpeed = scaredSpeed;
            this.chaseSpeed = chaseSpeed;
            this.detectionRadius = detectionRadius;
        }

        /**
         * Change the "skin" of the ghostron to purple (scared).
         */
        public void SetScaredMaterial() {
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
        public void SetOriginalMaterial() {
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
                normalWanderSpeed *= 3;
                scaredWanderSpeed *= 3;
                chaseSpeed *= 3;
            } else {
                if (!_crazyParty) {
                    // Directly return, as there could be some tenacious ghostrons spawned during Crazy Party
                    return;
                }

                // Set back to normal speed
                _crazyParty = false;
                normalWanderSpeed /= 3;
                scaredWanderSpeed /= 3;
                chaseSpeed /= 3;
            }
        }

        /**
         * Update the destination of the ghostron.
         * Called by multiple state classes.
         */
        public void MoveTo(Vector3 target) {
            agent.SetDestination(target);
        }
    }
}