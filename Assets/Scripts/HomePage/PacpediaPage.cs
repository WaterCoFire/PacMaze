using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HomePage {
    /**
     * Manages the Pacpedia page.
     */
    public class PacpediaPage : MonoBehaviour {
        // This page and the home page
        public GameObject pacpediaPage;
        public GameObject homePage;

        /* Buttons - for displaying corresponding section */
        public Button pacboyButton;
        public Button ghostronButton;
        public Button propsButton;
        public Button randomEventButton;

        public Button backButton; // Back to home page

        public Button tutorialButton; // Button to enter tutorial

        /* Scroll Rect used to display contents */
        public ScrollRect contentScrollRect;

        /* Section Content prefabs: Pacboy, Ghostron, Props, Random Event */
        public GameObject pacboyContentPrefab;
        public GameObject ghostronContentPrefab;
        public GameObject propsContentPrefab;
        public GameObject randomEventContentPrefab;

        // Button Colours
        // Normal color 8C79FF
        // Selected color E4FF49
        private readonly Color _normalColor = new(140f / 255f, 121f / 255f, 255f / 255f);
        private readonly Color _selectedColor = new(228f / 255f, 255f / 255f, 73f / 255f);

        // START FUNCTION
        private void Start() {
            // Set button action listeners
            SetButtonActionListener();

            // Enter Pacboy section by default
            OnPacboyButtonClick();
        }

        /**
         * Set the color of a button.
         * (Default/Selected)
         */
        private void SetButtonStatus(Button button, bool selected) {
            if (selected) {
                var colors = button.colors;
                colors.normalColor = _selectedColor;
                button.colors = colors;
            } else {
                var colors = button.colors;
                colors.normalColor = _normalColor;
                button.colors = colors;
            }
        }

        /**
         * Empties the Pacpedia content Scroll Rect.
         * Called when initialising/re-selecting a new content.
         */
        private void EmptyScrollRect() {
            // Empty the whole content list first
            foreach (Transform child in contentScrollRect.content.transform) {
                Destroy(child.gameObject);
            }

            contentScrollRect.verticalNormalizedPosition = 0f;
        }

        /* Set button action listeners */
        private void SetButtonActionListener() {
            pacboyButton.onClick.AddListener(OnPacboyButtonClick);
            ghostronButton.onClick.AddListener(OnGhostronButtonClick);
            propsButton.onClick.AddListener(OnPropsButtonClick);
            randomEventButton.onClick.AddListener(OnRandomEventButtonClick);
            backButton.onClick.AddListener(OnBackButtonClick);
            tutorialButton.onClick.AddListener(OnTutorialButtonClick);
        }

        // Pacboy button clicked
        private void OnPacboyButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Set button status
            SetButtonStatus(pacboyButton, true);
            SetButtonStatus(ghostronButton, false);
            SetButtonStatus(propsButton, false);
            SetButtonStatus(randomEventButton, false);

            // Update scroll rect content
            EmptyScrollRect();
            GameObject pacboyContentObject = Instantiate(pacboyContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = pacboyContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);
        }

        // Ghostron button clicked
        private void OnGhostronButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Set button status
            SetButtonStatus(pacboyButton, false);
            SetButtonStatus(ghostronButton, true);
            SetButtonStatus(propsButton, false);
            SetButtonStatus(randomEventButton, false);

            // Update scroll rect content
            EmptyScrollRect();
            GameObject ghostronContentObject = Instantiate(ghostronContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = ghostronContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);
        }

        // Props button clicked
        private void OnPropsButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Set button status
            SetButtonStatus(pacboyButton, false);
            SetButtonStatus(ghostronButton, false);
            SetButtonStatus(propsButton, true);
            SetButtonStatus(randomEventButton, false);

            // Update scroll rect content
            EmptyScrollRect();
            GameObject propsContentObject = Instantiate(propsContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = propsContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);
        }
        
        // Random event button clicked
        private void OnRandomEventButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Set button status
            SetButtonStatus(pacboyButton, false);
            SetButtonStatus(ghostronButton, false);
            SetButtonStatus(propsButton, false);
            SetButtonStatus(randomEventButton, true);
            
            // Update scroll rect content
            EmptyScrollRect();
            GameObject randomEventContentObject = Instantiate(randomEventContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = randomEventContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);
        }

        // Back button clicked: Return to the home page
        private void OnBackButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Go to home page
            pacpediaPage.SetActive(false);
            homePage.SetActive(true);
        }
        
        // Tutorial button clicked: Enter the tutorial scene
        private void OnTutorialButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Go to tutorial
            pacpediaPage.SetActive(false);
            // Update "Played before" status
            PlayerPrefs.SetInt("PlayedBefore", 1);
            // Load tutorial scene
            SceneManager.LoadScene("TutorialMap");
        }
    }
}