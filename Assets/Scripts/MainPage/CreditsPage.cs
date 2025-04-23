using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage {
    /**
     * Manages the credits page of the game.
     */
    public class CreditsPage : MonoBehaviour {
        private readonly string _chompManUrl =
            "https://assetstore.unity.com/packages/templates/tutorials/chomp-man-3d-game-kit-tutorial-174982";

        private readonly string _freePixelFontUrl =
            "https://assetstore.unity.com/packages/2d/fonts/free-pixel-font-thaleah-140059";

        private readonly string _simpleFreePixelArtStyledUiPackUrl =
            "https://assetstore.unity.com/packages/2d/gui/icons/simple-free-pixel-art-styled-ui-pack-165012";

        private readonly string _lowPolyObjectsPackUrl =
            "https://assetstore.unity.com/packages/3d/props/low-poly-objects-pack-241890";

        private readonly string _robotSphereUrl =
            "https://assetstore.unity.com/packages/3d/characters/robots/robot-sphere-136226";

        // This page
        public GameObject creditsPage;

        // Home page
        public GameObject homePage;

        // UI Buttons
        public Button chompManButton;
        public Button freePixelFontButton;
        public Button lowPolyObjectsPackButton;
        public Button robotSphereButton;
        public Button simpleFreePixelArtStyledUiPackButton;
        public Button backButton;

        private void Start() {
            // Bind action listeners
            // VISIT URL BUTTONS
            chompManButton.onClick.AddListener(OnChompManButtonClick);
            freePixelFontButton.onClick.AddListener(OnFreePixelFontButtonClick);
            lowPolyObjectsPackButton.onClick.AddListener(OnLowPolyObjectsPackButtonClick);
            robotSphereButton.onClick.AddListener(OnRobotSphereButtonClick);
            simpleFreePixelArtStyledUiPackButton.onClick.AddListener(OnSimpleFreePixelArtStyledUiPackButtonClick);

            // BACK BUTTON
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        /**
         * Opens the browser and visit the given URL.
         */
        private void OpenBrowser(string url) {
            Application.OpenURL(url);
        }

        /* Button action listeners */
        /* Visit corresponding URLs */
        private void OnChompManButtonClick() {
            OpenBrowser(_chompManUrl);
        }

        private void OnFreePixelFontButtonClick() {
            OpenBrowser(_freePixelFontUrl);
        }

        private void OnLowPolyObjectsPackButtonClick() {
            OpenBrowser(_lowPolyObjectsPackUrl);
        }

        private void OnRobotSphereButtonClick() {
            OpenBrowser(_robotSphereUrl);
        }

        private void OnSimpleFreePixelArtStyledUiPackButtonClick() {
            OpenBrowser(_simpleFreePixelArtStyledUiPackUrl);
        }


        /* Back button: back to the home page */
        private void OnBackButtonClick() {
            creditsPage.SetActive(false);
            PlayerPrefs.SetInt("MainPageAt", 0);
            homePage.SetActive(true);
        }
    }
}