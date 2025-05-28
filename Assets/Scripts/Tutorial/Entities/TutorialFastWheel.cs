using Entity.Prop;
using Sound;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Fast Wheel in tutorial
     * Still implementing Prop abstract class, like the normal one
     */
    public class TutorialFastWheel : Prop {
        // Fast Wheel speed
        private readonly float _fastSpeed = 7f;
        
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Play pick up sound (good prop)
            SoundManager.Instance.PlaySoundOnce(SoundType.PickUpGoodProp);
            
            // Set speed buff
            pacboy.GetComponent<TutorialPacboyMovement>().SetSpeedBuff(_fastSpeed);
            
            // Close action prompt as task (pick up Fast Wheel) is completed
            TutorialUI.Instance.CloseActionPrompt();
        }
    }
}