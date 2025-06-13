using UnityEngine;

namespace Entity.Ghostron.State.StateImpl {
    /**
     * Ghostron State - Normal wander state.
     */
    public class NormalWanderState : IGhostronState {
        private Vector3 _wanderTarget; // Wander destination
        private float _wanderInterval;
        private float _normalWanderSpeed;
        private float _detectionRadius;
        private float _minimumWanderDuration; // The Ghostron must wander for at least this time for every wander
        private float _timer;
        private bool _chaseAllowed; // Currently, chasing can be started or not
        
        // Animation speed when the Ghostron is in normal wander state
        private const float NormalAnimationSpeed = 0.6f;

        /**
         * Action when entering normal wander state.
         */
        public void Enter(Ghostron ghostron) {
            _timer = 0f; // Reset timer
            
            // Set params
            ghostron.isScared = false;
            _wanderInterval = ghostron.WanderInterval;
            _normalWanderSpeed = ghostron.normalWanderSpeed;
            _detectionRadius = ghostron.detectionRadius;
            _minimumWanderDuration = ghostron.MinimumWanderDuration;
            _chaseAllowed = false;
            
            // Set normal material
            ghostron.SetOriginalMaterial();

            // Set Ghostron animator and agent speed
            ghostron.animator.speed = NormalAnimationSpeed;
            ghostron.agent.speed = _normalWanderSpeed;
            
            // Set a new wandering destination
            _wanderTarget = ghostron.GenerateWanderingTarget();
        }

        /**
         * Update() event when in normal wander state.
         * Called each frame.
         */
        public void Update(Ghostron ghostron) {
            // Get the animator state info
            // Check if the Ghostron is in initialising (opening itself) animation
            // Not in walking animation status means that the Ghostron is initialising
            // Directly return if so
            AnimatorStateInfo stateInfo = ghostron.animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("anim_Walk_Loop")) return;
            
            // Update speed, as an event could just started/ended
            _normalWanderSpeed = ghostron.normalWanderSpeed;
            ghostron.agent.speed = _normalWanderSpeed;
            
            // Move the Ghostron towards its wandering target
            ghostron.MoveTo(_wanderTarget);
            
            // Check the distance between this Ghostron and Pacboy target
            float distance = Vector3.Distance(ghostron.gameObject.transform.position, ghostron.pacboy.transform.position);

            // Check if the Ghostron should enter chase state (to start chasing)
            // Only chases Pacboy if the Ghostron is close enough & has reached minimum chasing duration (judged by _chaseAllowed)
            if (distance <= _detectionRadius && _chaseAllowed) {
                // Enter chase state
                ghostron.StateMachine.ChangeState(new ChaseState());
                return;
            }
            
            // Update wander timer
            _timer += Time.deltaTime;
            
            // If the timer reaches the minimum limit, allow chasing to happen
            if (_timer > _minimumWanderDuration) {
                _chaseAllowed = true;
            }
            
            // Re-select a wandering target if:
            // - The Ghostron already reaches its reaching destination
            // - Wandering interval time reached
            if (!ghostron.agent.hasPath || ghostron.agent.remainingDistance < 0.5f || _timer >= _wanderInterval) {
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