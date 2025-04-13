using System;
using Unity.VisualScripting;
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

            // Obtain the UI page that the player last left from
            // If it is the first time the player starts the game, it will be at default 0 (main page)
            // All indexes:
            // 0 - Home Page
            // 1 - Play Page
            // 2 - Edit Maps Page
            // (More indexes to be reserved)
            int uiLocation = PlayerPrefs.GetInt("MainPageAt", 0);

            switch (uiLocation) {
                case 0:
                    Debug.Log("Stays in home page");
                    break;
                case 1:
                    Debug.Log("Going to play map page");
                    OnPlayButtonClick();
                    break;
                case 2:
                    Debug.Log("Going to edit map page");
                    OnEditMapsButtonClick();
                    break;
                default:
                    Debug.LogError("Invalid UI location: " + uiLocation);
                    break;
            }
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
            // Set the UI location information
            PlayerPrefs.SetInt("MainPageAt", 1);
            
            // Close this home page
            homePage.SetActive(false);

            // Open play maps page
            playMapsPage.SetActive(true);
        }

        // Edit maps button
        private void OnEditMapsButtonClick() {
            // Set the UI location information
            PlayerPrefs.SetInt("MainPageAt", 2);
            
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