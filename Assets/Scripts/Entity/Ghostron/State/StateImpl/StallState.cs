using PlayMap;
using UnityEngine;

namespace Entity.Ghostron.State.StateImpl {
    /**
     * Stall state.
     * (Pacboy catches the scared Ghostron)
     */
    public class StallState : IGhostronState {
        // Normal animation speed of Ghostron
        // Ghostron will display closing/opening animation at this speed
        private readonly float _normalAnimationSpeed = 0.6f;
        
        /**
         * Action when entering stalling state.
         */
        public void Enter(Ghostron ghostron) {
            // Speed and skin setting
            ghostron.agent.speed = 0f;
            ghostron.SetScaredMaterial();

            // Corresponding animation
            // Ghostron "closes" itself
            ghostron.animator.speed = _normalAnimationSpeed;
            ghostron.animator.SetBool("Open_Anim", false);
            ghostron.animator.SetBool("Walk_Anim", false);

            // Give the Pacboy 200 score points
            PlayMapController.Instance.AddScore(200);

            // The Update() function keeps tracking if the "closing" animation of the ghostron is over or not
            // After that animation is over, ghostron will "open" again
        }

        /**
         * Update() event when in stalling state.
         */
        public void Update(Ghostron ghostron) {
            // Check if the Ghostron "closing" animation has finished
            // If so, play "opening" animation and enter normal chasing state
            AnimatorStateInfo stateInfo = ghostron.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("anim_close")) {
                // Caught ghostron has finished the closing animation
                if (stateInfo.normalizedTime >= 1f) {
                    Debug.Log("Caught ghostron finished closing animation.");

                    // Let the ghostron play initialising ("opening") animation
                    ghostron.animator.SetBool("Open_Anim", true);
                    ghostron.animator.SetBool("Walk_Anim", true);
                    
                    // Enter normal wander state
                    ghostron.StateMachine.ChangeState(new NormalWanderState());
                }
            }
        }

        /**
         * Action when exiting stalling state.
         */
        public void Exit(Ghostron ghostron) {
            // Set to normal material
            ghostron.SetOriginalMaterial();
        }
    }
}