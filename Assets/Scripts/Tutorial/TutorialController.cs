using System.Collections.Generic;
using Tutorial.Entities;
using Tutorial.Entities.TutorialPacboy;
using UnityEngine;

namespace Tutorial {
    public class TutorialController : MonoBehaviour {
        // Checkpoints (Display corresponding tips when Pacboy is close to one)
        public List<GameObject> checkpoints; // All the checkpoints
        private const float TipTriggerDistance = 3f; // The trigger distance of displaying a tip

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
                float distance = Vector3.Distance(checkpoints[i].transform.position, pacboy.transform.position);
                if (distance < TipTriggerDistance) {
                    // The corresponding tip should be displayed
                    TutorialUI.Instance.DisplayTip(i);
                    break;
                }
            }
        }

        /**
         * Scares the Ghostron for demonstrating Power Pellet (Red) by changing its skin.
         * Called by TutorialPowerPellet when Power Pellet is picked up.
         */
        public void ScareDemoGhostron() {
            powerPelletGhostron.GetComponent<TutorialGhostron>().SetScaredMaterial();
        }

        /**
         * Kills the Ghostron for demonstrating Nice Bomb (Green).
         * Called by TutorialPacboyPropOperation when Pacboy uses the Nice Bomb.
         */
        public void KillDemoGhostron() {
            Destroy(niceBombUseGhostron);
        }

        /**
         * Kills the two Ghostrons for demonstrating Deployed Nice Bomb (Pink and Blue).
         * Called by TutorialDeployedNiceBomb when one of the two Ghostrons steps on the Deployed Nice Bomb.
         */
        public void KillDemoTwoGhostrons() {
            Destroy(niceBombDeployGhostronLeft);
            Destroy(niceBombDeployGhostronRight);
        }

        /**
         * "Spawns" the Tenacious Ghostron for demonstrating Bad Cherry.
         * Called by TutorialBadCherry when Pacboy eats the Bad Cherry.
         */
        public void SpawnDemoTenaciousGhostron() {
            tenaciousGhostron.SetActive(true);
        }

        /**
         * Pauses the tutorial.
         */
        private void PauseTutorial() {
            // Disable Pacboy control
            pacboy.GetComponent<TutorialPacboyMovement>().DisableMovement();
            pacboy.GetComponent<TutorialPacboyCamera>().DisableCameraOperation();
            pacboy.GetComponent<TutorialPacboyPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the time scale
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

            Time.timeScale = 1f; // Resume the time scale
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