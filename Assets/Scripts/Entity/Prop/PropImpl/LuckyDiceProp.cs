using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * LUCKY DICE
     * When picked, gives the effect of one of the five other props.
     * Probabilities vary according to the game difficulty.
     */
    public class LuckyDiceProp : Prop {
        // Override
        protected override void OnPicked(GameObject pacman) {
            Debug.Log("LUCKY DICE picked");
            // TODO
        }
    }
}