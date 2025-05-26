using Entity.Prop;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial.Entities {
    /**
     * Nice Bomb in tutorial
     */
    public class TutorialNiceBomb : Prop {
        // Override
        public override void OnPicked(GameObject pacboy) {
            // Give Pacboy the Nice Bomb
            pacboy.GetComponent<TutorialPacboyPropOperation>().GetNiceBomb();
            
            // Let the corresponding demo Ghostron chase Pacboy
            TutorialController.Instance.NiceBombPicked();
        }
    }
}