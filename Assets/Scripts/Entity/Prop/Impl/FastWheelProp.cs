using Entity.Pacman;
using UnityEngine;

namespace Entity.Prop.Impl {
    /**
     * FAST WHEEL
     * Pacman becomes faster for 5 secs.
     */
    public class FastWheelProp : Prop {
        // Fast wheel speed
        private readonly float _fastSpeed = 7f;

        // Override
        protected override void OnPicked(GameObject pacman) {
            Debug.Log("FAST WHEEL picked");
            // Pacman now has faster speed
            pacman.GetComponent<PacmanMovement>().SetSpeedBuff(_fastSpeed);
        }
    }
}