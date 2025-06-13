using UnityEngine;

namespace Entity.Ghostron.State.StateImpl {
    /**
     * Ghostron State - Chase state.
     */
    public class ChaseState : IGhostronState {
        private float _chaseSpeed;
        private float _maximalChaseDuration; // When the chasing Ghostron hits this time, it should begin wandering
        private float _timer;

        // Animation speed when the Ghostron is in chase state
        private const float ChaseAnimationSpeed = 1f;

        /**
         * Action when entering chase state.
         */
        public void Enter(Ghostron ghostron) {
            _timer = 0f; // Reset timer

            // Set params
            _maximalChaseDuration = ghostron.MaximalChaseDuration;
            _chaseSpeed = ghostron.chaseSpeed;

            // Set Ghostron animator and agent speed
            ghostron.animator.speed = ChaseAnimationSpeed;
            ghostron.agent.speed = _chaseSpeed;
        }

        /**
         * Update() event when in chase state.
         * Called each frame.
         */
        public void Update(Ghostron ghostron) {
            // Update speed, as an event could just start/end
            _chaseSpeed = ghostron.chaseSpeed;
            ghostron.agent.speed = _chaseSpeed;
            
            // Set the chase target (real time position of the Pacboy)
            ghostron.MoveTo(ghostron.pacboy.transform.position);
            
            // Update timer
            _timer += Time.deltaTime;
            
            // If chasing time reaches the maximum chasing duration
            // Stop the chase and enter normal wander state
            if (_timer >= _maximalChaseDuration) {
                // Enter normal wander state
                ghostron.StateMachine.ChangeState(new NormalWanderState());
            }
        }

        /**
         * Action when exiting chase state.
         */
        public void Exit(Ghostron ghostron) {
            _timer = 0f; // Reset timer
        }
    }
}