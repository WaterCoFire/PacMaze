﻿using PlayMap;
using PlayMap.UI;
using Sound;
using UnityEngine;

namespace Entity.Pacboy {
    /**
     * Manages the prop operation of the Pacboy.
     * In the current game design, when the Pacboy has picked up a nice bomb,
     * The player can press E (default) to directly use it (kills the nearest Ghostron)
     * or press F (default) to deploy it, and kills two Ghostrons when a Ghostron hits it
     */
    public class PacboyPropOperation : MonoBehaviour {
        // The game object prefab of the deployed nice bomb
        // Has a different color for the player to tell its difference from un-picked ones
        public GameObject deployedNiceBombPrefab;

        // The number of nice bombs that the Pacboy has
        private int _niceBombs;

        private bool _onCooldown; // Whether a prop is just being used
        private readonly float _cooldownDuration = 5.0f; // The duration between two consecutive use/deployment, 5 secs
        private float _cooldownTimer; // Cooldown timer

        // Nice bomb operation KeyCodes
        private KeyCode _useNiceBombKeyCode; // Use (default: E)
        private KeyCode _deployNiceBombKeyCode; // Deploy (default: F)

        // Disabled when game is not in normal process (e.g. paused)
        private bool _controllable;

        // START FUNCTION
        private void Start() {
            // Initialisation
            _niceBombs = 0;
            _cooldownTimer = 0f;
            
            EnablePropOperation(); // Enable Pacboy prop operation when game starts
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

                    // UI update (Now the Nice Bomb is available again)
                    GamePlayUI.Instance.SetNiceBombCooldown(false);
                }

                return; // No operation during cooldown
            }

            // No operation if no Nice Bombs currently obtained
            if (_niceBombs == 0) return;

            // Use Nice Bomb
            if (Input.GetKeyDown(_useNiceBombKeyCode)) {
                UseNiceBomb();
                return;
            }

            // Deploy Nice Bomb
            if (Input.GetKeyDown(_deployNiceBombKeyCode)) {
                DeployNiceBomb();
            }
        }

        /**
         * Pacboy uses a Nice Bomb (default key code E).
         * Directly kills the Ghostron nearest to the Pacboy.
         */
        private void UseNiceBomb() {
            // Reduce the number
            _niceBombs--;

            // Kill the Ghostron nearest to the Pacboy
            if (GhostronManager.Instance.KillNearestGhostron(gameObject.transform.position)) {
                // Killing is successful
                // Play explode sound
                SoundManager.Instance.PlaySoundOnce(SoundType.NiceBombExplode);
                
                // Give the Pacboy 200 score points 
                PlayMapController.Instance.AddScore(200);
                
                // Prompt the player
                GamePlayUI.Instance.NewInfo("Boom! You killed a Ghostron!", Color.green);
            }

            // Update cooldown status
            _cooldownTimer = 0f;
            _onCooldown = true;

            // UI update (Now the Nice Bomb should be temporarily banned for cooldown)
            GamePlayUI.Instance.SetNiceBombCooldown(true);

            // Make the button disappear if no more bombs
            // Update display num otherwise
            if (_niceBombs == 0) {
                GamePlayUI.Instance.SetNiceBombPrompt(false);
            } else {
                GamePlayUI.Instance.UpdateNiceBombNum(_niceBombs);
            }
        }

        /**
         * Pacboy deploys a Nice Bomb (default key code F).
         * Places the Deployed Nice Bomb at the current position.
         * Two Ghostrons nearest to it will be killed when a Ghostron hits the Deployed Nice Bomb.
         */
        private void DeployNiceBomb() {
            // Play deploy sound
            SoundManager.Instance.PlaySoundOnce(SoundType.DeployNiceBomb);
            
            // Reduce the number
            _niceBombs--;
            
            // Prompt the player
            GamePlayUI.Instance.NewInfo("Nice Bomb deployed!", Color.cyan);

            // Place the deployed bomb at the current location of the Pacboy
            Instantiate(deployedNiceBombPrefab, transform.position, Quaternion.identity);

            // Update cooldown status
            _cooldownTimer = 0f;
            _onCooldown = true;

            // UI update (Now the Nice Bomb should be temporarily banned for cooldown)
            GamePlayUI.Instance.SetNiceBombCooldown(true);

            // Make the button disappear if no more bombs
            // Update display num otherwise
            if (_niceBombs == 0) {
                GamePlayUI.Instance.SetNiceBombPrompt(false);
            } else {
                GamePlayUI.Instance.UpdateNiceBombNum(_niceBombs);
            }
        }

        /**
         * Grant a new nice bomb to the Pacboy.
         * Called when the Pacboy eats a new un-picked nice bomb object.
         */
        public void GetNiceBomb() {
            // One more nice bomb
            _niceBombs++;

            // UI update
            GamePlayUI.Instance.UpdateNiceBombNum(_niceBombs);
            GamePlayUI.Instance.SetNiceBombPrompt(true);
        }

        /**
         * Updates the KeyCode setting (Nice Bomb Use/Deploy).
         */
        private void UpdateKeyCodes() {
            _useNiceBombKeyCode = GetKeyCode("UseNiceBombKeyCode", KeyCode.E);
            _deployNiceBombKeyCode = GetKeyCode("DeployNiceBombKeyCode", KeyCode.F);

            // UI prompt update
            GamePlayUI.Instance.UpdateNiceBombOperationKeyPrompts(_useNiceBombKeyCode, _deployNiceBombKeyCode);
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
            UpdateKeyCodes(); // Update key binding information
        }

        /**
         * Stops the player from doing prop operations.
         */
        public void DisablePropOperation() {
            _controllable = false;
        }
    }
}