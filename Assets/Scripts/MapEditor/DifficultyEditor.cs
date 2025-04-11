using System;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    /**
     * DIFFICULTY SETTING
     * Easy:
     * - Lucky Dice won't bring in any negative effect (Slow Wheel, Bad Cherry)
     *   (50% Fast Wheel, 25% Power Pellet, 25% Nice Bomb)
     * - Ghosts move slowly when chasing
     * - A ghost would chase pacman only if it is CLOSE to pacman
     *
     * Normal:
     * - Lucky Dice would bring any effect
     *   (20% Power Pellet, 20% Fast Wheel, 20% Nice Bomb, 20% Slow Wheel, 20% Bad Cherry)
     * - Ghosts move at normal speed when chasing
     * - A ghost would chase pacman if it is NOT FAR from pacman
     *
     * Hard:
     * - Lucky Dice would more likely bring a negative effect
     *   (20% Fast Wheel, 15% Power Pellet, 15% Nice Bomb, 25% Slow Wheel, 25% Bad Cherry)
     * - Ghosts move faster when chasing
     * - Ghosts would ALWAYS chase the pacman, despite the distance
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

        // COLORS
        private Color _normalColor = new Color(140f / 255f, 121f / 255f, 255f / 255f);
        private Color _selectedColor = new Color(228f / 255f, 255f / 255f, 73f / 255f);

        private bool _difficultyMode;
        private char _difficultySet;
        
        // START FUNCTION
        private void Start() {
            _difficultyMode = false;
            SetButtonActionListener();
        }

        /**
         * Obtains the difficulty data.
         * Used by Map Editor when saving the map data.
         */
        public char GetDifficultyData() {
            return _difficultySet;
        }

        /**
         * Sets the difficulty data.
         * Used by Map Editor when entering the difficulty setting mode.
         */
        public void SetDifficultyData(char difficulty) {
            _difficultySet = difficulty;
        }

        /**
         * Enters the difficulty setting mode.
         * Used by Map Editor when user clicks the difficulty setting mode.
         */
        public void EnterDifficultyMode() {
            _difficultyMode = true;

            // Update UI based on the difficulty level
            switch (_difficultySet) {
                case 'E':
                    OnEasyButtonClick();
                    break;
                case 'N':
                    OnNormalButtonClick();
                    break;
                case 'H':
                    OnHardButtonClick();
                    break;
            }
        }

        /**
         * Quit the difficulty setting mode.
         * Used by Map Editor when user quits the map editor or enters other setting modes.
         */
        public void QuitDifficultyMode() {
            _difficultyMode = false;
        }

        // Set the color of a button
        private void SetButtonStatus(Button button, char correspondingDifficulty, bool selected) {
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
            // UI update
            SetButtonStatus(easyButton, 'E', true);
            SetButtonStatus(normalButton, 'N', false);
            SetButtonStatus(hardButton, 'H', false);
            
            // Prompt update
            easyModePrompt.SetActive(true);
            normalModePrompt.SetActive(false);
            hardModePrompt.SetActive(false);
            
            // Set difficulty data
            _difficultySet = 'E';
        }

        private void OnNormalButtonClick() {
            // UI update
            SetButtonStatus(easyButton, 'E', false);
            SetButtonStatus(normalButton, 'N', true);
            SetButtonStatus(hardButton, 'H', false);
            
            // Prompt update
            easyModePrompt.SetActive(false);
            normalModePrompt.SetActive(true);
            hardModePrompt.SetActive(false);
            
            // Set difficulty data
            _difficultySet = 'N';
        }

        private void OnHardButtonClick() {
            // UI update
            SetButtonStatus(easyButton, 'E', false);
            SetButtonStatus(normalButton, 'N', false);
            SetButtonStatus(hardButton, 'H', true);
            
            // Prompt update
            easyModePrompt.SetActive(false);
            normalModePrompt.SetActive(false);
            hardModePrompt.SetActive(true);
            
            // Set difficulty data
            _difficultySet = 'H';
        }
    }
}