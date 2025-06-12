using PlayMap;
using PlayMap.UI;
using Sound;
using UnityEngine;

namespace Entity.Prop.PropImpl {
    /**
     * Power Pellet
     * Scares all Ghostrons.
     * (Slows them down, and they are temporarily stalled when Pacboy catches them)
     */
    public class PowerPelletProp : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Play pick up sound (good prop)
            SoundManager.Instance.PlaySoundOnce(SoundType.PickUpGoodProp);
            
            // Scare all the ghosts
            GhostronManager.Instance.ScareAllGhostrons();
            
            // Give the pacboy 50 score points
            PlayMapController.Instance.AddScore(50);
            
            // Prompt the player
            GamePlayUI.Instance.NewInfo("Ghostrons are scared! Go get them!", Color.cyan);
        }
    }
}