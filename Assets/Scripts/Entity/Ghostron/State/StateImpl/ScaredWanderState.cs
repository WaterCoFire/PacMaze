using UnityEngine;

namespace Entity.Ghostron.State.StateImpl {
    /**
     * Ghostron State - Scared wander state.
     * (Pacboy eats a Power Pellet)
     */
    public class ScaredWanderState : IGhostronState {
        private Vector3 _wanderTarget; // Wander destination
        private float _scaredWanderSpeed;
        private float _scaredDuration;
        private float _timer;

        // Animation speed when the Ghostron is in scared wander state
        private const float NormalAnimationSpeed = 0.3f;

        private const float WarningTime = 2.0f; // The time before the scared state ends to start warning (in secs)

        /**
         * Action when entering scared wander state.
         */
        public void Enter(Ghostron ghostron) {
            _timer = 0f; // Reset timer

            // Set params
            ghostron.isScared = true;
            _scaredWanderSpeed = ghostron.scaredWanderSpeed;
            _scaredDuration = ghostron.ScaredDuration;

            // Set scared material
            ghostron.SetScaredMaterial();

            // Set Ghostron animator and agent speed
            ghostron.animator.speed = NormalAnimationSpeed;
            ghostron.agent.speed = _scaredWanderSpeed;

            // Set a new destination and let Ghostron go there
            _wanderTarget = ghostron.GenerateWanderingTarget();
            ghostron.MoveTo(_wanderTarget);
        }

        /**
         * Update() event when in scared wander state.
         * Called each frame.
         */
        public void Update(Ghostron ghostron) {
            // Update speed, as an event could just started/ended
            _scaredWanderSpeed = ghostron.scaredWanderSpeed;
            ghostron.agent.speed = _scaredWanderSpeed;
            
            // Update timer
            _timer += Time.deltaTime;

            // Check if the scared Ghostron is in the last two seconds of the scared state
            if (_timer >= _scaredDuration - WarningTime) {
                // Warn player that the scared status will end soon
                // Make the skin swap between normal and scared materials
                if (_timer % 0.5f < 0.25f) {
                    // Alternating the material every 0.25 seconds
                    ghostron.SetOriginalMaterial();
                } else {
                    ghostron.SetScaredMaterial();
                }
            }

            // If the timer reaches the scared duration time, stop being scared
            if (_timer >= _scaredDuration) {
                // Enter normal wander state
                ghostron.StateMachine.ChangeState(new NormalWanderState());
            }
        }

        /**
         * Action when exiting scared wander state.
         */
        public void Exit(Ghostron ghostron) {
            _timer = 0f; // Reset timer
        }
    }
}