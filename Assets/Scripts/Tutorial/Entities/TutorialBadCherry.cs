using Entity.Prop;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Bad Cherry in tutorial
     * Still implementing Prop abstract class, like the normal one
     */
    public class TutorialBadCherry : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Spawn the Tenacious Ghostron for demonstrating this Bad Cherry
            TutorialController.Instance.SpawnDemoTenaciousGhostron();
        }
    }
}