﻿using Sound;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    /**
     * Manages the event status setting mode in Map Editor.
     * 
     * This mode sets whether the game events can be triggered.
     * A game event might be triggered everytime a dot is eaten by Pacboy.
     * POSSIBLE EVENTS
     * - Super Bonus (All scores awarded are doubled)
     * - Crazy Party (Pacboy and Ghostrons are way faster)
     * - Air Walls (Walls become invisible)
     */
    public class EventEditor : MonoBehaviour {
        /* Buttons */
        public Button enableEventButton;
        public Button disableEventButton;

        // Current event status
        private bool _eventEnabled;

        // BUTTON COLORS
        private readonly Color _normalColor = new(140f / 255f, 121f / 255f, 255f / 255f);
        private readonly Color _selectedColor = new(228f / 255f, 255f / 255f, 73f / 255f);

        // Enabled/Disabled text prompts
        public GameObject eventEnabledPrompt;
        public GameObject eventDisabledPrompt;
        
        // Singleton instance
        public static EventEditor Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            SetButtonActionListener();
        }

        /**
         * Obtains the event status data.
         * Used by Map Editor when saving the map data.
         */
        public bool GetEventStatusData() {
            return _eventEnabled;
        }

        /**
         * Sets the event status data.
         * Used by Map Editor when entering the event setting mode.
         */
        public void SetEventStatusData(bool eventEnabled) {
            _eventEnabled = eventEnabled;
        }

        /**
         * Enters/Quits the event setting mode.
         * Used by Map Editor when user clicks the event setting mode.
         */
        public void SetEventMode(bool enter) {
            if (enter) {
                // Enter
                // Update UI based on the event status level
                if (_eventEnabled) {
                    OnEventEnabledButtonClick();
                } else {
                    OnEventDisabledButtonClick();
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

        /**
         * Sets action listeners for all the buttons.
         */
        private void SetButtonActionListener() {
            enableEventButton.onClick.AddListener(OnEventEnabledButtonClick);
            disableEventButton.onClick.AddListener(OnEventDisabledButtonClick);
        }

        /* Event status button operations */
        private void OnEventEnabledButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // UI update
            SetButtonStatus(enableEventButton, true);
            SetButtonStatus(disableEventButton, false);

            // Prompt update
            eventEnabledPrompt.SetActive(true);
            eventDisabledPrompt.SetActive(false);

            // Set event status data
            _eventEnabled = true;
        }

        private void OnEventDisabledButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // UI update
            SetButtonStatus(disableEventButton, true);
            SetButtonStatus(enableEventButton, false);

            // Prompt update
            eventDisabledPrompt.SetActive(true);
            eventEnabledPrompt.SetActive(false);

            // Set event status data
            _eventEnabled = false;
        }
    }
}