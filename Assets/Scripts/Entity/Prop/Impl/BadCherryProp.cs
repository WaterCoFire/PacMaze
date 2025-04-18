using UnityEngine;

namespace Entity.Prop.Impl {
    /**
     * BAD CHERRY
     * Spawns one more ghostron.
     * (spawns in the center)
     */
    public class BadCherryProp : Prop {
        // Override
        protected override void OnPicked(GameObject pacman) {
            Debug.Log("BAD CHERRY picked");
            // TODO
        }
    }
}