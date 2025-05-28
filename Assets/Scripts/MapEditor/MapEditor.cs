using System;
using System.IO;
using System.Text.RegularExpressions;
using Entity.Map;
using Entity.Prop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Sound;
using UnityEngine.SceneManagement;

namespace MapEditor {
    public class MapEditor : MonoBehaviour {
        public Button quitButton; // No saving
        public Button saveButton; // Save map and close

        // Button Colours
        // Normal color 8C79FF
        // Highlight color 634AFC
        // Selected color E4FF49
        private readonly Color _normalColor = new(140f / 255f, 121f / 255f, 255f / 255f);
        private readonly Color _selectedColor = new(228f / 255f, 255f / 255f, 73f / 255f);

        public Button wallModeButton; // Editing wall mode
        public Button propModeButton; // Editing prop mode
        public Button difficultyModeButton; // Difficulty setting mode
        public Button eventModeButton; // Event status setting mode

        // UI panels
        public GameObject wallModeSettingPanel;
        public GameObject propModeSettingPanel;
        public GameObject difficultySettingPanel;
        public GameObject eventSettingPanel;

        public TMP_Text mapNameText;
        
        // Prompt Text
        // Default: Click to select what you need to edit!
        // Wall Mode: Editing Walls
        // Prop Mode: Editing Props
        // Difficulty Mode: Editing Difficulty
        // Event Mode: Editing Event Status
        public TMP_Text modePromptText;

        /* Warning UI */
        // Warning panel: Pacboy spawn point not set
        public GameObject noPacboySpawnWarningPanel;
        public Button noPacboyWarningPanelCloseButton;
        
        // Warning panel: Invalid tiles exist
        public GameObject invalidTilesWarningPanel;
        public Button invalidTilesPanelCloseButton;

        // Current edit mode
        // 0 - Disabled/Default
        // 1 - Walls
        // 2 - Props
        // 3 - Difficulty
        // 4 - Event
        private int _mode;
        private string _mapName;

        // Map data save directory
        private readonly string _saveDirectory = Path.Combine(Application.dataPath, "Data", "Maps");
        private readonly Regex _regex = new(@"^([^_]+)_(\d+)_(\d+)");

        // START FUNCTION
        private void Start() {
            Debug.Log("MapEditor START");

            // Play background music
            SoundManager.Instance.PlayBackgroundMusic(false);
            
            // Create map data directory, if it does not exist
            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
            
            // Mode & UI Reset
            _mode = 0;
            WallEditor.Instance.QuitWallMode();
            PropEditor.Instance.QuitPropMode(true);
            DifficultyEditor.Instance.QuitDifficultyMode();
            EventEditor.Instance.QuitEventMode();
            TileChecker.Instance.ClearTileDisplay();
            noPacboySpawnWarningPanel.SetActive(false);
            invalidTilesWarningPanel.SetActive(false);

            SetButtonActionListener();
            LoadMap(); // Load the map to be edited
        }

        /**
         * Saves the map data to .json file.
         */
        private void SaveMapToFile() {
            WallData wallData = WallEditor.Instance.GetWallData();
            PropData propData = PropEditor.Instance.GetPropData();
            DifficultyType difficulty = DifficultyEditor.Instance.GetDifficultyData();
            bool eventEnabled = EventEditor.Instance.GetEventStatusData();

            Map map = new(_mapName, difficulty, eventEnabled, wallData, propData);

            // Get the number of total Ghostrons (will be part of the file name)
            int totalGhostrons = propData.TotalPropCounts[PropType.Ghostron];

            // Find the origin file to delete it
            string originMapFile = _saveDirectory + "/" + PlayerPrefs.GetString("EditMapFileToLoad") + ".json";
            if (!File.Exists(originMapFile)) {
                Debug.LogError("Map Editor save error: Origin map file not found!");
            }

            // Delete the origin file
            File.Delete(originMapFile);

            // Save to a new file in .json
            string json = JsonUtility.ToJson(new MapJsonWrapper(map), true);
            File.WriteAllText(
                Path.Combine(_saveDirectory, _mapName + "_" + totalGhostrons + "_" + (int)difficulty + ".json"),
                json);
        }

        /**
         * Loads the map data from the file. Used when the player enters the map editor.
         */
        private void LoadMap() {
            // Read from player preferences
            string mapFileName = PlayerPrefs.GetString("EditMapFileToLoad", "");

            Match match = _regex.Match(mapFileName);

            if (!match.Success) {
                Debug.LogError("File match error when loading map file! File name: " + mapFileName);
                return;
            }

            // Set map name & difficulty info according to map name
            _mapName = match.Groups[1].Value;
            DifficultyEditor.Instance.SetDifficultyData(Enum.Parse<DifficultyType>(match.Groups[3].Value));

            // Obtain the file path
            string path = Path.Combine(_saveDirectory, mapFileName + ".json");

            // Read the file
            if (!string.IsNullOrEmpty(mapFileName) && File.Exists(path)) {
                string json = File.ReadAllText(path);
                MapJsonWrapper wrapper = JsonConvert.DeserializeObject<MapJsonWrapper>(json);

                // Initialise UI (set names etc.)
                InitUI(_mapName);

                // Set walls
                WallEditor.Instance.SetWallData(new WallData(wrapper.HorizontalWallStatus, wrapper.VerticalWallStatus));

                // Set props
                PlayerPrefs.SetString("GameObjectReadMode", "EDITOR");
                PropEditor.Instance.SetPropData(new PropData(wrapper.PropPositions(),
                    wrapper.FixedPropCounts, wrapper.TotalPropCounts));

                // Set event status
                EventEditor.Instance.SetEventStatusData(wrapper.eventEnabled);
                Debug.Log("All set");
            } else {
                Debug.LogError("Load map error: File not found!");
            }
        }

        // Edit Walls button operation
        private void OnEditWallsButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Update the prompt
            modePromptText.SetText("Editing:\nWalls");

            // Mode setting
            PropEditor.Instance.QuitPropMode(true);
            DifficultyEditor.Instance.QuitDifficultyMode();
            EventEditor.Instance.QuitEventMode();
            WallEditor.Instance.EnterWallMode();

            _mode = 1;

            // Buttons color update
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(difficultyModeButton, false);
            SetButtonStatus(eventModeButton, false);
            SetButtonStatus(wallModeButton, true);

            // Update the setting panel
            propModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
            eventSettingPanel.SetActive(false);
            wallModeSettingPanel.SetActive(true);
        }

        // Edit Props button operation
        private void OnEditPropsButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Update the prompt
            modePromptText.SetText("Editing:\nProps");

            // Mode setting
            WallEditor.Instance.QuitWallMode();
            DifficultyEditor.Instance.QuitDifficultyMode();
            EventEditor.Instance.QuitEventMode();
            PropEditor.Instance.EnterPropMode();

            _mode = 2;

            // Buttons color update
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(difficultyModeButton, false);
            SetButtonStatus(eventModeButton, false);
            SetButtonStatus(propModeButton, true);

            // Update the setting panel
            wallModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
            eventSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(true);
        }

        // Difficulty setting button operation
        private void OnDifficultyButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Update the prompt
            modePromptText.SetText("Editing:\nDifficulty");

            // Mode setting
            PropEditor.Instance.QuitPropMode(true);
            WallEditor.Instance.QuitWallMode();
            EventEditor.Instance.QuitEventMode();
            DifficultyEditor.Instance.EnterDifficultyMode();

            _mode = 3;

            // Buttons color update
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(eventModeButton, false);
            SetButtonStatus(difficultyModeButton, true);

            // Update the setting panel
            wallModeSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(false);
            eventSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(true);
        }

        // Event setting button operation
        private void OnEventButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Update the prompt
            modePromptText.SetText("Editing:\nEvent Status");

            // Mode setting
            PropEditor.Instance.QuitPropMode(true);
            WallEditor.Instance.QuitWallMode();
            DifficultyEditor.Instance.QuitDifficultyMode();
            EventEditor.Instance.EnterEventMode();

            _mode = 4;

            // Buttons color update
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(difficultyModeButton, false);
            SetButtonStatus(eventModeButton, true);

            // Update the setting panel
            wallModeSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
            eventSettingPanel.SetActive(true);
        }

        // Quit (directly) button operation
        private void OnQuitButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            InitUI(_mapName); // Reset this UI as it is being closed

            // Load HomePage UI
            SceneManager.LoadScene("HomePage");
        }

        // Save & Quit button operation
        private void OnSaveAndQuitButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Check if the condition is met
            // 1 - PACBOY SPAWN POINT IS SET
            if (!PropEditor.Instance.CheckCondition()) {
                // If Pacboy spawn point not set
                // Show relevant warning
                noPacboySpawnWarningPanel.SetActive(true);

                // Temporarily disable all modes
                // (Recovered after the close button is clicked)
                WallEditor.Instance.QuitWallMode();
                PropEditor.Instance.QuitPropMode(true);
                DifficultyEditor.Instance.QuitDifficultyMode();
                EventEditor.Instance.QuitEventMode();

                // Temporarily ban all the buttons
                // (Recovered after the close button is clicked)
                quitButton.interactable = false;
                saveButton.interactable = false;
                propModeButton.interactable = false;
                wallModeButton.interactable = false;
                difficultyModeButton.interactable = false;
                eventModeButton.interactable = false;

                return;
            }

            // 2 - ALL TILES ARE ACCESSIBLE & DISTANCE <= 22 FROM CENTER
            bool tilesValid = TileChecker.Instance.CheckTileLegality(WallEditor.Instance.GetWallData());

            if (!tilesValid) {
                // If invalid tiles exist
                // Play warning sound
                SoundManager.Instance.PlaySoundOnce(SoundType.Warning);
                
                // Show invalid tiles warning
                invalidTilesWarningPanel.SetActive(true);
                
                // Temporarily disable all modes
                // (Automatically go to Wall Mode after the close button is clicked)
                WallEditor.Instance.QuitWallMode();
                PropEditor.Instance.QuitPropMode(false); // Note: the param should be false here
                DifficultyEditor.Instance.QuitDifficultyMode();
                EventEditor.Instance.QuitEventMode();

                // Temporarily ban all the buttons
                // (Recovered after the close button is clicked)
                quitButton.interactable = false;
                saveButton.interactable = false;
                propModeButton.interactable = false;
                wallModeButton.interactable = false;
                difficultyModeButton.interactable = false;
                eventModeButton.interactable = false;
                return;
            }

            InitUI(_mapName); // Reset this UI as it is being closed

            // Save map logic
            SaveMapToFile();

            // Load HomePage UI
            PlayerPrefs.SetInt("MainPageAt", 2);
            SceneManager.LoadScene("HomePage");
        }

        // Operations after the close button of the No Pacboy warning panel is clicked
        private void OnNoPacboyPanelCloseButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            noPacboySpawnWarningPanel.SetActive(false);

            // Enable all the buttons
            quitButton.interactable = true;
            saveButton.interactable = true;
            propModeButton.interactable = true;
            wallModeButton.interactable = true;
            difficultyModeButton.interactable = true;
            eventModeButton.interactable = true;

            switch (_mode) {
                case 0:
                    // In none of the modes
                    WallEditor.Instance.QuitWallMode();
                    PropEditor.Instance.QuitPropMode(true);
                    DifficultyEditor.Instance.QuitDifficultyMode();
                    EventEditor.Instance.QuitEventMode();
                    break;
                case 1:
                    // In walls mode
                    WallEditor.Instance.EnterWallMode();
                    PropEditor.Instance.QuitPropMode(true);
                    DifficultyEditor.Instance.QuitDifficultyMode();
                    EventEditor.Instance.QuitEventMode();
                    break;
                case 2:
                    // In props mode
                    WallEditor.Instance.QuitWallMode();
                    PropEditor.Instance.EnterPropMode();
                    DifficultyEditor.Instance.QuitDifficultyMode();
                    EventEditor.Instance.QuitEventMode();
                    break;
                case 3:
                    // In difficulty setting mode
                    WallEditor.Instance.QuitWallMode();
                    PropEditor.Instance.QuitPropMode(true);
                    DifficultyEditor.Instance.EnterDifficultyMode();
                    EventEditor.Instance.QuitEventMode();
                    break;
                case 4:
                    // In event setting mode
                    WallEditor.Instance.QuitWallMode();
                    PropEditor.Instance.QuitPropMode(true);
                    DifficultyEditor.Instance.QuitDifficultyMode();
                    EventEditor.Instance.EnterEventMode();
                    break;
                default:
                    Debug.LogError("Mode error!");
                    return;
            }
        }
        
        // Operations after the close button of the No Pacboy warning panel is clicked
        private void OnInvalidTilesPanelCloseButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            invalidTilesWarningPanel.SetActive(false);

            // Enable all the buttons
            quitButton.interactable = true;
            saveButton.interactable = true;
            propModeButton.interactable = true;
            wallModeButton.interactable = true;
            difficultyModeButton.interactable = true;
            eventModeButton.interactable = true;
            
            // Set invalid tiles display status
            TileChecker.Instance.invalidTilesDisplaying = true;

            // Automatically go to Edit Walls mode
            // Update the prompt
            modePromptText.SetText("Editing:\nWalls");

            // Mode setting
            PropEditor.Instance.QuitPropMode(false); // Note: the param should be false here
            DifficultyEditor.Instance.QuitDifficultyMode();
            EventEditor.Instance.QuitEventMode();
            WallEditor.Instance.EnterWallMode();

            _mode = 1;

            // Buttons color update
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(difficultyModeButton, false);
            SetButtonStatus(eventModeButton, false);
            SetButtonStatus(wallModeButton, true);

            // Update the setting panel
            propModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
            eventSettingPanel.SetActive(false);
            wallModeSettingPanel.SetActive(true);
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

        // Set the action listeners of all the buttons
        private void SetButtonActionListener() {
            quitButton.onClick.AddListener(OnQuitButtonClick);
            saveButton.onClick.AddListener(OnSaveAndQuitButtonClick);
            wallModeButton.onClick.AddListener(OnEditWallsButtonClick);
            propModeButton.onClick.AddListener(OnEditPropsButtonClick);
            difficultyModeButton.onClick.AddListener(OnDifficultyButtonClick);
            eventModeButton.onClick.AddListener(OnEventButtonClick);
            noPacboyWarningPanelCloseButton.onClick.AddListener(OnNoPacboyPanelCloseButtonClick);
            invalidTilesPanelCloseButton.onClick.AddListener(OnInvalidTilesPanelCloseButtonClick);
        }

        // Initialises the map editor UI. 
        private void InitUI(string mapName) {
            // Map Name setting
            _mapName = mapName;
            mapNameText.text = mapName;

            // Reset the prompt
            modePromptText.SetText("Click to select what you need to edit!");

            // Clear of all the modes
            WallEditor.Instance.QuitWallMode();
            PropEditor.Instance.QuitPropMode(true);
            DifficultyEditor.Instance.QuitDifficultyMode();
            EventEditor.Instance.QuitEventMode();

            // Mode index reset (in none of the modes)
            _mode = 0;

            // Reset the color of both buttons
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(difficultyModeButton, false);
            SetButtonStatus(eventModeButton, false);

            // Reset both setting panels
            wallModeSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
            eventSettingPanel.SetActive(false);
        }
    }
}