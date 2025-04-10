using System;
using System.IO;
using Entity.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace MapEditor {
    public class MapEditor : MonoBehaviour {
        public Button quitButton; // No saving
        public Button saveButton; // Save map and close

        // Normal color 8C79FF
        // Highlight color 634AFC
        // Selected color E4FF49
        private Color _normalColor = new Color(140f / 255f, 121f / 255f, 255f / 255f);
        private Color _selectedColor = new Color(228f / 255f, 255f / 255f, 73f / 255f);

        public Button wallModeButton; // Editing wall mode
        public Button propModeButton; // Editing prop mode

        // UI panels
        public GameObject wallModeSettingPanel;
        public GameObject propModeSettingPanel;

        public TMP_Text mapNameText;
        public TMP_Text modePromptText;

        public GameObject noPacmanSpawnWarningPanel;
        public Button warningPanelCloseButton;

        // Current edit mode
        // 0 - Disabled/Default
        // 1 - Walls
        // 2 - Props
        private int _mode;
        private string _mapName;

        // Map data save directory
        private string _saveDirectory;

        // Prompt Text
        // Default: Click to edit walls or props of your map!
        // Wall Mode: Editing Walls
        // Prop Mode: Editing Props

        private void Start() {
            // Set save directory
            _saveDirectory = Path.Combine(Application.dataPath, "Maps");

            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);

            SetButtonActionListener();
            InitUI("DEBUG TEST");

            // TEST TEST TEST
            PlayerPrefs.SetString("EditMapToLoad", "DEBUG TEST");
            LoadMap();
        }

        /**
         * Saves the map data to .json file.
         */
        private bool SaveMapToFile() {
            WallData wallData = gameObject.GetComponent<WallEditor>().GetWallData();
            PropData propData = gameObject.GetComponent<PropEditor>().GetPropData();

            Map map = new(_mapName, wallData, propData);
            
            // Get the number of total ghosts (will be part of the file name)
            int totalGhosts = propData.TotalPropCounts["GhostSpawn"];

            // Save to file in .json
            string json = JsonUtility.ToJson(new MapJsonWrapper(map), true);
            File.WriteAllText(Path.Combine(_saveDirectory, _mapName + "_" + totalGhosts + ".json"), json);
            Debug.Log("Map saved successfully: " + _mapName + ", location: " + _saveDirectory);

            return true;
        }

        /**
         * Loads the map data from the file. Used when the player enters the map editor.
         */
        private bool LoadMap() {
            // Read from player preferences
            string mapName = PlayerPrefs.GetString("EditMapToLoad", "");
            _mapName = mapName;

            // Obtain the file path
            string path = Path.Combine(_saveDirectory, _mapName + ".json");

            // Read the file
            if (!string.IsNullOrEmpty(mapName) && File.Exists(path)) {
                string json = File.ReadAllText(path);
                Debug.Log(json);
                MapJsonWrapper wrapper = JsonConvert.DeserializeObject<MapJsonWrapper>(json);

                Debug.Log("11111 " + wrapper.name);
                Debug.Log("11111 " + wrapper.played);
                Debug.Log("11111 " + wrapper.lastPlayedDateTime);
                Debug.Log("11111 " + wrapper.fastestTime);
                Debug.Log(wrapper.HorizontalWallStatus);
                Debug.Log(wrapper.VerticalWallStatus);
                Debug.Log(wrapper.FixedPropCounts);
                Debug.Log(wrapper.TotalPropCounts);

                // Initialize UI (set names etc.)
                InitUI(_mapName);

                // Set walls
                gameObject.GetComponent<WallEditor>()
                    .SetWallData(new WallData(wrapper.HorizontalWallStatus, wrapper.VerticalWallStatus));
                Debug.Log("Wall set");

                // Set props
                gameObject.GetComponent<PropEditor>().SetPropData(new PropData(wrapper.PropPositions(),
                    wrapper.FixedPropCounts, wrapper.TotalPropCounts));
                Debug.Log("Prop set");

                return true;
            } else {
                Debug.LogError("Load map error: File not found!");
                return false;
            }
        }

        // Edit Walls button operation
        private void OnEditWallsButtonClick() {
            // Update the prompt
            modePromptText.SetText("Editing:Walls");

            // Mode setting
            gameObject.GetComponent<PropEditor>().QuitPropMode();
            gameObject.GetComponent<WallEditor>().EnterWallMode();

            _mode = 1;

            // Buttons color update
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(wallModeButton, true);

            // Update the setting panel
            propModeSettingPanel.SetActive(false);
            wallModeSettingPanel.SetActive(true);
        }

        // Edit Props button operation
        private void OnEditPropsButtonClick() {
            // Update the prompt
            modePromptText.SetText("Editing:Props");

            // Mode setting
            gameObject.GetComponent<WallEditor>().QuitWallMode();
            gameObject.GetComponent<PropEditor>().EnterPropMode();

            _mode = 2;

            // Buttons color update
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(propModeButton, true);

            // Update the setting panel
            wallModeSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(true);
        }

        // Quit (directly) button operation
        private void OnQuitButtonClick() {
            InitUI(_mapName);
        }

        // Save & Quit button operation
        private void OnSaveAndQuitButtonClick() {
            // Check if the condition is met
            if (!gameObject.GetComponent<PropEditor>().CheckCondition()) {
                noPacmanSpawnWarningPanel.SetActive(true);
                
                // Temporarily disable both modes
                // (Recovered after the close button is clicked)
                gameObject.GetComponent<WallEditor>().QuitWallMode();
                gameObject.GetComponent<PropEditor>().QuitPropMode();

                return;
            }

            InitUI(_mapName);

            // Save map logic
            SaveMapToFile();
        }

        // Operations after the close button of the warning panel is clicked
        private void OnWarningPanelCloseButtonClick() {
            noPacmanSpawnWarningPanel.SetActive(false);

            switch (_mode) {
                case 0:
                    // In neither mode
                    gameObject.GetComponent<WallEditor>().QuitWallMode();
                    gameObject.GetComponent<PropEditor>().QuitPropMode();
                    break;
                case 1:
                    // In walls mode
                    gameObject.GetComponent<WallEditor>().EnterWallMode();
                    gameObject.GetComponent<PropEditor>().QuitPropMode();
                    break;
                case 2:
                    // In props mode
                    gameObject.GetComponent<WallEditor>().QuitWallMode();
                    gameObject.GetComponent<PropEditor>().EnterPropMode();
                    break;
                default:
                    Debug.LogError("Mode error!");
                    return;
            }
        }

        // Set the color of a button
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

        private void SetButtonActionListener() {
            quitButton.onClick.AddListener(OnQuitButtonClick);
            saveButton.onClick.AddListener(OnSaveAndQuitButtonClick);
            wallModeButton.onClick.AddListener(OnEditWallsButtonClick);
            propModeButton.onClick.AddListener(OnEditPropsButtonClick);
            warningPanelCloseButton.onClick.AddListener(OnWarningPanelCloseButtonClick);
        }

        // Initializes the map editor UI. 
        private void InitUI(string mapName) {
            // Map Name setting
            _mapName = mapName;
            mapNameText.text = mapName;

            // Reset the prompt
            modePromptText.SetText("Click to edit walls or props of your map!");

            // Clear of both modes
            gameObject.GetComponent<WallEditor>().QuitWallMode();
            gameObject.GetComponent<PropEditor>().QuitPropMode();
            _mode = 0;

            // Reset the color of both buttons
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(propModeButton, false);

            // Reset both setting panels
            wallModeSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(false);
        }
    }
}