using Sound;
using UnityEngine;
using UnityEngine.AI;

namespace Tutorial.Entities {
    /**
     * Ghostron in tutorial
     * Note: Not using FSM mechanism as Ghostrons in tutorial have much easier behaviours
     */
    public class TutorialGhostron : MonoBehaviour {
        public GameObject pacboy; // Pacboy game object, what the Ghostron should chase

        // Chasing Speed (very slow as this is tutorial)
        private float _chaseSpeed = 2.5f;

        private NavMeshAgent _agent; // NavMesh agent of the Ghostron

        // The original material of the Ghostron (the normal color)
        public Material originalMaterial;

        // Material when the Ghostron is scared
        public Material scaredMaterial;

        // Animator
        private Animator _animator;

        // Whether the Ghostron is chasing or not
        private bool _chasing;
        
        // Whether the Ghostron is scared or not
        private bool _scared;

        // START FUNCTION
        private void Start() {
            // Check if the Ghostron has all necessary components
            if (gameObject.GetComponent<NavMeshAgent>() == null ||
                gameObject.GetComponent<BoxCollider>() == null ||
                gameObject.GetComponent<Animator>() == null) {
                Debug.LogError("TutorialGhostron start error: Essential components missing!");
                return;
            }

            // Set the Ghostron as a trigger
            gameObject.GetComponent<BoxCollider>().isTrigger = true;

            // Bind the NavMeshAgent component
            _agent = GetComponent<NavMeshAgent>();

            // Bind the animator
            _animator = gameObject.GetComponent<Animator>();

            // Set to the original material
            SetOriginalMaterial();

            // Initialise the Ghostron animator (Make it open and stalled)
            _animator.speed = 0f;

            // Do not chase (stall) and not be scared by default
            _chasing = false;
            _scared = false;
        }

        // UPDATE FUNCTION
        private void Update() {
            // No action if not chasing
            if (!_chasing) return;

            // Set the chase target (real time position of the Pacboy)
            _agent.SetDestination(pacboy.transform.position);
        }
        
        /**
         * Unity event: When Ghostron collides with another game object
         * Only Pacboy is cared here
         * If Ghostron is scared: Stall it.
         * If Ghostron is NOT scared: Do nothing. This is tutorial so player won't lose.
         */
        private void OnTriggerEnter(Collider other) {
            // If the other game object is not Pacboy then do nothing
            if (!other.CompareTag("Pacboy")) return;
            
            // The other game object is indeed Pacboy
            // Check if the Ghostron is currently scared & not stalling
            if (_scared && _agent.speed != 0f) {
                // If Ghostron is currently scared, then Pacboy catches the Ghostron
                // Play Ghostron caught sound
                SoundManager.Instance.PlaySoundOnce(SoundType.GhostronCaught);
                
                // Stall the Ghostron
                _animator.speed = 0.7f;
                _agent.speed = 0f;
                _agent.angularSpeed = 0f;
                _animator.SetBool("Open_Anim", false);
                _animator.SetBool("Walk_Anim", false);
            
                // Close action prompt as task (catch a scared Ghostron) is completed
                TutorialUI.Instance.CloseActionPrompt();
            }
            // Note: no action if Ghostron is not scared. This is tutorial!
        }

        /**
         * Let the Ghostron "chase" the Pacboy.
         */
        public void EnableChase() {
            _chasing = true;
            // Set Ghostron animator and agent speed
            _animator.speed = 0.6f;
            _agent.speed = _chaseSpeed;
        }

        /**
         * Set the scared status of the Ghostron.
         * Change the "skin" of the Ghostron to purple (scared).
         */
        public void SetScared() {
            _scared = true;
            EnableChase();
            
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
         * Change the "skin" of the Ghostron back to the original one.
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
    }
}