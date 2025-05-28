using Entity.Pacboy;
using PlayMap;
using PlayMap.UI;
using Sound;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * SLOW WHEEL
     * Pacboy becomes slower for 5 secs.
     */
    public class SlowWheelProp : Prop {
        // Slow Wheel speed
        private readonly float _slowSpeed = 3f;

        // Override
        public override void OnPicked(GameObject pacboy) {
            // Pacboy now has slower speed
            if (pacboy.GetComponent<PacboyMovement>().SetSpeedBuff(_slowSpeed)) {
                // Speed debuff is valid (currently no Crazy Party event)
                // Play pick up sound (bad prop)
                SoundManager.Instance.PlaySoundOnce(SoundType.PickUpBadProp);

                // Reduce 10 score points
                PlayMapController.Instance.DeductScore(10);

                // Prompt the player
                GamePlayUI.Instance.NewInfo("Oh no! Slow Wheel is making you like a turtle!", Color.red);
            }
        }
    }
}