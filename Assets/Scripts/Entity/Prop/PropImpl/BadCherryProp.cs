using PlayMap;
using PlayMap.UI;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * BAD CHERRY
     * Spawns a new Tenacious Ghostron.
     * (spawns at randomly position)
     */
    public class BadCherryProp : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            Debug.Log("BAD CHERRY picked");
            // Spawn a new Ghostron at random place
            GhostronManager.Instance.SpawnTenaciousGhostron();
            
            // Reduce 50 score points
            PlayMapController.Instance.DeductScore(50);
            
            // Prompt the player
            GamePlayUI.Instance.NewInfo("Oh no! A Tenacious Ghostron is coming!", Color.red);
        }
    }
}