using UnityEngine;

namespace Entity.Ghostron.GhostronImpl {
    public class YellowGhostron : Ghostron {
        // Wander interval of the yellow ghostron
        protected override float WanderInterval {
            get { return 30.0f; }
        }

        // Scared duration of the yellow ghostron
        protected override float ScaredDuration {
            get { return 6.0f; }
        }

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Yellow Ghostron:
         * Always go to the position that the Pacboy has been to.
         */
        protected override Vector3 GenerateWanderingTarget() {
            if (Pacboy != null) {
                return Pacboy.transform.position;
            }

            return transform.position;
        }
    }
}