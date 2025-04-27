using PlayMap;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * LUCKY DICE
     * When picked, gives the effect of one of the five other props.
     * Probabilities vary according to the game difficulty:
     * - EASY: 50% Fast Wheel, 25% Power Pellet, 25% Nice Bomb
     * - NORMAL: 20% Power Pellet, 20% Fast Wheel, 20% Nice Bomb, 20% Slow Wheel, 20% Bad Cherry
     * - HARD: 20% Fast Wheel, 15% Power Pellet, 15% Nice Bomb, 25% Slow Wheel, 25% Bad Cherry
     */
    public class LuckyDiceProp : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            Debug.Log("LUCKY DICE picked");
            // Get a random prop type
            int propType = RandomPropType();

            // Corresponding effect
            switch (propType) {
                case 0:
                    // Power Pellet
                    new PowerPelletProp().OnPicked(pacboy);
                    return;
                case 1:
                    // Fast Wheel
                    new FastWheelProp().OnPicked(pacboy);
                    return;
                case 2:
                    // Nice Bomb
                    new NiceBombProp().OnPicked(pacboy);
                    return;
                case 3:
                    // Slow Wheel
                    new SlowWheelProp().OnPicked(pacboy);
                    return;
                case 4:
                    // Bad Cherry
                    new BadCherryProp().OnPicked(pacboy);
                    return;
                default:
                    Debug.LogError("Invalid prop type!");
                    return;
            }
        }

        /**
         * Returns a random prop type.
         * 0 - Power Pellet
         * 1 - Fast Wheel
         * 2 - Nice Bomb
         * 3 - Slow Wheel
         * 4 - Bad Cherry
         */
        private int RandomPropType() {
            // Get the difficulty
            char difficulty = PlayMapController.Instance.GetDifficulty();

            // Random number from 1 to 100
            int rand = Random.Range(1, 101);
            switch (difficulty) {
                case 'E':
                    // EASY
                    if (rand <= 25) return 0;
                    if (rand <= 75) return 1;
                    return 2;
                case 'N':
                    // NORMAL
                    if (rand <= 20) return 0;
                    if (rand <= 40) return 1;
                    if (rand <= 60) return 2;
                    if (rand <= 80) return 3;
                    return 4;
                case 'H':
                    // HARD
                    if (rand <= 15) return 0;
                    if (rand <= 35) return 1;
                    if (rand <= 50) return 2;
                    if (rand <= 75) return 3;
                    return 4;
                default:
                    Debug.LogError("Invalid difficulty data read when generating random prop type!");
                    return -1;
            }
        }
    }
}