using UnityEngine;
using UnityEngine.AI;

namespace Entity.Ghostron.GhostronImpl {
    public class GreenGhostron : Ghostron {
        // Wander interval of the green ghostron
        protected override float WanderInterval {
            get { return 6.0f; }
        }
        
        // Scared duration of the green ghostron
        protected override float ScaredDuration {
            get { return 6.0f; }
        }
        
        // The four quadrant middle points, as potential positions
        private readonly Vector3[] _potentialPositions = {
            new(-9, 0, -9), new(-9, 0, 9), new(9, 0, -9), new(9, 0, 9)
        };

        private int _positionIndex = -1; // Index of the current target position, 0-3

        /**
         * OVERRIDE
         * Generates a position, used for getting a target when wandering.
         * Green Ghostron:
         * Go to the middle point of the four quadrants of the map that is the nearest to the Pacboy.
         * If the green ghostron already arrives there, go to another random quadrant middle point.
         */
        protected override Vector3 GenerateWanderingTarget() {
            if (Pacboy != null) {
                // Find the corner that is the furthest away from the Pacboy
                Vector3 nearestPosition = _potentialPositions[0];
                int index = 0;
                float minDistance = Vector3.Distance(nearestPosition, Pacboy.transform.position);

                for (int i = 0; i < 4; i++) {
                    float distance = Vector3.Distance(_potentialPositions[i], Pacboy.transform.position);
                    if (distance < minDistance) {
                        nearestPosition = _potentialPositions[i];
                        index = i;
                        minDistance = distance;
                    }
                }

                // Duplicate target avoiding logic
                if (index != _positionIndex) {
                    // If the new position is different, return this position
                    _positionIndex = index;
                    return nearestPosition;
                } else {
                    // Get another random position at the corner
                    int randIndex;
                    while (true) {
                        var rand = Random.Range(0, 4);
                        if (rand != _positionIndex) {
                            randIndex = rand;
                            break;
                        }
                    }

                    _positionIndex = randIndex;
                    nearestPosition = _potentialPositions[randIndex];
                    return nearestPosition;
                }
            }

            return transform.position;
        }
    }
}