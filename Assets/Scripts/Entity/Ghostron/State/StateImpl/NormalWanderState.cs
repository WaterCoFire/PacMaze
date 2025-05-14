using UnityEngine;

namespace Entity.Ghostron.State.StateImpl {
    /**
     * Normal wander state.
     */
    public class NormalWanderState : IGhostronState {
        private Vector3 _wanderTarget;
        public float WanderInterval;
        public float NormalWanderSpeed;
        public float DetectionRadius;
        public float MinimumWanderDuration; // The ghostron must wander for at least this time for every wander
        private float _timer;
        private bool _chaseAllowed;
        
        // Animation speed when the ghostron is in normal wander state
        private readonly float _normalAnimationSpeed = 0.6f;

        /**
         * Action when entering normal wander state.
         */
        public void Enter(Ghostron ghostron) {
            _timer = 0f; // Reset timer
            
            // Set params
            ghostron.isScared = false;
            WanderInterval = ghostron.WanderInterval;
            NormalWanderSpeed = ghostron.normalWanderSpeed;
            DetectionRadius = ghostron.detectionRadius;
            MinimumWanderDuration = ghostron.MinimumWanderDuration;
            _chaseAllowed = false;
            
            // Set normal material
            ghostron.SetOriginalMaterial();

            // Set Ghostron animator and agent speed
            ghostron.animator.speed = _normalAnimationSpeed;
            ghostron.agent.speed = NormalWanderSpeed;
            
            // Set a new wandering destination
            _wanderTarget = ghostron.GenerateWanderingTarget();
        }

        /**
         * Update() event when in normal wander state.
         */
        public void Update(Ghostron ghostron) {
            // Get the animator state info
            // Check if the ghostron is in initialising (opening itself) animation
            // Not in walking animation status means that the ghostron is initialising
            // Directly return if so
            AnimatorStateInfo stateInfo = ghostron.animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("anim_Walk_Loop")) return;
            
            // Move the Ghostron towards its wandering target
            ghostron.MoveTo(_wanderTarget);
            
            // Check the distance between this ghostron and Pacboy target
            float distance = Vector3.Distance(ghostron.gameObject.transform.position, ghostron.pacboy.transform.position);

            // Check if the Ghostron should enter chase state (to start chasing)
            // Only chases Pacboy if the Ghostron is close enough & has reached minimum chasing duration (judged by _chaseAllowed)
            if (distance <= DetectionRadius && _chaseAllowed) {
                // Enter chase state
                ghostron.StateMachine.ChangeState(new ChaseState());
                return;
            }
            
            // Update wander timer
            _timer += Time.deltaTime;
            
            // If the timer reaches the minimum limit, allow chasing to happen
            if (_timer > MinimumWanderDuration) {
                _chaseAllowed = true;
            }
            
            // Re-select a wandering target if:
            // - The Ghostron already reaches its reaching destination
            // - Wandering interval time reached
            if (!ghostron.agent.hasPath || ghostron.agent.remainingDistance < 0.5f || _timer >= WanderInterval) {
                // Set a new wandering destination
                _wanderTarget = ghostron.GenerateWanderingTarget();
                ghostron.MoveTo(_wanderTarget);
                
                // Reset timer
                _timer = 0f;
            }
        }

        /**
         * Action when exiting normal wander state.
         */
        public void Exit(Ghostron ghostron) {
            // Reset params
            _timer = 0f;
            _chaseAllowed = false;
        }
    }
}