using Entity.Prop;
using Sound;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * For tutorial only
     * Bad Cherry in tutorial
     * Still implementing Prop abstract class, like the normal one
     */
    public class TutorialBadCherry : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Play pick up sound (bad prop)
            SoundManager.Instance.PlaySoundOnce(SoundType.PickUpBadProp);
            
            // Spawn the Tenacious Ghostron for demonstrating this Bad Cherry
            TutorialController.Instance.SpawnDemoTenaciousGhostron();
        }
    }
}