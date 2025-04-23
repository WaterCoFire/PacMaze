using Entity.Pacman;
using PlayMap;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * FAST WHEEL
     * Pacman becomes faster for 5 secs.
     */
    public class FastWheelProp : Prop {
        // Fast wheel speed
        private readonly float _fastSpeed = 7f;

        // Override
        public override void OnPicked(GameObject pacman) {
            Debug.Log("FAST WHEEL picked");
            // Pacman now has faster speed
            pacman.GetComponent<PacmanMovement>().SetSpeedBuff(_fastSpeed);
            
            // Add 10 score points
            PlayMapController.Instance.AddScore(10);
        }
    }
}