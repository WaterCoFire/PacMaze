using Entity.Prop;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Power Pellet in tutorial
     */
    public class TutorialPowerPellet : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Scare the Ghostron in front of the Power Pellet
            TutorialController.Instance.ScareDemoGhostron();
        }
    }
}