using System;
using Sound;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    /**
     * Manages the help page in Map Editor.
     * Automatically displays when it is the first time player uses Map Editor
     * Also displays when the player clicks the Help Button in Map Editor
     */
    public class MapEditorHelpPage : MonoBehaviour {
        public GameObject helpPage; // This help page
        public Button helpButton; // The help button in the Map Editor UI
        public Button closeButton; // The button to close help page

        /* Scroll Rect used to display contents */
        public ScrollRect contentScrollRect;

        /* Help content prefab */
        public GameObject helpContentPrefab;

        // Singleton instance
        public static MapEditorHelpPage Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            // Set button action listeners
            SetButtonActionListener();
        }

        /**
         * Displays the help page:
         * Disables operations in the Map Editor
         * puts the content prefab into the ScrollRect
         * and shows the whole page
         */
        public void ShowHelpPage() {
            // Temporarily disable all modes in Map Editor
            MapEditor.Instance.SwitchMode(MapEditorMode.None, true);

            // Temporarily ban all the buttons
            // (Recovered after the close button is clicked)
            MapEditor.Instance.SetButtonsAvailability(false);
            helpButton.interactable = false;
            
            // Content scroll rect initialisation
            contentScrollRect.verticalNormalizedPosition = 0f;
            GameObject helpContentObject = Instantiate(helpContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = helpContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);

            // Display help page
            helpPage.SetActive(true);
        }

        /**
         * Sets the action listeners for all buttons.
         */
        private void SetButtonActionListener() {
            helpButton.onClick.AddListener(OnHelpButtonClick);
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        /* Button action listeners */
        // Help button in Map Editor
        private void OnHelpButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Show the help page
            ShowHelpPage();
        }

        // Close button in help page
        private void OnCloseButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Close the help page
            helpPage.SetActive(false);

            // Set the Map Editor mode back to the previous one
            MapEditor.Instance.SwitchMode(MapEditor.Instance.GetMode(), true);

            // Enable all buttons
            MapEditor.Instance.SetButtonsAvailability(true);
            helpButton.interactable = true;

            // Set the player to be having already used Map Editor
            PlayerPrefs.SetInt("UsedMapEditorBefore", 1);
        }
    }
}