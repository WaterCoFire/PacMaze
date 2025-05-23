using System;
using UnityEngine;
using UnityEngine.UI;

namespace HomePage {
    /**
     * Pacpedia Page
     */
    public class PacpediaPage : MonoBehaviour {
        // This page and the home page
        public GameObject pacpediaPage;
        public GameObject homePage;

        /* Buttons */
        public Button pacboyButton;
        public Button ghostronButton;
        public Button propsButton;

        public Button backButton; // Back to home page

        /* Scroll Rect used to display contents */
        public ScrollRect contentScrollRect;

        /* Content prefabs: Pacboy, Ghostron, Props */
        public GameObject pacboyContentPrefab;
        public GameObject ghostronContentPrefab;
        public GameObject propsContentPrefab;

        // Button Colours
        // Normal color 8C79FF
        // Highlight color 634AFC
        // Selected color E4FF49
        private readonly Color _normalColor = new(140f / 255f, 121f / 255f, 255f / 255f);
        private readonly Color _selectedColor = new(228f / 255f, 255f / 255f, 73f / 255f);

        // START FUNCTION
        private void Start() {
            Debug.Log("PacpediaPage START");

            // Set button action listeners
            SetButtonActionListener();
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
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        // Pacboy button clicked
        private void OnPacboyButtonClick() {
            // Set button status
            SetButtonStatus(pacboyButton, true);
            SetButtonStatus(ghostronButton, false);
            SetButtonStatus(propsButton, false);

            // Update scroll rect content
            EmptyScrollRect();
            GameObject pacboyContentObject = Instantiate(pacboyContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = pacboyContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);
        }

        // Ghostron button clicked
        private void OnGhostronButtonClick() {
            // Set button status
            SetButtonStatus(pacboyButton, false);
            SetButtonStatus(ghostronButton, true);
            SetButtonStatus(propsButton, false);

            // Update scroll rect content
            EmptyScrollRect();
            GameObject ghostronContentObject = Instantiate(ghostronContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = ghostronContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);
        }

        // Props button clicked
        private void OnPropsButtonClick() {
            // Set button status
            SetButtonStatus(pacboyButton, false);
            SetButtonStatus(ghostronButton, false);
            SetButtonStatus(propsButton, true);

            // Update scroll rect content
            EmptyScrollRect();
            GameObject propsContentObject = Instantiate(propsContentPrefab, contentScrollRect.content);
            RectTransform itemTransform = propsContentObject.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(0f, 0f);
        }

        // Back button clicked: Return to the home page
        private void OnBackButtonClick() {
            pacpediaPage.SetActive(false);
            PlayerPrefs.SetInt("MainPageAt", 0);
            homePage.SetActive(true);
        }
    }
}