namespace Entity.Ghostron.State {
    /**
     * The interface for all Ghostron state classes.
     */
    public interface IGhostronState {
        void Enter(Ghostron ghostron);
        void Update(Ghostron ghostron);
        void Exit(Ghostron ghostron);
    }
}