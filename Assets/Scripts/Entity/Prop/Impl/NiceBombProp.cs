using UnityEngine;

namespace Entity.Prop.Impl {
    /**
     * NICE BOMB
     * Does nothing when the pacman picks it
     * Only effective on the player's second operation when one bomb is picked:
     * Press E to directly kill the nearest ghostron,
     * OR press Space to deploy the bomb down on the current tile.
     * If using it by deployment, BOTH the ghostron that hits that deployed bomb
     * AND another ghostron that is the nearest to the bomb will be killed.
     * (So Space deployment can kill up to two ghostrons while E kill can only kill one)
     */
    public class NiceBombProp : Prop {
        // Override
        protected override void OnPicked(GameObject pacman) {
            Debug.Log("NICE BOMB picked");
            // TODO
        }
    }
}