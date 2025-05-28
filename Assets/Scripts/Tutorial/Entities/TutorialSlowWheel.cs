using Entity.Prop;
using Sound;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Slow Wheel in tutorial
     * Still implementing Prop abstract class, like the normal one
     */
    public class TutorialSlowWheel : Prop {
        // Slow Wheel speed
        private readonly float _slowSpeed = 3f;
        
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Play pick up sound (bad prop)
            SoundManager.Instance.PlaySoundOnce(SoundType.PickUpBadProp);
            
            // Set speed debuff
            pacboy.GetComponent<TutorialPacboyMovement>().SetSpeedBuff(_slowSpeed);
        }
    }
}