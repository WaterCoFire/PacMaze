using System;
using System.Collections.Generic;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Setting {
    /**
     * Manages the UI of key setting page.
     */
    public class SettingPage : MonoBehaviour {
        // This setting page and the previous page
        public GameObject settingPage;
        public GameObject previousPage;

        // Back button
        public Button backButton;

        // Invalid operation prompt
        public TMP_Text promptText;
        private bool _isListening; // Listening status
        private string _listeningKeyName; // The name of the key code that are currently being listened
        
        // The list of definitions of UI elements for each key code setting
        public List<SettingUIDefinition> settingUIDefinitions;

        // START FUNCTION
        private void Start() {
            Debug.Log("SettingPage START");

            _isListening = false;

            SetButtonActionListener(); // Set button action listeners
        }

        // UPDATE FUNCTION
        // Keeps listening for the new key code if _isListening is true
        private void Update() {
            if (!_isListening) return;

            // Check if any key down
            if (Input.anyKeyDown) {
                // Get the key pressed
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) {
                    if (Input.GetKeyDown(keyCode)) {
                        // Check the validity of the key code
                        if (!CheckValidity(keyCode)) {
                            // Warn the player if not valid
                            // Play warning sound
                            SoundManager.Instance.PlaySoundOnce(SoundType.Warning);

                            // Update prompt
                            promptText.text = "Invalid key! Only alphabetic keys allowed.";
                            promptText.gameObject.SetActive(true);

                            _isListening = false;
                            InitUI();
                            return;
                        }

                        // Key is valid
                        // Check for conflict
                        if (CheckConflict(_listeningKeyName, keyCode)) {
                            // Conflict detected
                            // Play warning sound
                            SoundManager.Instance.PlaySoundOnce(SoundType.Warning);

                            // Update prompt
                            promptText.text = "This key conflicts with another one!";
                            promptText.gameObject.SetActive(true);

                            _isListening = false;
                            InitUI();
                            return;
                        }

                        // All valid
                        // Set the key code
                        SetKeyCode(_listeningKeyName, keyCode);
                        KeyBindingManager.SaveInfoToFile(); // Update in the file
                        _isListening = false;
                        InitUI();
                    }
                }
            }
        }

        /**
         * Initialises the UI.
         * Should be called every time this setting page is opened
         * and every time a new key code is set.
         */
        public void InitUI() {
            // Update key binding information
            KeyBindingManager.LoadInfoFromFile();

            // Key text and button status updating
            foreach (var settingUI in settingUIDefinitions) {
                settingUI.keyText.text = PlayerPrefs.GetString(settingUI.keyName, "?");
                settingUI.settingButton.GetComponentInChildren<TMP_Text>().text = "Update";
                settingUI.settingButton.interactable = true;
            }
        }

        /**
         * Sets the new key code.
         */
        private void SetKeyCode(string keyName, KeyCode newKeyCode) {
            // Get the corresponding key and set the key code
            Debug.Log("Setting successful: key " + keyName + ", code: " + newKeyCode);
            PlayerPrefs.SetString(keyName, newKeyCode.ToString());

            promptText.text = "";
            promptText.gameObject.SetActive(false);
        }

        /**
         * Checks for the validity of the new key code.
         * (A-Z)
         * Returns true if valid, false otherwise.
         */
        private bool CheckValidity(KeyCode newKeyCode) {
            promptText.text = "";
            promptText.gameObject.SetActive(false);

            if (newKeyCode >= KeyCode.A && newKeyCode <= KeyCode.Z) {
                return true;
            }

            return false;
        }

        /**
         * Checks for conflict when setting a new key code.
         * Returns true if conflict detected, false if fine.
         */
        private bool CheckConflict(string keyName, KeyCode newKeyCode) {
            // Check all the key codes for conflict
            foreach (var settingUI in settingUIDefinitions) {
                // Skip the own one
                if (settingUI.keyName == keyName) continue;
                // Get from player preferences
                string keyString = PlayerPrefs.GetString(settingUI.keyName, null);
                // Parse from string to KeyCode
                if (Enum.TryParse(keyString, out KeyCode result)) {
                    if (result == newKeyCode) {
                        // Conflict
                        return true;
                    }
                } else {
                    // Parse error
                    Debug.LogWarning(
                        $"The key value in player preferences {keyString} cannot be transformed to KeyCode!");
                    return true;
                }
            }

            // Return true if there is no conflict
            return false;
        }
        
        
        /**
         * Sets the action listeners for all the buttons.
         */
        private void SetButtonActionListener() {
            // Setting button for all key codes
            foreach (var settingUI in settingUIDefinitions) {
                if (settingUI.settingButton != null) {
                    settingUI.settingButton.onClick.RemoveAllListeners(); // Clear existing
                    settingUI.settingButton.onClick.AddListener(() =>
                        OnSettingButtonClick(settingUI.keyName));
                }
            }
            
            // Back button
            backButton.onClick.AddListener(OnBackButtonClick);
        }
        

        // Action when the back button is clicked.
        // Returns to the previous page.
        private void OnBackButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Reset UI
            _isListening = false;
            promptText.text = "";
            promptText.gameObject.SetActive(false);

            // Back to the previous page
            settingPage.SetActive(false);
            previousPage.SetActive(true);
        }

        // Action when the setting button of a key code is pressed.
        private void OnSettingButtonClick(string keyName) {
            // No action if a listening is already taking place
            if (_isListening) return;
            _isListening = true;
            _listeningKeyName = keyName;

            // Disable all the buttons except this one
            foreach (var settingUI in settingUIDefinitions) {
                if (settingUI.keyName == keyName) {
                    settingUI.settingButton.interactable = true;
                    settingUI.settingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
                } else {
                    settingUI.settingButton.interactable = false;
                }
            }
        }

        /**
         * The UI setting unit for each key code.
         */
        [Serializable]
        public class SettingUIDefinition {
            // Key type name (e.g., "ForwardKeyCode")
            public string keyName;

            // Current key information display text
            public TMP_Text keyText;

            // The setting button
            public Button settingButton;
        }
    }
}