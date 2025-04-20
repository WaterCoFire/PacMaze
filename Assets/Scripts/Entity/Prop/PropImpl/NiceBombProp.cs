using Entity.Pacman;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * NICE BOMB
     * Does nothing when the pacman picks it
     * Only effective on the player's second operation when a bomb is picked:
     * Press E (default) to directly kill the nearest ghostron,
     * OR press F (default) to deploy the bomb down on the current tile.
     * If using it by deployment, BOTH the ghostron that hits that deployed bomb
     * AND another ghostron that is the nearest to the bomb will be killed.
     * (So SPACE deployment can kill up to two ghostrons while E kill can only kill one)
     */
    public class NiceBombProp : Prop {
        // Override
        protected override void OnPicked(GameObject pacman) {
            Debug.Log("NICE BOMB picked");
            
            // Give the pacman one more bomb
            pacman.GetComponent<PacmanPropOperation>().GetNiceBomb();
        }
    }
}