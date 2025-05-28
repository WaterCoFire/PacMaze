using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tutorial {
    /**
     * Manages the pause page in tutorial.
     */
    public class TutorialPausePage : MonoBehaviour {
        // Buttons
        public Button backToTutorialButton; // Or pressing ESC
        public Button quitButton;
        
        // START FUNCTION
        private void Start() {
            Debug.Log("TutorialPausePage START");
            
            // Set button action listeners
            AddButtonActionListener();
        }
        
        // UPDATE FUNCTION
        private void Update() {
            // Listening for ESC (alternative of pressing the Back to Tutorial button)
            if (Input.GetKeyDown(KeyCode.Escape)) {
                OnBackToTutorialButtonClick();
            }
        }

        /**
         * Sets the action listeners for all the buttons.
         */
        private void AddButtonActionListener() {
            backToTutorialButton.onClick.AddListener(OnBackToTutorialButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }
        
        /* Button action listeners */
        // Back to tutorial button
        private void OnBackToTutorialButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Resume the tutorial
            TutorialController.Instance.ResumeTutorial();
        }
        
        // Quit button
        private void OnQuitButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Quit the tutorial and go back to the Home Page
            SceneManager.LoadScene("HomePage");
        }
    }
}