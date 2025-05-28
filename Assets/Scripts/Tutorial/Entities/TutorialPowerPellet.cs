using Entity.Prop;
using Sound;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Power Pellet in tutorial
     * Still implementing Prop abstract class, like the normal one
     */
    public class TutorialPowerPellet : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Play pick up sound (good prop)
            SoundManager.Instance.PlaySoundOnce(SoundType.PickUpGoodProp);
            
            // Scare the Ghostron in front of the Power Pellet
            TutorialController.Instance.ScareDemoGhostron();
        }
    }
}