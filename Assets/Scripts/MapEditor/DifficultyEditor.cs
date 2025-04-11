using System;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    public class DifficultyEditor : MonoBehaviour {
        // Difficulty setting buttons
        public Button easyButton;
        public Button normalButton;
        public Button hardButton;

        // COLORS
        // EASY - normal color
        // NORMAL - normal color
        // HARD - normal color
        // Selected color E4FF49
        private Color _easyColor = new Color(140f / 255f, 121f / 255f, 255f / 255f);
        private Color _normalColor = new Color(140f / 255f, 121f / 255f, 255f / 255f);
        private Color _hardColor = new Color(140f / 255f, 121f / 255f, 255f / 255f);
        private Color _selectedColor = new Color(228f / 255f, 255f / 255f, 73f / 255f);

        private bool _difficultyMode;
        private char _difficultySet;
        
        // START FUNCTION
        private void Start() {
            _difficultyMode = false;
            SetButtonActionListener();
        }

        // UPDATE FUNCTION
        private void Update() {
            if (!_difficultyMode) return;
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

                switch (correspondingDifficulty) {
                    case 'E':
                        colors.normalColor = _easyColor;
                        button.colors = colors;
                        break;
                    case 'N':
                        colors.normalColor = _normalColor;
                        button.colors = colors;
                        break;
                    case 'H':
                        colors.normalColor = _hardColor;
                        button.colors = colors;
                        break;
                }
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
            
            // Set difficulty data
            _difficultySet = 'E';
        }

        private void OnNormalButtonClick() {
            // UI update
            SetButtonStatus(easyButton, 'E', false);
            SetButtonStatus(normalButton, 'N', true);
            SetButtonStatus(hardButton, 'H', false);
            
            // Set difficulty data
            _difficultySet = 'N';
        }

        private void OnHardButtonClick() {
            // UI update
            SetButtonStatus(easyButton, 'E', false);
            SetButtonStatus(normalButton, 'N', false);
            SetButtonStatus(hardButton, 'H', true);
            
            // Set difficulty data
            _difficultySet = 'H';
        }
    }
}