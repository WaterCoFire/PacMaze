using Setting;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HomePage {
    /**
     * The home page.
     * Buttons: Play, Edit Maps, Setting, Quit Game
     */
    public class HomePage : MonoBehaviour {
        // This home page
        public GameObject homePage;

        public GameObject playMapsPage; // Play maps view page
        public GameObject editMapsPage; // Edit maps view page
        public GameObject pacpediaPage; // Pacpedia page
        public GameObject settingPage; // Key binding setting page

        // Window to be shown if the player has never played PacMaze before
        public GameObject tutorialPromptWindow;

        /* Buttons */
        // In home page
        public Button playButton;
        public Button editMapsButton;
        public Button pacpediaButton;
        public Button settingButton;
        public Button quitGameButton;

        // In tutorial prompt window
        public Button enterTutorialButton; // Enter tutorial
        public Button skipButton; // No thanks, no tutorial

        // AWAKE FUNCTION
        private void Awake() {
            /* For test purpose only - PlayedBefore manual setting */
            PlayerPrefs.SetInt("PlayedBefore", 0);
        }

        // START FUNCTION
        private void Start() {
            Debug.Log("HomePage START");
            SetButtonActionListener(); // Set button action listeners
            
            // Play background music
            SoundManager.Instance.PlayBackgroundMusic(false);

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
                    break;
                case 1:
                    OnPlayButtonClick();
                    break;
                case 2:
                    OnEditMapsButtonClick();
                    break;
                default:
                    Debug.LogError("Invalid UI location: " + uiLocation);
                    break;
            }
        }

        /**
         * Set the action listeners for all the buttons.
         */
        private void SetButtonActionListener() {
            playButton.onClick.AddListener(OnPlayButtonClick);
            editMapsButton.onClick.AddListener(OnEditMapsButtonClick);
            pacpediaButton.onClick.AddListener(OnPacpediaButtonClick);
            settingButton.onClick.AddListener(OnSettingButtonClick);
            quitGameButton.onClick.AddListener(OnQuitButtonClick);
            enterTutorialButton.onClick.AddListener(OnEnterTutorialButtonClick);
            skipButton.onClick.AddListener(OnSkipButtonClick);
        }

        /* Button action listeners */
        // Play button
        private void OnPlayButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Close this home page
            homePage.SetActive(false);

            // Check if the player has played PacMaze before
            bool playedBefore = PlayerPrefs.GetInt("PlayedBefore", 0) == 1;

            if (playedBefore) {
                // Played Before
                // Set the UI location information
                PlayerPrefs.SetInt("MainPageAt", 1);

                // Open play maps page
                playMapsPage.SetActive(true);
            } else {
                // First Time Playing PacMaze
                // Display tutorial prompt window
                tutorialPromptWindow.SetActive(true);
            }
        }

        // Edit maps button
        private void OnEditMapsButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Set the UI location information
            PlayerPrefs.SetInt("MainPageAt", 2);

            // Close this home page
            homePage.SetActive(false);

            // Open edit maps page
            editMapsPage.SetActive(true);
        }

        // Pacpedia button
        private void OnPacpediaButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Close this home page
            homePage.SetActive(false);

            // Open Pacpedia page
            pacpediaPage.SetActive(true);
        }

        // Setting button
        private void OnSettingButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Close this home page
            homePage.SetActive(false);

            // Open setting page
            settingPage.SetActive(true);
            settingPage.GetComponent<SettingPage>().InitUI();
        }

        // Quit game button
        private void OnQuitButtonClick() { }

        // Tutorial prompt window - Enter tutorial button
        private void OnEnterTutorialButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Update played status
            PlayerPrefs.SetInt("PlayedBefore", 1);
            
            // Close the tutorial prompt window
            tutorialPromptWindow.SetActive(false);

            // Enter tutorial scene
            SceneManager.LoadScene("TutorialMap");
        }

        // Tutorial prompt window - Skip button
        private void OnSkipButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Update played status
            PlayerPrefs.SetInt("PlayedBefore", 1);
            
            // Close the tutorial prompt window
            tutorialPromptWindow.SetActive(false);

            // Open play maps page
            playMapsPage.SetActive(true);
        }
    }
}