using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage {
    /**
     * The home page.
     * Buttons: Play, Edit Maps, Setting, Quit Game
     */
    public class HomePage : MonoBehaviour {
        // This home page
        public GameObject homePage;

        public GameObject playMapsPage; // Play maps view page
        public GameObject editMapsPage; // Edit maps view page
        
        // public GameObject settingPage; // Setting page

        /* Buttons */
        public Button playButton;
        public Button editMapsButton;
        public Button settingButton;
        public Button quitGameButton;

        // START FUNCTION
        private void Start() {
            Debug.Log("HomePage START");
            SetButtonActionListener(); // Set button action listeners
        }

        /**
         * Set the action listeners for all the buttons
         */
        private void SetButtonActionListener() {
            playButton.onClick.AddListener(OnPlayButtonClick);
            editMapsButton.onClick.AddListener(OnEditMapsButtonClick);
            settingButton.onClick.AddListener(OnSettingButtonClick);
            quitGameButton.onClick.AddListener(OnQuitButtonClick);
        }

        /* Button action listeners */
        // Play button
        private void OnPlayButtonClick() {
            // Close this home page
            homePage.SetActive(false);

            // Open play maps page
            playMapsPage.SetActive(true);
        }

        // Edit maps button
        private void OnEditMapsButtonClick() {
            // Close this home page
            homePage.SetActive(false);

            // Open play maps page
            editMapsPage.SetActive(true);
        }

        // Setting button
        private void OnSettingButtonClick() {
            // Close this home page
            homePage.SetActive(false);

            // Open setting page
            // settingPage.SetActive(true);
        }

        // Quit game button
        private void OnQuitButtonClick() { }
    }
}