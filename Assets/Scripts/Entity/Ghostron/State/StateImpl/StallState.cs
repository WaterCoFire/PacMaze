using PlayMap;
using UnityEngine;

namespace Entity.Ghostron.State.StateImpl {
    /**
     * Ghostron State - Stall state.
     * (Pacboy catches the scared Ghostron)
     */
    public class StallState : IGhostronState {
        // Normal animation speed of Ghostron
        // Ghostron will display closing/opening animation at this speed
        private readonly float _normalAnimationSpeed = 0.6f;

        private bool _closed; // Ghostron has totally stalled, ready to re-open

        /**
         * Action when entering stalling state.
         */
        public void Enter(Ghostron ghostron) {
            // Speed and skin setting
            ghostron.agent.speed = 0f;
            ghostron.SetScaredMaterial();

            // Status setting
            _closed = false;

            // Corresponding animation
            // Ghostron "closes" itself
            ghostron.animator.speed = _normalAnimationSpeed;
            ghostron.animator.SetBool("Open_Anim", false);
            ghostron.animator.SetBool("Walk_Anim", false);

            // Give the Pacboy 200 score points
            PlayMapController.Instance.AddScore(200);

            // The Update() function keeps tracking if the "closing" animation of the Ghostron is over or not
            // After that animation is over, Ghostron will "open" again
        }

        /**
         * Update() event when in stalling state.
         * Called each frame.
         */
        public void Update(Ghostron ghostron) {
            AnimatorStateInfo stateInfo = ghostron.animator.GetCurrentAnimatorStateInfo(0);
            // Check if the Ghostron "closing" animation has finished
            // If so, play "opening" animation
            if (stateInfo.IsName("anim_close")) {
                // Caught Ghostron has finished the closing animation
                if (stateInfo.normalizedTime >= 1f) {
                    _closed = true;
                    // Let the Ghostron play initialising ("opening") animation
                    ghostron.animator.SetBool("Open_Anim", true);
                    ghostron.animator.SetBool("Walk_Anim", true);
                }
            }

            // Get the animator state info
            // Check if the Ghostron has finished the "opening animation"
            // If so, enter normal wander state
            if (_closed && stateInfo.IsName("anim_Walk_Loop")) {
                // Enter normal wander state
                ghostron.StateMachine.ChangeState(new NormalWanderState());
            }
        }

        /**
         * Action when exiting stalling state.
         */
        public void Exit(Ghostron ghostron) {
            // Status reset
            _closed = false;
            
            // Set to normal material
            ghostron.SetOriginalMaterial();
        }
    }
}