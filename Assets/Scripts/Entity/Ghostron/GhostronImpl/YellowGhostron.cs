using UnityEngine;

namespace Entity.Ghostron.GhostronImpl {
    public class YellowGhostron : Ghostron {
        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Yellow Ghostron:
         * Always wandering towards Pacboy current location.
         *
         *
         */
        protected override Vector3 GenerateWanderingTarget() {
            if (Pacboy != null) {
                return Pacboy.transform.position;
            }

            return transform.position;
        }
    }
}