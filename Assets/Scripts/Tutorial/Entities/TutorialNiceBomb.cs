using Entity.Prop;
using Sound;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * For tutorial only
     * Nice Bomb in tutorial
     * Still implementing Prop abstract class, like the normal one
     */
    public class TutorialNiceBomb : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Play pick up sound (good prop)
            SoundManager.Instance.PlaySoundOnce(SoundType.PickUpGoodProp);
            
            // Give Pacboy the Nice Bomb
            pacboy.GetComponent<TutorialPacboyPropOperation>().GetNiceBomb();
            
            // Let the corresponding demo Ghostron chase Pacboy
            TutorialController.Instance.NiceBombPicked();
        }
    }
}