namespace Entity.Ghostron.State {
    /**
     * The interface for all Ghostron state classes.
     */
    public interface IGhostronState {
        // Enter a state
        void Enter(Ghostron ghostron);
        
        // Each frame during a state
        void Update(Ghostron ghostron);
        
        // Exit a state
        void Exit(Ghostron ghostron);
    }
}