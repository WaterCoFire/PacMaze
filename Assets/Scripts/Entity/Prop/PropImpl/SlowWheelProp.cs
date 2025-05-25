using Entity.Pacboy;
using PlayMap;
using PlayMap.UI;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * SLOW WHEEL
     * Pacboy becomes slower for 5 secs.
     */
    public class SlowWheelProp : Prop {
        // Slow wheel speed
        private readonly float _slowSpeed = 4f;

        // Override
        public override void OnPicked(GameObject pacboy) {
            Debug.Log("SLOW WHEEL picked");
            // Pacboy now has slower speed
            if (pacboy.GetComponent<PacboyMovement>().SetSpeedBuff(_slowSpeed)) {
                // Speed debuff is valid (currently no Crazy Party event)
                // Reduce 10 score points
                PlayMapController.Instance.DeductScore(10);

                // Prompt the player
                GamePlayUI.Instance.NewInfo("Oh no! Slow Wheel is making you like a turtle!", Color.red);
            }
        }
    }
}