using Entity.Prop;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Slow Wheel in tutorial
     */
    public class TutorialSlowWheel : Prop {
        // Slow Wheel speed
        private readonly float _slowSpeed = 4f;
        
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Set speed debuff
            pacboy.GetComponent<TutorialPacboyMovement>().SetSpeedBuff(_slowSpeed);
        }
    }
}