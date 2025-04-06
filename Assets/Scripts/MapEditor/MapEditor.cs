using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    public class MapEditor : MonoBehaviour {
        public Button quitButton; // No saving
        public Button saveButton; // Save map and close
        
        // Normal color 8C79FF
        // Highlight color E4FF49
        public Button wallModeButton; // Editing wall mode
        public Button propModeButton; // Eduting prop mode

        public TMP_Text mapNameText;
        public TMP_Text modePromptText;
        
        // Prompt Text
        // Default: Click to edit walls or props of your map!
        // Wall Mode: Editing Walls
        // Prop Mode: Editing Props

        private void Start() {
            modePromptText.SetText("Click to edit walls or props of your map!");
        }
    }
}