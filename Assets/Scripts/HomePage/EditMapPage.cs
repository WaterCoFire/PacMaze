using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Entity.Map;
using Entity.Map.Utility;
using Sound;
using TMPro;
using UnityEngine.SceneManagement;

namespace HomePage {
    /**
     * Manages the "Edit Maps" page.
     */
    public class EditMapPage : MonoBehaviour {
        public ScrollRect editMapScrollRect;
        public GameObject mapInfoPrefab;

        // Buttons at the top
        public Button backButton;
        public Button createNewMapButton;

        // This edit map page
        public GameObject editMapPage;

        // Home page
        public GameObject homePage;

        // Prompt for no map
        public GameObject noMapPrompt;
        
        // All map infos read
        private List<MapInfo> _mapInfos;

        // Map data directory, and regex for matching file names
        private readonly string _saveDirectory = Path.Combine(Application.dataPath, "Data", "Maps");
        private readonly Regex _regex = new(@"^([^_]+)_(\d+)_(\d+)\.json$");
        
        /* UI Params */
        private float _cumulativeHeight;
        private readonly float _prefabHeight = 170f;
        private readonly float _padding = 1f;
        
        private MapInfo _renamedMapOldInfo;
        private string _renamedMapNewName;

        // Map difficulty text color
        private readonly Color _easyTextColor = new Color(65f / 255f, 255f / 255f, 0f / 255f);
        private readonly Color _normalTextColor = new Color(255f / 255f, 255f / 255f, 0f / 255f);
        private readonly Color _hardTextColor = new Color(255f / 255f, 37f / 255f, 0f / 255f);

        // START FUNCTION
        private void Start() {
            Debug.Log("EditMapPage START");

            SetButtonActionListener();

            _mapInfos = new List<MapInfo>();
            UpdateEditMapList();
        }

        // Sets the button action listeners
        private void SetButtonActionListener() {
            backButton.onClick.AddListener(OnBackButtonClick);
            createNewMapButton.onClick.AddListener(OnCreateNewMapButtonClick);
        }

        private void UpdateEditMapList() {
            // Empty the whole display list first
            foreach (Transform child in editMapScrollRect.content.transform) {
                Destroy(child.gameObject);
            }

            editMapScrollRect.verticalNormalizedPosition = 0f;

            // Clear the list
            _mapInfos.Clear();

            // Reset the cumulative height
            _cumulativeHeight = 0f;

            // Check if the directory exists
            if (!Directory.Exists(_saveDirectory)) {
                Debug.LogError("File location not exist!");
                return;
            }

            // Read all the files in the directory
            string[] files = Directory.GetFiles(_saveDirectory, "*.json");

            // The format of the file
            // MAP NAME + GHOSTRONS (NUM) + DIFFICULTY (NUM)

            // Read all file names
            foreach (var filePath in files) {
                string fileName = Path.GetFileName(filePath);
                Match match = _regex.Match(fileName);

                // File name matched
                if (match.Success) {
                    string mapName = match.Groups[1].Value;
                    int ghostrons = int.Parse(match.Groups[2].Value);
                    DifficultyType difficulty = Enum.Parse<DifficultyType>(match.Groups[3].Value);

                    MapInfo mapInfo = new MapInfo(mapName, ghostrons, difficulty);
                    Debug.Log("Edit Matched: Map info " + mapName + ", " + ghostrons + ", " + difficulty);
                    _mapInfos.Add(mapInfo);
                }
            }

            // Check if there is any map
            if (_mapInfos.Count == 0) {
                // No map: Show prompt
                noMapPrompt.SetActive(true);
                return;
            }

            // There exists at least one map
            noMapPrompt.SetActive(false);

            // UI update
            foreach (var mapInfo in _mapInfos) {
                GameObject mapInfoObject = Instantiate(mapInfoPrefab, editMapScrollRect.content);
                RectTransform itemTransform = mapInfoObject.GetComponent<RectTransform>();
                itemTransform.anchoredPosition = new Vector2(0f, -_cumulativeHeight);
                _cumulativeHeight += _prefabHeight + _padding;

                // Find the map name TMP_Text and the Ghostron number TMP_Text in the prefab
                TMP_Text[] objectTexts = mapInfoObject.GetComponentsInChildren<TMP_Text>();
                foreach (var text in objectTexts) {
                    string objName = text.gameObject.name;

                    if (objName == "MapNameText") {
                        // Map name
                        text.text = mapInfo.Name;
                    } else if (objName == "TotalGhostronNumText") {
                        // Number of Ghostrons
                        text.text = mapInfo.GhostronNum.ToString();
                    } else if (objName == "DifficultyText") {
                        // Map difficulty
                        // Set the text content and color
                        switch (mapInfo.Difficulty) {
                            case DifficultyType.Easy:
                                text.color = _easyTextColor;
                                break;
                            case DifficultyType.Normal:
                                text.color = _normalTextColor;
                                break;
                            case DifficultyType.Hard:
                                text.color = _hardTextColor;
                                break;
                            default:
                                Debug.LogError("Difficulty error while reading files");
                                return;
                        }

                        text.text = mapInfo.Difficulty.ToString();
                    }
                }

                // Set button operations
                Button[] buttons = mapInfoObject.GetComponentsInChildren<Button>();
                foreach (var button in buttons) {
                    string objName = button.gameObject.name;

                    if (objName == "DeleteButton") {
                        // DELETE THIS MAP OPERATION
                        // Delete this map (.json file)
                        button.onClick.AddListener(() => {
                            // Play click sound
                            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

                            // Get the file name and delete this map file
                            string path = Path.Combine(_saveDirectory,
                                $"{mapInfo.Name}_{mapInfo.GhostronNum}_{(int)mapInfo.Difficulty}.json");
                            if (File.Exists(path)) {
                                File.Delete(path);
                            } else {
                                Debug.LogError("Error occurred when trying to delete file!");
                            }

                            Destroy(mapInfoObject);
                            Debug.Log($"Deleted map: {mapInfo.Name}");

                            UpdateEditMapList(); // Update then
                        });
                    } else if (objName == "RenameButton") {
                        // RENAME THIS MAP OPERATION
                        button.onClick.AddListener(() => {
                            // Play click sound
                            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

                            _renamedMapOldInfo = mapInfo;
                            ShowRenameWindow(mapInfo.Name); // Show rename map name window
                        });
                    } else if (objName == "EditButton") {
                        // EDIT THIS MAP
                        // Enter the map editor interface for this map
                        // Set the player preferences string "EditMapToLoad" to the map name (mapInfo.Name) and switch to MapEditor scene
                        button.onClick.AddListener(() => {
                            // Play click sound
                            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

                            // Go to Map Editor scene
                            PlayerPrefs.SetString("EditMapFileToLoad",
                                mapInfo.Name + "_" + mapInfo.GhostronNum + "_" + (int)mapInfo.Difficulty);
                            SceneManager.LoadScene("MapEditor");
                        });
                    }
                }
            }
        }

        /**
         * Displays the map rename window.
         */
        private void ShowRenameWindow(string oldName) {
            gameObject.GetComponent<EditMapRenameWindow>().ShowRenameWindow(oldName);
        }

        /**
         * Renames the map.
         */
        public void RenameMap(string newName) {
            // Get file paths
            string oldPath = Path.Combine(_saveDirectory,
                $"{_renamedMapOldInfo.Name}_{_renamedMapOldInfo.GhostronNum}_{(int)_renamedMapOldInfo.Difficulty}.json");
            string newPath = Path.Combine(_saveDirectory,
                $"{newName}_{_renamedMapOldInfo.GhostronNum}_{(int)_renamedMapOldInfo.Difficulty}.json");

            if (File.Exists(oldPath)) {
                // File operation
                File.Move(oldPath, newPath);
                Debug.Log($"Renamed {oldPath} -> {newPath}");

                UpdateEditMapList(); // Update the data & UI

                // Re-set the map name in the json file
                // Decrypt map data and get the JSON
                byte[] encryptedBytes = File.ReadAllBytes(newPath);
                string json = AesHelper.DecryptWithIv(encryptedBytes);

                // Replace the old name with the new one
                string updatedJson = Regex.Replace(
                    json,
                    "\"name\"\\s*:\\s*\"[^\"]*\"",
                    $"\"name\": \"{newName}\""
                );

                // Encrypt data and update the file
                byte[] bytes = AesHelper.EncryptWithRandomIv(updatedJson);

                // Write data to a new file
                File.WriteAllBytes(newPath, bytes);

                Debug.Log($"Successfully updated 'name' field in {Path.GetFileName(newPath)} to '{newName}'");
            } else {
                Debug.LogError("Rename error: Old file no longer exists!");
            }
        }

        /**
         * Creates a new map.
         * Called by the confirm button in create window.
         */
        public void CreateMap(string newMapName) {
            // Create new map file
            // BY DEFAULT, THE MAP:
            // Number of Ghostrons: 5 Ghostrons
            // Difficulty: Easy
            string mapFileName = newMapName + "_5_0";

            // Default map path
            string defaultMapPath = _saveDirectory + "/Default/DEFAULT_MAP.json";

            // Path of the map to be created
            string targetMapPath = _saveDirectory + "/" + mapFileName + ".json";

            if (!File.Exists(defaultMapPath)) {
                Debug.LogError("Default map file does not exist!");
                return;
            }

            try {
                // Decrypt the default map file and get the JSON data
                byte[] encryptedBytes = File.ReadAllBytes(defaultMapPath);
                string json = AesHelper.DecryptWithIv(encryptedBytes);

                // Replace the default name with the new one
                string newMapJson = Regex.Replace(
                    json,
                    "\"name\"\\s*:\\s*\"[^\"]*\"",
                    $"\"name\": \"{newMapName}\""
                );

                // Encrypt the JSON data and write it to the new map file
                byte[] newMapEncryptedBytes = AesHelper.EncryptWithRandomIv(newMapJson);
                File.WriteAllBytes(targetMapPath, newMapEncryptedBytes);
            } catch (IOException e) {
                Debug.LogError($"Create map error: {e.Message}");
                return;
            }

            // Player preference setting: the file name to load
            PlayerPrefs.SetString("EditMapFileToLoad", mapFileName);

            // Jump to Map Editor
            SceneManager.LoadScene("MapEditor");
        }

        /* Button action listeners setting */
        // Back button: back to home page
        private void OnBackButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Set the UI location information
            PlayerPrefs.SetInt("MainPageAt", 0);

            // Disable this page
            editMapPage.SetActive(false);

            // Enable the home page
            homePage.SetActive(true);
        }

        // Create new map button: Enters the create window
        private void OnCreateNewMapButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Display create map window
            gameObject.GetComponent<EditMapCreateWindow>().ShowCreateWindow();
        }

        /**
         * Checks if a name is already occupied by an existing map.
         * Used by Map rename/create window when setting the map name.
         * RETURNS:
         * true if conflict happens, false if okay
         */
        public bool CheckNameConflict(string proposedName) {
            // Check every map info
            foreach (var mapInfo in _mapInfos) {
                if (mapInfo.Name == proposedName) {
                    return true;
                }
            }

            // No conflict
            return false;
        }
    }
}