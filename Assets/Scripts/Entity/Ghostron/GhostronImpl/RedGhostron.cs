using UnityEngine;

namespace Entity.Ghostron.GhostronImpl {
    public class RedGhostron : Ghostron {
        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Red Ghostron:
         * Go to the map corner which is the furthest away from the Pacboy.
         */
        protected override Vector3 GenerateWanderingTarget() {
            if (Pacboy != null) {
                // The four corners
                Vector3[] potentialPositions = {
                    new(-15, 0, -15), new(-15, 0, 15), new(15, 0, -15), new(15, 0, 15)
                };

                // Find the corner that is the furthest away from the Pacboy
                Vector3 furthestPosition = potentialPositions[0];
                float maxDistance = Vector3.Distance(furthestPosition, Pacboy.transform.position);

                foreach (var pos in potentialPositions) {
                    float distance = Vector3.Distance(pos, Pacboy.transform.position);
                    if (distance > maxDistance) {
                        furthestPosition = pos;
                        maxDistance = distance;
                    }
                }

                return furthestPosition;
            }

            return transform.position;
        }
    }
}