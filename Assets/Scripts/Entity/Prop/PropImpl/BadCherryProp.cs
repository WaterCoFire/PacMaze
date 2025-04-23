using PlayMap;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Prop.PropImpl {
    /**
     * BAD CHERRY
     * Spawns one more ghostron.
     * (spawns randomly)
     */
    public class BadCherryProp : Prop {
        // Override
        public override void OnPicked(GameObject pacman) {
            Debug.Log("BAD CHERRY picked");
            // Spawn a new ghostron at random place
            GhostronManager.Instance.NewRandomGhostron();
            
            // Reduce 50 score points
            PlayMapController.Instance.DeductScore(50);
        }
    }
}