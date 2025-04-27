using Entity.Pacboy;
using PlayMap;
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
            pacboy.GetComponent<PacboyMovement>().SetSpeedBuff(_slowSpeed);
            
            // Reduce 10 score points
            PlayMapController.Instance.DeductScore(10);
        }
    }
}