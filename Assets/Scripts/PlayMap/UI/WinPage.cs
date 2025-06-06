using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayMap.UI {
    /**
     * Manages the game over page when the player wins.
     */
    public class WinPage : MonoBehaviour {
        public Button homePageButton; // Return to home page button
        public TMP_Text winText; // Winning congratulation text
        public GameObject newHighScorePrompt; // New high score prompt

        // START FUNCTION
        private void Start() {
            // Set the action listener for the button
            homePageButton.onClick.AddListener(OnHomePageButtonClick);
            
            // Stop the background music
            SoundManager.Instance.StopBackgroundMusic();
            
            // Play win sound
            SoundManager.Instance.PlaySoundOnce(SoundType.PlayerWin);
        }

        /* Action when the home page button is clicked */
        private void OnHomePageButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Back to play map view
            SceneManager.LoadScene("HomePage");

            // Close this page
            gameObject.SetActive(false);
        }

        /**
         * Update the congratulation text.
         * Displays time, final score and new high score prompt (if applicable).
         *
         * PARAM
         * - true: This is a new high score
         * - false: otherwise
         */
        public void UpdateText(bool newHighScore) {
            // Get the game time, final score and history high score
            float gameTime = PlayMapController.Instance.GetTime();
            int gameScore = PlayMapController.Instance.GetScore();

            // Format it into MM:SS
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            string finalTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Text update
            string winPrompt =
                $"Pacboy has successfully eaten all the dots in {finalTime}.\n\n" +
                $"The maze is clean, your appetite is legendary, and the Ghostrons are off filing emotional damage claims." +
                $"\n\nYour game score: {gameScore}\n";
            
            // Display/Hide new high score prompt
            newHighScorePrompt.SetActive(newHighScore);

            winText.text = winPrompt;
        }
    }
}