﻿using Entity.Map;
using Sound;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    /**
     * Manages the difficulty setting mode in Map Editor.
     * Easy:
     * - Lucky Dice won't bring in any negative effect (Slow Wheel, Bad Cherry)
     *   (50% Fast Wheel, 25% Power Pellet, 25% Nice Bomb)
     * - Ghostrons move slowly when chasing
     * - Ghostrons are easy to deal with
     *
     * Normal:
     * - Lucky Dice would bring any effect
     *   (20% Power Pellet, 20% Fast Wheel, 20% Nice Bomb, 20% Slow Wheel, 20% Bad Cherry)
     * - Ghostrons move at normal speed when chasing
     * - A Ghostron would chase Pacboy if it is NOT FAR from Pacboy
     * - Ghostrons are not hard to deal with
     *
     * Hard:
     * - Lucky Dice would more likely bring a negative effect
     *   (20% Fast Wheel, 15% Power Pellet, 15% Nice Bomb, 25% Slow Wheel, 25% Bad Cherry)
     * - Ghostrons move faster when chasing
     * - Ghostrons wouldn't stop chasing Pacboy unless it is FAR ENOUGH from Pacboy
     * - Ghostrons are very hard to deal with
     */
    public class DifficultyEditor : MonoBehaviour {
        // Difficulty setting buttons
        public Button easyButton;
        public Button normalButton;
        public Button hardButton;

        // The prompts describing every difficulty level
        public GameObject easyModePrompt;
        public GameObject normalModePrompt;
        public GameObject hardModePrompt;

        // BUTTON COLORS
        private readonly Color _normalColor = new(140f / 255f, 121f / 255f, 255f / 255f);
        private readonly Color _selectedColor = new(228f / 255f, 255f / 255f, 73f / 255f);

        // Current difficulty
        private DifficultyType _difficultySet;

        // Singleton instance
        public static DifficultyEditor Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            SetButtonActionListener(); // Initialise button action listeners
        }

        /**
         * Obtains the difficulty data.
         * Used by Map Editor when saving the map data.
         */
        public DifficultyType GetDifficultyData() {
            return _difficultySet;
        }

        /**
         * Sets the difficulty data.
         * Used by Map Editor when entering the difficulty setting mode.
         */
        public void SetDifficultyData(DifficultyType difficulty) {
            _difficultySet = difficulty;
        }

        /**
         * Enters/Quits the difficulty setting mode.
         */
        public void SetDifficultyMode(bool enter) {
            if (enter) {
                // Enter
                // Update UI based on the difficulty level
                switch (_difficultySet) {
                    case DifficultyType.Easy:
                        OnEasyButtonClick();
                        break;
                    case DifficultyType.Normal:
                        OnNormalButtonClick();
                        break;
                    case DifficultyType.Hard:
                        OnHardButtonClick();
                        break;
                }
            }
            // Quit: No logic
        }

        /**
         * Sets the color of a button.
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

        private void SetButtonActionListener() {
            easyButton.onClick.AddListener(OnEasyButtonClick);
            normalButton.onClick.AddListener(OnNormalButtonClick);
            hardButton.onClick.AddListener(OnHardButtonClick);
        }

        /* Difficulty button operations */
        private void OnEasyButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // UI update
            SetButtonStatus(easyButton, true);
            SetButtonStatus(normalButton, false);
            SetButtonStatus(hardButton, false);

            // Prompt update
            easyModePrompt.SetActive(true);
            normalModePrompt.SetActive(false);
            hardModePrompt.SetActive(false);

            // Set difficulty data
            _difficultySet = DifficultyType.Easy;
        }

        private void OnNormalButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // UI update
            SetButtonStatus(easyButton, false);
            SetButtonStatus(normalButton, true);
            SetButtonStatus(hardButton, false);

            // Prompt update
            easyModePrompt.SetActive(false);
            normalModePrompt.SetActive(true);
            hardModePrompt.SetActive(false);

            // Set difficulty data
            _difficultySet = DifficultyType.Normal;
        }

        private void OnHardButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // UI update
            SetButtonStatus(easyButton, false);
            SetButtonStatus(normalButton, false);
            SetButtonStatus(hardButton, true);

            // Prompt update
            easyModePrompt.SetActive(false);
            normalModePrompt.SetActive(false);
            hardModePrompt.SetActive(true);

            // Set difficulty data
            _difficultySet = DifficultyType.Hard;
        }
    }
}