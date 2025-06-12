using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tutorial {
    /**
     * For tutorial only
     * Manages the finish page in tutorial.
     */
    public class TutorialFinishPage : MonoBehaviour {
        public Button homePageButton; // Return to home page button

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
            
            // Back to the home page
            SceneManager.LoadScene("HomePage");

            // Close this page
            gameObject.SetActive(false);
        }
    }
}