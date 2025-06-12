using Entity.Ghostron.State;

namespace Entity.Ghostron {
    /**
     * The Ghostron Finite State Machine.
     * Manages the state switching of each Ghostron.
     * FOUR STATES: Normal Wander, Scared Wander, Chase, Stall
     */
    public class GhostronStateMachine {
        // Current state
        private IGhostronState _currentState;
        
        // Ghostron object
        private readonly Ghostron _ghostron;

        /* Constructor */
        public GhostronStateMachine(Ghostron ghostron) {
            _ghostron = ghostron;
        }

        /**
         * Switch to a new state.
         * Quits the current state (by calling its Exit(ghostron) function)
         * and enters the new state (by calling its Enter(ghostron) function).
         */
        public void ChangeState(IGhostronState newState) {
            _currentState?.Exit(_ghostron);
            _currentState = newState;
            _currentState.Enter(_ghostron);
        }

        /**
         * Returns the current state the machine is in.
         */
        public IGhostronState GetCurrentState() {
            return _currentState;
        }

        /**
         * Update() function, called each frame
         * Called by the Update() Unity event function in Ghostron script.
         * Calls the Update() in current state class to execute corresponding logic.
         */
        public void Update() {
            _currentState?.Update(_ghostron);
        }
    }
}