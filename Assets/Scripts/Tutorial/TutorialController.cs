using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
    public class TutorialController : MonoBehaviour {
        // All the dots
        public List<GameObject> allDots;
        
        // Checkpoints (Display corresponding tips when Pacboy is close to one)
        public List<GameObject> checkpoints; // All the checkpoints
        private const float TipTriggerDistance = 3f; // The trigger distance of displaying a tip
        
        private bool _tutorialInProgress; // Whether the tutorial is currently in progress or not

        // Pacboy game object
        public GameObject pacboy;
        
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
        }
        
        // UPDATE FUNCTION
        private void Update() {
            // No action if game is currently paused
            if (!_tutorialInProgress) return;

            // Listening for ESC pause
            if (Input.GetKeyDown(KeyCode.Escape)) {
                PauseTutorial();
            }
        }
        
        /**
         * Pauses the tutorial.
         */
        private void PauseTutorial() {
            // Disable Pacboy control
            // _pacboy.GetComponent<PacboyMovement>().DisableMovement();
            // _pacboy.GetComponent<PacboyCamera>().DisableCameraOperation();
            // _pacboy.GetComponent<PacboyPropOperation>().DisablePropOperation();

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
            // _pacboy.GetComponent<PacboyMovement>().EnableMovement();
            // _pacboy.GetComponent<PacboyCamera>().EnableCameraOperation();
            // _pacboy.GetComponent<PacboyPropOperation>().EnablePropOperation();

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
            // _pacboy.GetComponent<PacboyMovement>().DisableMovement();
            // _pacboy.GetComponent<PacboyCamera>().DisableCameraOperation();
            // _pacboy.GetComponent<PacboyPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the time scale

            // Display finish page
            TutorialUI.Instance.DisplayFinishPage();
        }
    }
}