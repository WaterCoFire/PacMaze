using UnityEngine;
using UnityEngine.AI;

namespace Tutorial.Entities {
    /**
     * Ghostron in tutorial
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

        // START FUNCTION
        void Start() {
            Debug.Log("Ghostron START");

            // Check if the Ghostron has all necessary components
            if (gameObject.GetComponent<NavMeshAgent>() == null ||
                gameObject.GetComponent<BoxCollider>() == null ||
                gameObject.GetComponent<Animator>() == null) {
                Debug.LogError("Ghostron start error: Essential components missing!");
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
            
            // Do not chase (stall) by default
            _chasing = false;
        }

        // UPDATE FUNCTION
        private void Update() {
            // No action if not chasing
            if (!_chasing) return;
            
            // Set the chase target (real time position of the Pacboy)
            _agent.SetDestination(pacboy.transform.position);
        }

        /**
         * Let the Ghostron "chase" the Pacboy.
         */
        public void EnableChase() {
            _chasing = true;
            // Set Ghostron animator and agent speed
            _animator.speed = 1f;
            _agent.speed = _chaseSpeed;
        }

        /**
         * Change the "skin" of the Ghostron to purple (scared).
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