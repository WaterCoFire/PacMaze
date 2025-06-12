using Entity.Pacboy;
using PlayMap;
using PlayMap.UI;
using Sound;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * Fast Wheel
     * Pacboy becomes faster for 5 secs.
     */
    public class FastWheelProp : Prop {
        // Fast Wheel speed
        private readonly float _fastSpeed = 7f;

        // Override
        public override void OnPicked(GameObject pacboy) {
            // Pacboy now has faster speed
            if (pacboy.GetComponent<PacboyMovement>().SetSpeedBuff(_fastSpeed)) {
                // Speed buff is valid (currently no Crazy Party event)
                // Play pick up sound (good prop)
                SoundManager.Instance.PlaySoundOnce(SoundType.PickUpGoodProp);
                
                // Add 10 score points
                PlayMapController.Instance.AddScore(10);

                // Prompt the player
                GamePlayUI.Instance.NewInfo("Fast Wheel is making you fly!", Color.cyan);
            }
        }
    }
}