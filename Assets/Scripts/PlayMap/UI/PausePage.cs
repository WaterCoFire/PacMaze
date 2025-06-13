using Setting;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayMap.UI {
    /**
     * Manages the game pause page UI.
     */
    public class PausePage : MonoBehaviour {
        // This pause page and the setting page
        public GameObject pausePage;
        public GameObject settingPage;
        
        // Buttons
        public Button backToGameButton; // Or pressing ESC
        public Button settingButton;
        public Button quitButton;
        
        // START FUNCTION
        private void Start() {
            // Set button action listeners
            AddButtonActionListener();
        }
        
        // UPDATE FUNCTION
        private void Update() {
            // Listening for ESC (alternative of pressing the Back to Game button)
            if (Input.GetKeyDown(KeyCode.Escape)) {
                OnBackToGameButtonClick();
            }
        }

        /**
         * Sets the action listeners for all the buttons.
         */
        private void AddButtonActionListener() {
            backToGameButton.onClick.AddListener(OnBackToGameButtonClick);
            settingButton.onClick.AddListener(OnSettingButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }
        
        /* Button action listeners */
        // Back to game button
        private void OnBackToGameButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Resume the game
            PlayMapController.Instance.ResumeGame();
        }
        
        // Setting button
        private void OnSettingButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Close this page
            pausePage.SetActive(false);
            
            // Open setting page
            settingPage.SetActive(true);
            settingPage.GetComponent<SettingPage>().InitUI();
        }
        
        // Quit button
        private void OnQuitButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Quit the game, back to Play Map page
            SceneManager.LoadScene("HomePage");
        }
    }
}