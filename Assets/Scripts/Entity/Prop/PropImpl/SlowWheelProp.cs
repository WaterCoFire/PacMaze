using Entity.Pacman;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * SLOW WHEEL
     * Pacman becomes slower for 5 secs.
     */
    public class SlowWheelProp : Prop {
        // Slow wheel speed
        private readonly float _slowSpeed = 4f;

        // Override
        protected override void OnPicked(GameObject pacman) {
            Debug.Log("SLOW WHEEL picked");
            // Pacman now has slower speed
            pacman.GetComponent<PacmanMovement>().SetSpeedBuff(_slowSpeed);
        }
    }
}