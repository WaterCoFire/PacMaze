using System;
using MainPage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PlayMap {
    /**
     * Manages the game pause page UI.
     */
    public class PausePage : MonoBehaviour {
        // This pause page and the setting page
        public GameObject pausePage;
        public GameObject settingPage;
        
        // Buttons
        public Button backToGameButton;
        public Button settingButton;
        public Button quitButton;
        
        // START FUNCTION
        private void Start() {
            Debug.Log("PausePage START");
            
            // Set button action listeners
            AddButtonActionListener();
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
            // Resume the game
            PlayMapController.Instance.ResumeGame();
        }
        
        // Setting button
        private void OnSettingButtonClick() {
            // Close this page
            pausePage.SetActive(false);
            
            // Open setting page
            settingPage.SetActive(true);
            settingPage.GetComponent<SettingPage>().InitUI();
        }
        
        // Quit button
        private void OnQuitButtonClick() {
            // TODO Quit the game
        }
    }
}