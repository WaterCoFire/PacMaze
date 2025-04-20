using System;
using System.Net.Mime;
using PlayMap;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Entity.Pacman {
    /**
     * Manages the prop operation of the pacman.
     * In the current game design, when the pacman has picked up a nice bomb,
     * The player can press E (default) to directly use it (kills the nearest ghostron)
     * or press F (default) to deploy it, and kills two ghostrons when a ghostron hits it
     */
    public class PacmanPropOperation : MonoBehaviour {
        // The game object prefab of the deployed nice bomb
        // Has a different color for the player to tell its difference from un-picked ones
        public GameObject deployedNiceBombPrefab;

        // The number of nice bombs that the pacman has
        private int _niceBombs;

        private bool _onCooldown; // Status indicating if a prop is just being used
        private readonly float _cooldownDuration = 5.0f; // The duration between two consecutive use/deployment, 3 secs
        private float _cooldownTimer; // Cooldown timer

        // Nice bomb operation KeyCodes
        private KeyCode _useNiceBombKeyCode; // Use (default: E)
        private KeyCode _deployNiceBombKeyCode; // deploy (default: F)

        // UI elements
        private Button
            _niceBombButton; // Displayed when the player has nice bombs (Note: this button cannot be actually clicked)

        private TMP_Text _niceBombNumText; // Inside the button, shows the number of nice bombs the player currently has
        private TMP_Text _cooldownPromptText; // Inside the button, shows the cooldown status
        private TMP_Text _useNiceBombKeyPrompt; // Prompt indicating which key to press to use the nice bomb
        private TMP_Text _deployNiceBombKeyPrompt; // Prompt indicating which key to press to deploy the nice bomb

        // Disabled when game is not in normal process (e.g., paused)
        private bool _controllable;

        // START FUNCTION
        private void Start() {
            Debug.Log("PacmanPropOperation START");

            Debug.Log("Key: " + _useNiceBombKeyCode + ", " + _deployNiceBombKeyCode);

            _niceBombs = 0;
            _controllable = true;
            _cooldownTimer = 0f;

            // UI initialization
            GameObject niceBombButton = GameObject.Find("NiceBombButton");
            _niceBombButton = niceBombButton.GetComponent<Button>();
            _niceBombNumText = GameObject.Find("NiceBombNum").GetComponent<TMP_Text>();
            _cooldownPromptText = GameObject.Find("CooldownPrompt").GetComponent<TMP_Text>();
            _useNiceBombKeyPrompt = GameObject.Find("UseNiceBombKeyPrompt").GetComponent<TMP_Text>();
            _deployNiceBombKeyPrompt = GameObject.Find("DeployNiceBombKeyPrompt").GetComponent<TMP_Text>();

            _niceBombNumText.text = "0";
            _cooldownPromptText.gameObject.SetActive(false);
            _niceBombButton.gameObject.SetActive(false);

            // KeyCode setting and prompt updating
            UpdateOperationKey();
        }

        // UPDATE FUNCTION
        private void Update() {
            // No operation if not controllable
            if (!_controllable) return;

            // Cooldown handling logic
            if (_onCooldown) {
                _cooldownTimer += Time.deltaTime;

                // Check if the cooldown period time is over
                if (_cooldownTimer >= _cooldownDuration) {
                    _cooldownTimer = 0f;
                    _onCooldown = false;

                    // UI update
                    _cooldownPromptText.gameObject.SetActive(false);
                    _niceBombButton.interactable = true;
                }

                return; // No operation during cooldown
            }

            // No operation if no nice bombs currently obtained
            if (_niceBombs == 0) return;

            // Use nice bomb
            if (Input.GetKeyDown(_useNiceBombKeyCode)) {
                UseNiceBomb();
                return;
            }

            // Deploy nice bomb
            if (Input.GetKeyDown(_deployNiceBombKeyCode)) {
                DeployNiceBomb();
            }
        }

        /**
         * Pacman uses a nice bomb (default key code E).
         * Directly kills the ghostron nearest to the pacman.
         */
        private void UseNiceBomb() {
            Debug.Log("NiceBomb USE");
            // Reduce the number
            _niceBombs--;

            // Kill the ghostron nearest to the pacman
            GhostronManager.Instance.KillNearestGhostron(gameObject.transform.position);

            // Update cooldown status
            _cooldownTimer = 0f;
            _onCooldown = true;

            // UI update
            _cooldownPromptText.gameObject.SetActive(true);
            _niceBombButton.interactable = false;

            // Make the button disappear if no more bombs
            // Update display num otherwise
            if (_niceBombs == 0) {
                _niceBombButton.gameObject.SetActive(false);
            } else {
                _niceBombNumText.text = _niceBombs.ToString();
            }
        }

        /**
         * Pacman deploys a nice bomb (default key code F).
         * Places the deployed bomb at the current position.
         * Two ghostrons nearest to it will be killed when a ghostron hits the deployed bomb.
         */
        private void DeployNiceBomb() {
            Debug.Log("NiceBomb DEPLOY");
            // Reduce the number
            _niceBombs--;

            // Place the deployed bomb at the current location of the pacman
            Instantiate(deployedNiceBombPrefab, transform.position, Quaternion.identity);

            // Update cooldown status
            _cooldownTimer = 0f;
            _onCooldown = true;

            // UI update
            _cooldownPromptText.gameObject.SetActive(true);
            _niceBombButton.interactable = false;

            // Make the button disappear if no more bombs
            // Update display num otherwise
            if (_niceBombs == 0) {
                _niceBombButton.gameObject.SetActive(false);
            } else {
                _niceBombNumText.text = _niceBombs.ToString();
            }
        }

        /**
         * Grant a new nice bomb to the pacman.
         * Called when the pacman eats a new un-picked nice bomb object.
         */
        public void GetNiceBomb() {
            // One more nice bomb
            _niceBombs++;

            // UI update
            _niceBombNumText.text = _niceBombs.ToString();
            _niceBombButton.gameObject.SetActive(true);
        }
        
        /**
         * Update the key binding and prompts.
         * Let the player know which key to press to use/deploy nice bombs.
         */
        private void UpdateOperationKey() {
            _useNiceBombKeyCode = GetKeyCode("UseNiceBombKeyCode", KeyCode.E);
            _deployNiceBombKeyCode = GetKeyCode("DeployNiceBombKeyCode", KeyCode.F);

            _useNiceBombKeyPrompt.text = "Use    " + _useNiceBombKeyCode;
            _deployNiceBombKeyPrompt.text = "Deploy " + _deployNiceBombKeyCode;
        }

        /**
         * Trys to obtain the corresponding key code in player preferences.
         * If failed, a warning is shown.
         */
        private KeyCode GetKeyCode(string key, KeyCode defaultKeyCode) {
            string keyString = PlayerPrefs.GetString(key, defaultKeyCode.ToString());
            if (System.Enum.TryParse(keyString, out KeyCode result)) {
                return result;
            } else {
                // Default key code logic due to failure in parsing
                Debug.LogWarning(
                    $"The key value in player preferences {keyString} cannot be transformed to KeyCode, using default KeyCode: {defaultKeyCode}");
                return defaultKeyCode;
            }
        }

        /**
         * Allows the player to do prop operations.
         */
        public void EnablePropOperation() {
            _controllable = true;
            UpdateOperationKey(); // Update key binding information
        }

        /**
         * Stops the player from doing prop operations.
         */
        public void DisablePropOperation() {
            _controllable = false;
        }
    }
}