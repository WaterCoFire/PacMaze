using Entity.Pacboy;
using PlayMap;
using PlayMap.UI;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * NICE BOMB
     * Does nothing when the pacboy picks it
     * Only effective on the player's second operation when a bomb is picked:
     * Press E (default) to directly kill the nearest ghostron,
     * OR press F (default) to deploy the bomb down on the current tile.
     * If using it by deployment, BOTH the ghostron that hits that deployed bomb
     * AND another ghostron that is the nearest to the bomb will be killed.
     * (So SPACE deployment can kill up to two ghostrons while E kill can only kill one)
     */
    public class NiceBombProp : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            Debug.Log("NICE BOMB picked");
            
            // Give the pacboy one more bomb
            pacboy.GetComponent<PacboyPropOperation>().GetNiceBomb();
            
            // Add 10 score points
            PlayMapController.Instance.AddScore(10);
            
            // Prompt the player
            GamePlayUI.Instance.NewInfo("You get a Nice Bomb!", Color.cyan);
        }
    }
}