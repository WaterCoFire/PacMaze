using System.Collections.Generic;
using System.Threading;
using Sound;
using Tutorial.Entities;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial {
    public class TutorialController : MonoBehaviour {
        // Checkpoints (Display corresponding tips when Pacboy is close to one)
        public List<GameObject> checkpoints; // All the checkpoints
        private bool[] _checkpointStatus; // The status of all checkpoints (triggered/not triggered)
        private const float TipTriggerDistance = 1.5f; // The trigger distance of displaying a tip

        private bool _tutorialInProgress; // Whether the tutorial is currently in progress or not

        // Pacboy game object
        public GameObject pacboy;

        // The Ghostron for demonstrating Power Pellet
        public GameObject powerPelletGhostron;

        // The Ghostron for demonstrating (using) Nice Bomb
        public GameObject niceBombUseGhostron;

        // The two Ghostrons for demonstrating Deployed Nice Bomb
        public GameObject niceBombDeployGhostronLeft;
        public GameObject niceBombDeployGhostronRight;

        // The Tenacious Ghostron for demonstrating Bad Cherry
        public GameObject tenaciousGhostron;

        private bool _firstNiceBombUsed; // Whether the first Nice Bomb has been used

        // Singleton instance
        public static TutorialController Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("TutorialController AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            Debug.Log("TutorialController START");

            _tutorialInProgress = true;
            _firstNiceBombUsed = false;
            
            // Play background music
            SoundManager.Instance.PlayBackgroundMusic(true);

            // Initialise the checkpoint status array
            _checkpointStatus = new bool[checkpoints.Count];
            
            // Hide the Tenacious Ghostron
            tenaciousGhostron.SetActive(false);

            // Display the first tip (start tip)
            TutorialUI.Instance.DisplayTip(-1);
        }

        // UPDATE FUNCTION
        private void Update() {
            // No action if game is currently paused
            if (!_tutorialInProgress) return;

            // Listening for ESC pause
            if (Input.GetKeyDown(KeyCode.Escape)) {
                PauseTutorial();
            }

            // Compute the distance between Pacboy and all checkpoints (for displaying tips)
            for (int i = 0; i < checkpoints.Count; i++) {
                // Ignore if the iterated checkpoint is already triggered
                if (_checkpointStatus[i]) continue;

                float distance = Vector3.Distance(checkpoints[i].transform.position, pacboy.transform.position);
                if (distance < TipTriggerDistance) {
                    // The corresponding tip should be displayed
                    TutorialUI.Instance.DisplayTip(i);
                    _checkpointStatus[i] = true;
                    break;
                }
            }
        }

        /**
         * Scares the Ghostron for demonstrating Power Pellet (Red).
         * Called by TutorialPowerPellet when Power Pellet is picked up.
         */
        public void ScareDemoGhostron() {
            powerPelletGhostron.GetComponent<TutorialGhostron>().SetScared();
        }

        /**
         * Action when a Nice Bomb is clicked.
         * If this is the first time (for demonstrating using a Nice Bomb):
         * - Make the Green Ghostron chase Pacboy.
         *
         * If this is the second time (for demonstrating deploying a Nice Bomb):
         * - Make the Pink and Blue Ghostrons chase Pacboy.
         */
        public void NiceBombPicked() {
            if (!_firstNiceBombUsed) {
                // First time (using tutorial)
                niceBombUseGhostron.GetComponent<TutorialGhostron>().EnableChase();
            } else {
                // Second time (deploying tutorial)
                niceBombDeployGhostronLeft.GetComponent<TutorialGhostron>().EnableChase();
                niceBombDeployGhostronRight.GetComponent<TutorialGhostron>().EnableChase();
            }
        }

        /**
         * Kills the Ghostron for demonstrating Nice Bomb (Green).
         * Called by TutorialPacboyPropOperation when Pacboy uses the Nice Bomb.
         * Return true if successful
         * Return false if player is doing this operation at the wrong time
         */
        public void KillDemoGhostron() {
            _firstNiceBombUsed = true;
            Destroy(niceBombUseGhostron);
            
            // Close action prompt as task (kill a Ghostron using Nice Bomb) is completed
            TutorialUI.Instance.CloseActionPrompt();
        }

        /**
         * Kills the two Ghostrons for demonstrating Deployed Nice Bomb (Pink and Blue).
         * Called by TutorialDeployedNiceBomb when one of the two Ghostrons steps on the Deployed Nice Bomb.
         * Return true if successful
         * Return false if player is doing this operation at the wrong time
         */
        public void KillDemoTwoGhostrons() {
            Destroy(niceBombDeployGhostronLeft);
            Destroy(niceBombDeployGhostronRight);
            
            // Close action prompt as task (kill two Ghostrons using Deployed Nice Bomb) is completed
            TutorialUI.Instance.CloseActionPrompt();
        }

        /**
         * "Spawns" the Tenacious Ghostron for demonstrating Bad Cherry.
         * Called by TutorialBadCherry when Pacboy eats the Bad Cherry.
         */
        public void SpawnDemoTenaciousGhostron() {
            tenaciousGhostron.SetActive(true);
            // Let it chase Pacboy
            tenaciousGhostron.GetComponent<TutorialGhostron>().EnableChase();
        }

        /**
         * Pauses the tutorial.
         */
        private void PauseTutorial() {
            // Disable Pacboy control
            pacboy.GetComponent<TutorialPacboyMovement>().DisableMovement();
            pacboy.GetComponent<TutorialPacboyCamera>().DisableCameraOperation();
            pacboy.GetComponent<TutorialPacboyPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the timescale
            _tutorialInProgress = false;

            // Display the pause page
            TutorialUI.Instance.SetPausePage(true);
        }

        /**
         * Resumes the tutorial.
         */
        public void ResumeTutorial() {
            // Enable Pacboy control
            pacboy.GetComponent<TutorialPacboyMovement>().EnableMovement();
            pacboy.GetComponent<TutorialPacboyCamera>().EnableCameraOperation();
            pacboy.GetComponent<TutorialPacboyPropOperation>().EnablePropOperation();

            Time.timeScale = 1f; // Resume the timescale
            _tutorialInProgress = true;

            // Hide the pause page
            TutorialUI.Instance.SetPausePage(false);
        }

        /**
         * Called when the tutorial finishes.
         */
        public void Finish() {
            _tutorialInProgress = false;

            // Disable Pacboy control
            pacboy.GetComponent<TutorialPacboyMovement>().DisableMovement();
            pacboy.GetComponent<TutorialPacboyCamera>().DisableCameraOperation();
            pacboy.GetComponent<TutorialPacboyPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the timescale

            // Display finish page
            TutorialUI.Instance.DisplayFinishPage();
        }
    }
}