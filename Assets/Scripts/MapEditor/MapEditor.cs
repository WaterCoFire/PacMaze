using System;
using System.Collections.Generic;
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
        /* Buttons */
        public Button quitButton; // Directly quit, no saving
        public Button saveButton; // Save map and quit
        public Button helpButton; // Open help TODO help

        public Button wallModeButton; // Editing wall mode
        public Button propModeButton; // Editing prop mode
        public Button difficultyModeButton; // Difficulty setting mode
        public Button eventModeButton; // Event status setting mode

        // Map name
        public TMP_Text mapNameText;

        // UI panels
        public GameObject wallModeSettingPanel;
        public GameObject propModeSettingPanel;
        public GameObject difficultySettingPanel;
        public GameObject eventSettingPanel;

        // Button Colours
        // Normal color 8C79FF
        // Selected color E4FF49
        private readonly Color _normalColor = new(140f / 255f, 121f / 255f, 255f / 255f);
        private readonly Color _selectedColor = new(228f / 255f, 255f / 255f, 73f / 255f);

        // Prompt Text (Tells player what mode it currently is, e.g., "Editing: Walls")
        public TMP_Text modePromptText;

        /* Warning UI */
        // Warning panel: Pacboy spawn point not set
        public GameObject noPacboySpawnWarningPanel;
        public Button noPacboyWarningPanelCloseButton;

        // Warning panel: Invalid tiles exist
        public GameObject invalidTilesWarningPanel;
        public Button invalidTilesPanelCloseButton;

        // Current edit mode
        // Disabled/Default, Wall, Prop, Difficulty, Event
        private MapEditorMode _mode;
        private string _mapName;

        // Map data save directory
        private readonly string _saveDirectory = Path.Combine(Application.dataPath, "Data", "Maps");
        private readonly Regex _regex = new(@"^([^_]+)_(\d+)_(\d+)");
        
        // Dictionaries mapping Map Editor mode to buttons/setting panels
        private Dictionary<MapEditorMode, Button> _buttons;
        private Dictionary<MapEditorMode, GameObject> _panels;

        // START FUNCTION
        private void Start() {
            Debug.Log("MapEditor START");

            // Play background music
            SoundManager.Instance.PlayBackgroundMusic(false);

            // Create map data directory, if it does not exist
            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
            
            // Initialise dictionaries
            _buttons = new Dictionary<MapEditorMode, Button>();
            _buttons.Add(MapEditorMode.Wall, wallModeButton);
            _buttons.Add(MapEditorMode.Prop, propModeButton);
            _buttons.Add(MapEditorMode.Difficulty, difficultyModeButton);
            _buttons.Add(MapEditorMode.Event, eventModeButton);
            
            _panels = new Dictionary<MapEditorMode, GameObject>();
            _panels.Add(MapEditorMode.Wall, wallModeSettingPanel);
            _panels.Add(MapEditorMode.Prop, propModeSettingPanel);
            _panels.Add(MapEditorMode.Difficulty, difficultySettingPanel);
            _panels.Add(MapEditorMode.Event, eventSettingPanel);

            // Mode initialising
            SwitchMode(MapEditorMode.None, true);

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

            // Mode switching
            SwitchMode(MapEditorMode.Wall, true);
        }

        // Edit Props button operation
        private void OnEditPropsButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Update the prompt
            modePromptText.SetText("Editing:\nProps");

            // Mode switching
            SwitchMode(MapEditorMode.Prop, true);
        }

        // Difficulty setting button operation
        private void OnDifficultyButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Update the prompt
            modePromptText.SetText("Editing:\nDifficulty");

            // Mode switching
            SwitchMode(MapEditorMode.Difficulty, true);
        }

        // Event setting button operation
        private void OnEventButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Update the prompt
            modePromptText.SetText("Editing:\nEvent Status");

            // Mode switching
            SwitchMode(MapEditorMode.Event, true);
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
            // Check if the condition is met
            // 1 - PACBOY SPAWN POINT IS SET
            if (!PropEditor.Instance.CheckCondition()) {
                // If Pacboy spawn point not set
                // Play warning sound
                SoundManager.Instance.PlaySoundOnce(SoundType.Warning);

                // Show relevant warning
                noPacboySpawnWarningPanel.SetActive(true);

                // Temporarily disable all modes
                // (Recovered after the close button is clicked)
                SwitchMode(MapEditorMode.None, true);

                // Temporarily ban all the buttons
                // (Recovered after the close button is clicked)
                SetButtonsAvailability(false);

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
                SwitchMode(MapEditorMode.None, false); // Do not reset the selected tile

                // Temporarily ban all the buttons
                // (Recovered after the close button is clicked)
                SetButtonsAvailability(false);
                return;
            }

            // The map is valid
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

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
            SetButtonsAvailability(true);
            
            // Enter the previous mode
            SwitchMode(_mode, true);
        }

        // Operations after the close button of the No Pacboy warning panel is clicked
        private void OnInvalidTilesPanelCloseButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            invalidTilesWarningPanel.SetActive(false);

            // Enable all the buttons
            SetButtonsAvailability(true);

            // Set invalid tiles display status
            TileChecker.Instance.invalidTilesDisplaying = true;

            // Automatically go to Edit Walls mode
            // Update the prompt
            modePromptText.SetText("Editing:\nWalls");
            
            // Mode switching
            SwitchMode(MapEditorMode.Wall, false); // Do not reset the selected tile
        }
        
        /**
         * Switch to any Map Editor mode, including default (initialise).
         */
        private void SwitchMode(MapEditorMode mode, bool resetSelectedTile) {
            _mode = mode;

            if (mode == MapEditorMode.None) {
                // Default mode - initialise all UIs
                // Reset buttons
                SetButtonStatus(wallModeButton, false);
                SetButtonStatus(propModeButton, false);
                SetButtonStatus(difficultyModeButton, false);
                SetButtonStatus(eventModeButton, false);

                // Quit all modes
                WallEditor.Instance.SetWallMode(false);
                PropEditor.Instance.SetPropMode(false, resetSelectedTile);
                DifficultyEditor.Instance.SetDifficultyMode(false);
                EventEditor.Instance.SetEventMode(false);

                // Quit all setting panels
                wallModeSettingPanel.SetActive(false);
                propModeSettingPanel.SetActive(false);
                difficultySettingPanel.SetActive(false);
                eventSettingPanel.SetActive(false);
            } else {
                // Enter a specific mode
                // Button status setting
                foreach (var buttonKvp in _buttons) {
                    SetButtonStatus(buttonKvp.Value, buttonKvp.Key == mode);
                }
                
                // Mode setting
                WallEditor.Instance.SetWallMode(mode == MapEditorMode.Wall);
                PropEditor.Instance.SetPropMode(mode == MapEditorMode.Prop, resetSelectedTile);
                DifficultyEditor.Instance.SetDifficultyMode(mode == MapEditorMode.Difficulty);
                EventEditor.Instance.SetEventMode(mode == MapEditorMode.Event);
                
                // Panel setting
                foreach (var panelKvp in _panels) {
                    panelKvp.Value.SetActive(panelKvp.Key == mode);
                }
            }
        }

        /**
         * Set the status (selected colour / normal colour) of a button
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
         * Initialises the map editor UI.
         */ 
        private void InitUI(string mapName) {
            // Map Name setting
            _mapName = mapName;
            mapNameText.text = mapName;

            // Reset the prompt
            modePromptText.SetText("Click to select what you need to edit!");

            // Go to default mode
            SwitchMode(MapEditorMode.None, true);
        }
        

        /**
         * Set the action listeners of all the buttons.
         */
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

        /**
         * Enables/Disables all buttons.
         * Used when a panel with higher level is opened/closed.
         */
        private void SetButtonsAvailability(bool setEnabled) {
            quitButton.interactable = setEnabled;
            saveButton.interactable = setEnabled;
            propModeButton.interactable = setEnabled;
            wallModeButton.interactable = setEnabled;
            difficultyModeButton.interactable = setEnabled;
            eventModeButton.interactable = setEnabled;
        }
    }
}