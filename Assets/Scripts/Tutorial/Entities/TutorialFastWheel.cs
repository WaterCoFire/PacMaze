using Entity.Prop;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Fast Wheel in tutorial
     */
    public class TutorialFastWheel : Prop {
        // Fast Wheel speed
        private readonly float _fastSpeed = 7f;
        
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Set speed buff
            pacboy.GetComponent<TutorialPacboyMovement>().SetSpeedBuff(_fastSpeed);
            
            // Close action prompt as task (pick up Fast Wheel) is completed
            TutorialUI.Instance.CloseActionPrompt();
        }
    }
}