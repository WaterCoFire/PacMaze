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
        private KeyCode _useNiceBombKeyCode = KeyCode.E; // Use
        private KeyCode _deployNiceBombKeyCode = KeyCode.F; // Deploy

        // Disabled when game is not in normal process (e.g. paused)
        private bool _controllable;

        // START FUNCTION
        private void Start() {
            Debug.Log("PacboyPropOperation START");

            // Initialisation
            _niceBombs = 0;
            _controllable = true;
        }

        // UPDATE FUNCTION
        private void Update() {
            // No operation if not controllable
            if (!_controllable) return;

            // No operation if no nice bombs currently obtained
            if (_niceBombs == 0) return;

            // Use nice bomb
            if (Input.GetKeyDown(_useNiceBombKeyCode)) {
                UseNiceBomb();
                return;
            }

            // Deploy nice bomb
            if (Input.GetKeyDown(_deployNiceBombKeyCode)) {
                DeployNiceBomb();
            }
        }

        /**
         * Pacboy uses a nice bomb (default key code E).
         * Directly kills the Ghostron nearest to the Pacboy.
         */
        private void UseNiceBomb() {
            Debug.Log("NiceBomb USE");
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
         * Pacboy deploys a nice bomb (default key code F).
         * Places the deployed bomb at the current position.
         * Two ghostrons nearest to it will be killed when a ghostron hits the deployed bomb.
         */
        private void DeployNiceBomb() {
            Debug.Log("NiceBomb DEPLOY");
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