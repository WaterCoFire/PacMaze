using PlayMap;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * POWER PELLET
     * Scares all ghostrons.
     * (Slows them down, and they are temporarily "frozen" when pacman catches them)
     */
    public class PowerPelletProp : Prop {
        // Override
        public override void OnPicked(GameObject pacman) {
            Debug.Log("POWER PELLET picked");
            // Scare all the ghosts
            GhostronManager.Instance.ScareAllGhostrons();
            
            // Give the pacman 50 score points
            PlayMapController.Instance.AddScore(50);
        }
    }
}