using UnityEngine;

namespace Entity.Ghostron.GhostronImpl {
    public class PinkGhostron : Ghostron {
        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Pink Ghostron:
         * Go to the center point.
         */
        protected override Vector3 GenerateWanderingTarget() {
            Vector3 centerPosition = new(0, 0, 0);
            return centerPosition;
        }
    }
}