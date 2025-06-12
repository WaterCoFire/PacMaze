using UnityEngine;

namespace Entity.Ghostron.State.StateImpl {
    /**
     * Ghostron State - Chase state.
     */
    public class ChaseState : IGhostronState {
        public float ChaseSpeed;
        public float MaximalChaseDuration; // When the chasing Ghostron hits this time, it should begin wandering
        private float _timer;

        // Animation speed when the Ghostron is in chase state
        private readonly float _chaseAnimationSpeed = 1f;

        /**
         * Action when entering chase state.
         */
        public void Enter(Ghostron ghostron) {
            _timer = 0f; // Reset timer

            // Set params
            MaximalChaseDuration = ghostron.MaximalChaseDuration;
            ChaseSpeed = ghostron.chaseSpeed;

            // Set Ghostron animator and agent speed
            ghostron.animator.speed = _chaseAnimationSpeed;
            ghostron.agent.speed = ChaseSpeed;
        }

        /**
         * Update() event when in chase state.
         * Called each frame.
         */
        public void Update(Ghostron ghostron) {
            // Update speed, as an event could just start/end
            ChaseSpeed = ghostron.chaseSpeed;
            ghostron.agent.speed = ChaseSpeed;
            
            // Set the chase target (real time position of the Pacboy)
            ghostron.MoveTo(ghostron.pacboy.transform.position);
            
            // Update timer
            _timer += Time.deltaTime;
            
            // If chasing time reaches the maximum chasing duration
            // Stop the chase and enter normal wander state
            if (_timer >= MaximalChaseDuration) {
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