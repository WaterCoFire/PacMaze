using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron.GhostronImpl {
    public class GreenGhostron : Ghostron {
        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Green Ghostron:
         * Go to the middle point of the four quadrants of the map that is the nearest to the Pacboy.
         */
        protected override Vector3 GenerateWanderingTarget() {
            if (Pacboy != null) {
                // The middle point of the four quadrants
                Vector3[] potentialPositions = {
                    new(-9, 0, -9), new(-9, 0, 9), new(9, 0, -9), new(9, 0, 9)
                };

                Vector3 closestPosition = potentialPositions[0];
                float minDistance = Vector3.Distance(closestPosition, Pacboy.transform.position);

                // Find the nearest one to the Pacboy
                foreach (var pos in potentialPositions) {
                    float distance = Vector3.Distance(pos, Pacboy.transform.position);
                    if (distance < minDistance) {
                        closestPosition = pos;
                        minDistance = distance;
                    }
                }

                // Return this position
                return closestPosition;
            }

            return transform.position;
        }
    }
}