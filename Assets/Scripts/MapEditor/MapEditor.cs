using System.IO;
using System.Text.RegularExpressions;
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
        public Button difficultyModeButton; // Difficulty setting mode

        // UI panels
        public GameObject wallModeSettingPanel;
        public GameObject propModeSettingPanel;
        public GameObject difficultySettingPanel;

        public TMP_Text mapNameText;
        public TMP_Text modePromptText;

        public GameObject noPacmanSpawnWarningPanel;
        public Button warningPanelCloseButton;

        // Current edit mode
        // 0 - Disabled/Default
        // 1 - Walls
        // 2 - Props
        // 3 - Difficulty
        private int _mode;
        private string _mapName;

        // Map data save directory
        private string _saveDirectory = Path.Combine(Application.dataPath, "Maps");
        private Regex _regex = new(@"^([^_]+)_(\d+)_([A-Za-z])");

        // Prompt Text
        // Default: Click to select what you need to edit!
        // Wall Mode: Editing Walls
        // Prop Mode: Editing Props
        // Difficulty Mode: Editing Difficulty

        private void Start() {
            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);

            SetButtonActionListener();
            InitUI("DEBUG TEST");

            // TEST TEST TEST TODO integrate
            PlayerPrefs.SetString("EditMapFileToLoad", "CLASSIC_2_E");
            LoadMap();
        }

        /**
         * Saves the map data to .json file.
         */
        private bool SaveMapToFile() {
            WallData wallData = gameObject.GetComponent<WallEditor>().GetWallData();
            PropData propData = gameObject.GetComponent<PropEditor>().GetPropData();
            char difficulty = gameObject.GetComponent<DifficultyEditor>().GetDifficultyData();

            Map map = new(_mapName, difficulty, wallData, propData);

            // Get the number of total ghosts (will be part of the file name)
            int totalGhosts = propData.TotalPropCounts["GhostSpawn"];

            // Save to file in .json
            string json = JsonUtility.ToJson(new MapJsonWrapper(map), true);
            File.WriteAllText(Path.Combine(_saveDirectory, _mapName + "_" + totalGhosts + "_" + difficulty + ".json"),
                json);
            Debug.Log("Map saved successfully: " + _mapName + ", location: " + _saveDirectory);

            return true;
        }

        /**
         * Loads the map data from the file. Used when the player enters the map editor.
         */
        private bool LoadMap() {
            // Read from player preferences
            string mapFileName = PlayerPrefs.GetString("EditMapFileToLoad", "");

            Match match = _regex.Match(mapFileName);

            if (!match.Success) {
                Debug.LogError("File match error when loading map file! File name: " + mapFileName);
                return false;
            }

            // Set map name and difficulty
            _mapName = match.Groups[1].Value;
            gameObject.GetComponent<DifficultyEditor>().SetDifficultyData(match.Groups[3].Value[0]);

            // Obtain the file path
            string path = Path.Combine(_saveDirectory, mapFileName + ".json");

            // Read the file
            if (!string.IsNullOrEmpty(mapFileName) && File.Exists(path)) {
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
            modePromptText.SetText("Editing:\nWalls");

            // Mode setting
            gameObject.GetComponent<PropEditor>().QuitPropMode();
            gameObject.GetComponent<DifficultyEditor>().QuitDifficultyMode();
            gameObject.GetComponent<WallEditor>().EnterWallMode();

            _mode = 1;

            // Buttons color update
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(difficultyModeButton, false);
            SetButtonStatus(wallModeButton, true);

            // Update the setting panel
            propModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
            wallModeSettingPanel.SetActive(true);
        }

        // Edit Props button operation
        private void OnEditPropsButtonClick() {
            // Update the prompt
            modePromptText.SetText("Editing:\nProps");

            // Mode setting
            gameObject.GetComponent<WallEditor>().QuitWallMode();
            gameObject.GetComponent<DifficultyEditor>().QuitDifficultyMode();
            gameObject.GetComponent<PropEditor>().EnterPropMode();

            _mode = 2;

            // Buttons color update
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(difficultyModeButton, false);
            SetButtonStatus(propModeButton, true);

            // Update the setting panel
            wallModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(true);
        }

        // Difficulty setting button operation
        private void OnDifficultyButtonClick() {
            // Update the prompt
            modePromptText.SetText("Editing:\nDifficulty");

            // Mode setting
            gameObject.GetComponent<PropEditor>().QuitPropMode();
            gameObject.GetComponent<WallEditor>().QuitWallMode();
            gameObject.GetComponent<DifficultyEditor>().EnterDifficultyMode();

            _mode = 3;

            // Buttons color update
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(difficultyModeButton, true);

            // Update the setting panel
            wallModeSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(true);
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
                gameObject.GetComponent<DifficultyEditor>().QuitDifficultyMode();

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
                    // In none of the modes
                    gameObject.GetComponent<WallEditor>().QuitWallMode();
                    gameObject.GetComponent<PropEditor>().QuitPropMode();
                    gameObject.GetComponent<DifficultyEditor>().QuitDifficultyMode();
                    break;
                case 1:
                    // In walls mode
                    gameObject.GetComponent<WallEditor>().EnterWallMode();
                    gameObject.GetComponent<PropEditor>().QuitPropMode();
                    gameObject.GetComponent<DifficultyEditor>().QuitDifficultyMode();
                    break;
                case 2:
                    // In props mode
                    gameObject.GetComponent<WallEditor>().QuitWallMode();
                    gameObject.GetComponent<PropEditor>().EnterPropMode();
                    gameObject.GetComponent<DifficultyEditor>().QuitDifficultyMode();
                    break;
                case 3:
                    // In difficulty setting mode
                    gameObject.GetComponent<WallEditor>().QuitWallMode();
                    gameObject.GetComponent<PropEditor>().QuitPropMode();
                    gameObject.GetComponent<DifficultyEditor>().EnterDifficultyMode();
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

        // Set the action listeners of all the buttons
        private void SetButtonActionListener() {
            quitButton.onClick.AddListener(OnQuitButtonClick);
            saveButton.onClick.AddListener(OnSaveAndQuitButtonClick);
            wallModeButton.onClick.AddListener(OnEditWallsButtonClick);
            propModeButton.onClick.AddListener(OnEditPropsButtonClick);
            difficultyModeButton.onClick.AddListener(OnDifficultyButtonClick);
            warningPanelCloseButton.onClick.AddListener(OnWarningPanelCloseButtonClick);
        }

        // Initializes the map editor UI. 
        private void InitUI(string mapName) {
            // Map Name setting
            _mapName = mapName;
            mapNameText.text = mapName;

            // Reset the prompt
            modePromptText.SetText("Click to select what you need to edit!");

            // Clear of all the modes
            gameObject.GetComponent<WallEditor>().QuitWallMode();
            gameObject.GetComponent<PropEditor>().QuitPropMode();
            gameObject.GetComponent<DifficultyEditor>().QuitDifficultyMode();
            
            // Mode index reset (in none of the modes)
            _mode = 0;

            // Reset the color of both buttons
            SetButtonStatus(wallModeButton, false);
            SetButtonStatus(propModeButton, false);
            SetButtonStatus(difficultyModeButton, false);

            // Reset both setting panels
            wallModeSettingPanel.SetActive(false);
            propModeSettingPanel.SetActive(false);
            difficultySettingPanel.SetActive(false);
        }
    }
}