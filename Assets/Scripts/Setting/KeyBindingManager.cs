using System.IO;
using UnityEngine;

namespace Setting {
    /**
     * Manages the key binding information of the game.
     */
    public class KeyBindingManager {
        // INFORMATION FILE PATH
        private static string _filePath = Path.Combine(Application.dataPath, "Data", "key_pref.json");

        /**
         * Saves key binding information to the file.
         */
        public static void SaveInfoToFile() {
            // Create new struct for storing key binding information
            KeyBindings keyBindings = new KeyBindings {
                ForwardKeyCode = PlayerPrefs.GetString("ForwardKeyCode", "W"),
                BackwardKeyCode = PlayerPrefs.GetString("BackwardKeyCode", "S"),
                LeftwardKeyCode = PlayerPrefs.GetString("LeftwardKeyCode", "A"),
                RightwardKeyCode = PlayerPrefs.GetString("RightwardKeyCode", "D"),
                TurnBackKeyCode = PlayerPrefs.GetString("TurnBackKeyCode", "Q"),
                SwitchViewKeyCode = PlayerPrefs.GetString("SwitchViewKeyCode", "V"),
                OpenMapKeyCode = PlayerPrefs.GetString("OpenMapKeyCode", "M"),
                UseNiceBombKeyCode = PlayerPrefs.GetString("UseNiceBombKeyCode", "E"),
                DeployNiceBombKeyCode = PlayerPrefs.GetString("DeployNiceBombKeyCode", "F")
            };

            // Transform to json
            string json = JsonUtility.ToJson(keyBindings, true);

            // Write to the file
            File.WriteAllText(_filePath, json);
            Debug.Log("KeyBindings saved to " + _filePath);
        }

        /**
         * Loads key binding information from the file.
         */
        public static void LoadInfoFromFile() {
            // Check file path validity
            if (File.Exists(_filePath)) {
                string json = File.ReadAllText(_filePath);

                KeyBindings keyBindings = JsonUtility.FromJson<KeyBindings>(json);

                // Load to player preferences
                PlayerPrefs.SetString("ForwardKeyCode", keyBindings.ForwardKeyCode);
                PlayerPrefs.SetString("BackwardKeyCode", keyBindings.BackwardKeyCode);
                PlayerPrefs.SetString("LeftwardKeyCode", keyBindings.LeftwardKeyCode);
                PlayerPrefs.SetString("RightwardKeyCode", keyBindings.RightwardKeyCode);
                PlayerPrefs.SetString("TurnBackKeyCode", keyBindings.TurnBackKeyCode);
                PlayerPrefs.SetString("SwitchViewKeyCode", keyBindings.SwitchViewKeyCode);
                PlayerPrefs.SetString("OpenMapKeyCode", keyBindings.OpenMapKeyCode);
                PlayerPrefs.SetString("UseNiceBombKeyCode", keyBindings.UseNiceBombKeyCode);
                PlayerPrefs.SetString("DeployNiceBombKeyCode", keyBindings.DeployNiceBombKeyCode);

                Debug.Log("KeyBindings loaded from " + _filePath);
            } else {
                Debug.LogError("key_pref.json file not found at " + _filePath);
            }
        }
    }
}