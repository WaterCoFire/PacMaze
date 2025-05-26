using Entity.Prop;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Power Pellet in tutorial
     * Still implementing Prop abstract class, like the normal one
     */
    public class TutorialPowerPellet : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Scare the Ghostron in front of the Power Pellet
            TutorialController.Instance.ScareDemoGhostron();
        }
    }
}