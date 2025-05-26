using PlayMap;
using PlayMap.UI;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * POWER PELLET
     * Scares all Ghostrons.
     * (Slows them down, and they are temporarily "frozen" when pacboy catches them)
     */
    public class PowerPelletProp : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            Debug.Log("POWER PELLET picked");
            // Scare all the ghosts
            GhostronManager.Instance.ScareAllGhostrons();
            
            // Give the pacboy 50 score points
            PlayMapController.Instance.AddScore(50);
            
            // Prompt the player
            GamePlayUI.Instance.NewInfo("Ghostrons are scared! Go get them!", Color.cyan);
        }
    }
}