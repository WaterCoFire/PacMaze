using System.IO;
using System.Reflection;
using UnityEngine;

namespace Setting {
    /**
     * Manages the key binding information of the game.
     */
    public class KeyBindingManager {
        // Data file path
        private static readonly string FilePath = Path.Combine(Application.dataPath, "Data", "key_pref.json");

        /**
         * Saves key binding information to the file.
         */
        public static void SaveInfoToFile() {
            // Get all key binding information from player preferences
            KeyBindings keyBindings = new KeyBindings();
            foreach (FieldInfo field in typeof(KeyBindings).GetFields()) {
                string keyName = field.Name;
                string value = PlayerPrefs.GetString(keyName, "");
                field.SetValueDirect(__makeref(keyBindings), value);
            }

            // Transform to JSON and write to file
            string json = JsonUtility.ToJson(keyBindings, true);
            File.WriteAllText(FilePath, json);
            Debug.Log("KeyBindings saved to " + FilePath);
        }

        /**
         * Loads key binding information from the file.
         */
        public static void LoadInfoFromFile() {
            // Check file path validity
            if (!File.Exists(FilePath)) {
                Debug.LogError("key_pref.json file not found at " + FilePath);
                return;
            }

            // Read from JSON file
            string json = File.ReadAllText(FilePath);
            KeyBindings keyBindings = JsonUtility.FromJson<KeyBindings>(json);

            // Load to player preferences
            foreach (FieldInfo field in typeof(KeyBindings).GetFields()) {
                string keyName = field.Name;
                string value = (string)field.GetValue(keyBindings);
                PlayerPrefs.SetString(keyName, value);
            }

            Debug.Log("KeyBindings loaded from " + FilePath);
        }
    }
}