using Entity.Pacboy;
using PlayMap;
using PlayMap.UI;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * FAST WHEEL
     * Pacboy becomes faster for 5 secs.
     */
    public class FastWheelProp : Prop {
        // Fast wheel speed
        private readonly float _fastSpeed = 7f;

        // Override
        public override void OnPicked(GameObject pacboy) {
            Debug.Log("FAST WHEEL picked");
            // Pacboy now has faster speed
            pacboy.GetComponent<PacboyMovement>().SetSpeedBuff(_fastSpeed);
            
            // Add 10 score points
            PlayMapController.Instance.AddScore(10);
            
            // Prompt the player
            GamePlayUI.Instance.NewInfo("Fast Wheel is making you fly!", Color.cyan);
        }
    }
}