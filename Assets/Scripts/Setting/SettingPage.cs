using System;
using System.Collections.Generic;
using System.IO;
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

        // Key information display texts
        public TMP_Text moveForwardKeyText;
        public TMP_Text moveBackWardKeyText;
        public TMP_Text moveLeftwardKeyText;
        public TMP_Text moveRightwardKeyText;
        public TMP_Text turnBackKeyText;
        public TMP_Text switchViewKeyText;
        public TMP_Text useNiceBombKeyText;
        public TMP_Text deployNiceBombKeyText;

        // Binding buttons
        public Button moveForwardSettingButton;
        public Button moveBackwardSettingButton;
        public Button moveLeftwardSettingButton;
        public Button moveRightwardSettingButton;
        public Button turnBackSettingButton;
        public Button switchViewSettingButton;
        public Button useNiceBombSettingButton;
        public Button deployNiceBombSettingButton;

        // Invalid operation prompt
        public TMP_Text promptText;

        private List<Button> _settingButtons = new();
        private bool _isListening; // Listening status
        private int _listeningIndex; // 1-8, indicating which operations are being set

        // START FUNCTION
        private void Start() {
            Debug.Log("SettingPage START");
            _settingButtons.Add(moveForwardSettingButton);
            _settingButtons.Add(moveBackwardSettingButton);
            _settingButtons.Add(moveLeftwardSettingButton);
            _settingButtons.Add(moveRightwardSettingButton);
            _settingButtons.Add(turnBackSettingButton);
            _settingButtons.Add(switchViewSettingButton);
            _settingButtons.Add(useNiceBombSettingButton);
            _settingButtons.Add(deployNiceBombSettingButton);

            _isListening = false;

            AddButtonActionListener(); // Set button action listeners
        }

        // UPDATE FUNCTION
        // Keeps listening for the new key code if _isListening is true
        private void Update() {
            if (!_isListening) return;

            // Check if any key down
            if (Input.anyKeyDown) {
                // Get the key pressed
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode))) {
                    if (Input.GetKeyDown(keyCode)) {
                        // Check the validity of the key code
                        if (!CheckValidity(keyCode)) {
                            // Warn the player if not valid
                            Debug.LogWarning("Invalid key pressed: " + keyCode);
                            promptText.text = "Invalid key! Only alphabetic keys allowed.";
                            promptText.gameObject.SetActive(true);

                            _isListening = false;
                            InitUI();
                            return;
                        }

                        // Key is valid
                        // Check for conflict
                        if (CheckConflict(_listeningIndex, keyCode)) {
                            Debug.LogWarning("Conflict detected: " + keyCode);
                            promptText.text = "This key conflicts with another one!";
                            promptText.gameObject.SetActive(true);

                            _isListening = false;
                            InitUI();
                            return;
                        }

                        // Set the key code
                        SetKeyCode(_listeningIndex, keyCode);
                        KeyBindingManager.SaveInfoToFile(); // Update in the file
                        _isListening = false;
                        InitUI();
                    }
                }
            }
        }

        /**
         * Initializes the UI.
         * Should be called every time this setting page is opened
         * and every time a new key code is set.
         */
        public void InitUI() {
            // Update key binding information
            KeyBindingManager.LoadInfoFromFile();

            // Key text update
            moveForwardKeyText.text = PlayerPrefs.GetString("ForwardKeyCode", "?");
            moveBackWardKeyText.text = PlayerPrefs.GetString("BackwardKeyCode", "?");
            moveLeftwardKeyText.text = PlayerPrefs.GetString("LeftwardKeyCode", "?");
            moveRightwardKeyText.text = PlayerPrefs.GetString("RightwardKeyCode", "?");
            turnBackKeyText.text = PlayerPrefs.GetString("TurnBackKeyCode", "?");
            switchViewKeyText.text = PlayerPrefs.GetString("SwitchViewKeyCode", "?");
            useNiceBombKeyText.text = PlayerPrefs.GetString("UseNiceBombKeyCode", "?");
            deployNiceBombKeyText.text = PlayerPrefs.GetString("DeployNiceBombKeyCode", "?");

            // Button status setting
            foreach (var button in _settingButtons) {
                button.GetComponentInChildren<TMP_Text>().text = "Update";
                button.interactable = true;
            }
        }

        /**
         * Sets the new key code.
         */
        private void SetKeyCode(int keyIndex, KeyCode newKeyCode) {
            // All keys in player preferences
            string[] keys = {
                "ForwardKeyCode", "BackwardKeyCode", "LeftwardKeyCode", "RightwardKeyCode", "TurnBackKeyCode",
                "SwitchViewKeyCode", "UseNiceBombKeyCode", "DeployNiceBombKeyCode"
            };

            // Get the corresponding key and set the key code
            Debug.Log("Setting successful: key " + keys[keyIndex - 1] + ", code: " + newKeyCode);
            PlayerPrefs.SetString(keys[keyIndex - 1], newKeyCode.ToString());

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
            } else {
                return false;
            }
        }

        /**
         * Checks for conflict when setting a new key code.
         * Returns true if conflict detected, false if fine.
         */
        private bool CheckConflict(int keyIndex, KeyCode newKeyCode) {
            // All keys in player preferences
            string[] keys = {
                "ForwardKeyCode", "BackwardKeyCode", "LeftwardKeyCode", "RightwardKeyCode", "TurnBackKeyCode",
                "SwitchViewKeyCode", "UseNiceBombKeyCode", "DeployNiceBombKeyCode"
            };

            // Check all the key codes for conflict
            foreach (var key in keys) {
                // Skip the own one
                if (key == keys[keyIndex - 1]) continue;
                // Get from player preferences
                string keyString = PlayerPrefs.GetString(key, null);
                // Parse from string to KeyCode
                if (System.Enum.TryParse(keyString, out KeyCode result)) {
                    if (result == newKeyCode) {
                        // Conflict
                        Debug.LogWarning("Setting: " + keyIndex + ", key code " + newKeyCode +
                                         " is conflict with: " +
                                         key);
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
        private void AddButtonActionListener() {
            moveForwardSettingButton.onClick.AddListener(OnMoveForwardSettingButtonClick);
            moveBackwardSettingButton.onClick.AddListener(OnMoveBackwardSettingButtonClick);
            moveLeftwardSettingButton.onClick.AddListener(OnMoveLeftwardSettingButtonClick);
            moveRightwardSettingButton.onClick.AddListener(OnMoveRightwardSettingButtonClick);
            turnBackSettingButton.onClick.AddListener(OnTurnBackSettingButtonClick);
            switchViewSettingButton.onClick.AddListener(OnSwitchViewSettingButtonClick);
            useNiceBombSettingButton.onClick.AddListener(OnUseNiceBombSettingButtonClick);
            deployNiceBombSettingButton.onClick.AddListener(OnDeployNiceBombSettingButtonClick);

            backButton.onClick.AddListener(OnBackButtonClick);
        }

        /* Button Action Listeners */
        // Move forward setting button
        private void OnMoveForwardSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 1;

            // Disable all the buttons except this one
            DisableAllButtons();
            moveForwardSettingButton.interactable = true;
            moveForwardSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Move backward setting button
        private void OnMoveBackwardSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 2;

            // Disable all the buttons except this one
            DisableAllButtons();
            moveBackwardSettingButton.interactable = true;
            moveBackwardSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Move leftward setting button
        private void OnMoveLeftwardSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 3;

            // Disable all the buttons except this one
            DisableAllButtons();
            moveLeftwardSettingButton.interactable = true;
            moveLeftwardSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Move rightward setting button
        private void OnMoveRightwardSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 4;

            // Disable all the buttons except this one
            DisableAllButtons();
            moveRightwardSettingButton.interactable = true;
            moveRightwardSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Turn back setting button
        private void OnTurnBackSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 5;

            // Disable all the buttons except this one
            DisableAllButtons();
            turnBackSettingButton.interactable = true;
            turnBackSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Switch view setting button
        private void OnSwitchViewSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 6;

            // Disable all the buttons except this one
            DisableAllButtons();
            switchViewSettingButton.interactable = true;
            switchViewSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Use nice bomb setting button
        private void OnUseNiceBombSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 7;

            // Disable all the buttons except this one
            DisableAllButtons();
            useNiceBombSettingButton.interactable = true;
            useNiceBombSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Deploy nice bomb setting button
        private void OnDeployNiceBombSettingButtonClick() {
            if (_isListening) return;
            _isListening = true;
            _listeningIndex = 8;

            // Disable all the buttons except this one
            DisableAllButtons();
            deployNiceBombSettingButton.interactable = true;
            deployNiceBombSettingButton.GetComponentInChildren<TMP_Text>().text = "Listening";
        }

        // Action when the back button is clicked.
        // Returns to the previous page.
        private void OnBackButtonClick() {
            _isListening = false;
            promptText.text = "";
            promptText.gameObject.SetActive(false);
            settingPage.SetActive(false);
            previousPage.SetActive(true);
        }

        /**
         * Sets all the setting buttons to not interact-able.
         */
        private void DisableAllButtons() {
            foreach (var button in _settingButtons) {
                button.interactable = false;
            }
        }
    }
}