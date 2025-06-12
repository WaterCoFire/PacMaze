using Entity.Pacboy;
using PlayMap;
using PlayMap.UI;
using Sound;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * Nice Bomb
     * Does nothing when Pacboy picks it
     * Only effective on the player's second operation when a bomb is picked:
     * Press E (default) to directly kill the nearest Ghostron,
     * OR press F (default) to deploy the bomb down on the current tile.
     * If using it by deployment, BOTH the Ghostron that hits that deployed bomb
     * AND another Ghostron that is the nearest to the bomb will be killed.
     * (So deployment can kill up to two Ghostrons while E kill can only kill one)
     */
    public class NiceBombProp : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Play pick up sound (good prop)
            SoundManager.Instance.PlaySoundOnce(SoundType.PickUpGoodProp);
            
            // Give the pacboy one more bomb
            pacboy.GetComponent<PacboyPropOperation>().GetNiceBomb();
            
            // Add 10 score points
            PlayMapController.Instance.AddScore(10);
            
            // Prompt the player
            GamePlayUI.Instance.NewInfo("You get a Nice Bomb!", Color.cyan);
        }
    }
}