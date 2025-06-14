﻿using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayMap.UI {
    /**
     * Manages the game over page when the player loses.
     */
    public class LosePage : MonoBehaviour {
        public Button homePageButton; // Return to home page button
        
        // START FUNCTION
        private void Start() {
            // Set the action listener for the button
            homePageButton.onClick.AddListener(OnHomePageButtonClick);
            
            // Stop the background music
            SoundManager.Instance.StopBackgroundMusic();
            
            // Play lose sound
            SoundManager.Instance.PlaySoundOnce(SoundType.PlayerLose);
        }

        /* Action when the home page button is clicked */
        private void OnHomePageButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Back to Play Maps page
            SceneManager.LoadScene("HomePage");
            
            // Close this page
            gameObject.SetActive(false);
        }
    }
}