using Sound;
using UnityEngine;

namespace Tutorial.Entities.TutorialPacboy {
    /**
     * Manages the prop operation of the Pacboy in tutorial.
     */
    public class TutorialPacboyPropOperation : MonoBehaviour {
        // The game object prefab of the deployed nice bomb
        // Has a different color for the player to tell its difference from un-picked ones
        public GameObject deployedNiceBombPrefab;

        // The number of nice bombs that the Pacboy has
        private int _niceBombs;

        // Nice bomb operation KeyCodes (not changeable in tutorial)
        private const KeyCode UseNiceBombKeyCode = KeyCode.E; // Use
        private const KeyCode DeployNiceBombKeyCode = KeyCode.F; // Deploy

        // Disabled when game is not in normal process (e.g. paused)
        private bool _controllable;

        // START FUNCTION
        private void Start() {
            // Initialisation
            _niceBombs = 0;
            _controllable = true;
        }

        // UPDATE FUNCTION
        private void Update() {
            // No operation if not controllable
            if (!_controllable) return;

            // No operation if no Nice Bombs currently obtained
            if (_niceBombs == 0) return;

            // Use Nice Bomb (demo)
            if (Input.GetKeyDown(UseNiceBombKeyCode)) {
                UseDemoNiceBomb();
                return;
            }

            // Deploy Nice Bomb (demo)
            if (Input.GetKeyDown(DeployNiceBombKeyCode)) {
                DeployDemoNiceBomb();
            }
        }

        /**
         * Pacboy uses a Nice Bomb (default key code E).
         * Directly kills the Ghostron for demonstrating this.
         */
        private void UseDemoNiceBomb() {
            // Play explode sound
            SoundManager.Instance.PlaySoundOnce(SoundType.NiceBombExplode);
            
            // Reduce the number
            _niceBombs--;
            
            // Kill the nearest Ghostron
            TutorialController.Instance.KillDemoGhostron();
            
            // Make the button disappear if no more bombs
            // Update display num otherwise
            if (_niceBombs == 0) {
                TutorialUI.Instance.SetNiceBombPrompt(false);
            } else {
                TutorialUI.Instance.UpdateNiceBombNum(_niceBombs);
            }
        }

        /**
         * Pacboy deploys a Nice Bomb (default key code F).
         * Places the Deployed Nice Bomb at the current position.
         * Two Ghostrons for demonstrating this will be killed when one of them steps on the deployed bomb.
         */
        private void DeployDemoNiceBomb() {
            // Play deploy sound
            SoundManager.Instance.PlaySoundOnce(SoundType.DeployNiceBomb);
            
            // Reduce the number
            _niceBombs--;

            // Place the deployed bomb at the current location of the Pacboy
            Instantiate(deployedNiceBombPrefab, transform.position, Quaternion.identity);

            // Make the button disappear if no more bombs
            // Update display num otherwise
            if (_niceBombs == 0) {
                TutorialUI.Instance.SetNiceBombPrompt(false);
            } else {
                TutorialUI.Instance.UpdateNiceBombNum(_niceBombs);
            }
        }

        /**
         * Grant a new nice bomb to the Pacboy.
         * Called when the Pacboy eats a new un-picked nice bomb object.
         */
        public void GetNiceBomb() {
            // One more nice bomb
            _niceBombs++;

            // UI update
            TutorialUI.Instance.UpdateNiceBombNum(_niceBombs);
            TutorialUI.Instance.SetNiceBombPrompt(true);
        }

        /**
         * Allows the player to do prop operations.
         */
        public void EnablePropOperation() {
            _controllable = true;
        }

        /**
         * Stops the player from doing prop operations.
         */
        public void DisablePropOperation() {
            _controllable = false;
        }
    }
}