using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.SceneManagement;

namespace MainPage {
    public class EditMapView : MonoBehaviour {
        public ScrollRect editMapScrollRect;
        public GameObject mapInfoPrefab;
        public Transform scrollRectContent;

        // Buttons at the top
        public Button backButton;
        public Button createNewMapButton;
        
        // This edit map page
        public GameObject editMapPage;
        
        // Home page
        public GameObject homePage;

        // Prompt for no map
        public GameObject noMapPrompt;

        private List<MapInfo> _mapInfos;

        private float _cumulativeHeight;
        private float _prefabHeight = 170f;
        private float _padding = 1f;

        private string _saveDirectory = Path.Combine(Application.dataPath, "Maps");

        private MapInfo _renamedMapOldInfo;
        private string _renamedMapNewName;

        // Map difficulty text color
        private Color _easyTextColor = new Color(65f / 255f, 255f / 255f, 0f / 255f);
        private Color _normalTextColor = new Color(255f / 255f, 255f / 255f, 0f / 255f);
        private Color _hardTextColor = new Color(255f / 255f, 37f / 255f, 0f / 255f);

        private void Start() {
            Debug.Log("EditMapView START");

            SetButtonActionListener();

            _mapInfos = new List<MapInfo>();
            UpdateEditMapList();
        }

        // Sets the button action listeners
        private void SetButtonActionListener() {
            backButton.onClick.AddListener(OnBackButtonClick);
            createNewMapButton.onClick.AddListener(OnCreateNewMapButtonClick);
        }

        public bool UpdateEditMapList() {
            // Empty the whole display list first
            foreach (Transform child in editMapScrollRect.content.transform) {
                Destroy(child.gameObject);
            }

            Debug.Log("1 Destroyed");

            // Clear the list
            _mapInfos.Clear();
            Debug.Log("2 Cleared");

            // Reset the cumulative height
            _cumulativeHeight = 0f;

            // Check if the directory exists
            if (!Directory.Exists(_saveDirectory)) {
                Debug.LogError("File location not exist!");
                return false;
            }

            Debug.Log("3 Directory found");

            // Read all the files in the directory
            string[] files = Directory.GetFiles(_saveDirectory, "*.json");
            Debug.Log("4 Files read");

            // The format of the file
            // MAP NAME + GHOSTS (NUM) + DIFFICULTY (LETTER)
            Regex regex = new Regex(@"^([^_]+)_(\d+)_([A-Za-z])\.json$");

            // Read all file names
            foreach (var filePath in files) {
                string fileName = Path.GetFileName(filePath);
                Match match = regex.Match(fileName);

                Debug.Log("5 Files name " + fileName);

                // File name matched
                if (match.Success) {
                    Debug.Log("5.1 Match success " + fileName);
                    string mapName = match.Groups[1].Value;
                    int ghosts = int.Parse(match.Groups[2].Value);
                    char difficulty = match.Groups[3].Value[0];

                    MapInfo mapInfo = new MapInfo(mapName, ghosts, difficulty);
                    Debug.Log("5.1 Map info " + mapName + ", " + ghosts + ", " + difficulty);
                    _mapInfos.Add(mapInfo);
                }
            }

            // Check if there is any map
            if (_mapInfos.Count == 0) {
                // No map: Show prompt
                noMapPrompt.SetActive(true);
                return true;
            }

            // There exists at least one map
            noMapPrompt.SetActive(false);

            // UI update
            foreach (var mapInfo in _mapInfos) {
                GameObject mapInfoObject = Instantiate(mapInfoPrefab, scrollRectContent);
                RectTransform itemTransform = mapInfoObject.GetComponent<RectTransform>();
                itemTransform.anchoredPosition = new Vector2(0f, -_cumulativeHeight);
                _cumulativeHeight += _prefabHeight + _padding;

                // Find the map name TMP_Text and the ghost number TMP_Text in the prefab
                TMP_Text[] objectTexts = mapInfoObject.GetComponentsInChildren<TMP_Text>();
                foreach (var text in objectTexts) {
                    string objName = text.gameObject.name;

                    if (objName == "MapNameText") {
                        // Map name
                        text.text = mapInfo.Name;
                    } else if (objName == "TotalGhostNumText") {
                        // Number of ghosts
                        text.text = mapInfo.GhostNum.ToString();
                    } else if (objName == "DifficultyText") {
                        // Map difficulty
                        // Set the text content and color
                        switch (mapInfo.Difficulty) {
                            case 'E':
                                text.text = "EASY";
                                text.color = _easyTextColor;
                                break;
                            case 'N':
                                text.text = "NORMAL";
                                text.color = _normalTextColor;
                                break;
                            case 'H':
                                text.text = "HARD";
                                text.color = _hardTextColor;
                                break;
                            default:
                                Debug.LogError("Difficulty error while reading files");
                                return false;
                        }
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
                            Debug.Log("Delete clicked!");
                            string path = Path.Combine(_saveDirectory,
                                $"{mapInfo.Name}_{mapInfo.GhostNum}_{mapInfo.Difficulty}.json");
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
                            Debug.Log("Rename clicked!");
                            _renamedMapOldInfo = mapInfo;
                            ShowRenameWindow(mapInfo.Name); // Show rename window
                        });
                    } else if (objName == "EditButton") {
                        // EDIT THIS MAP
                        // Enter the map editor interface for this map
                        // Set the player preferences string "EditMapToLoad" to the map name (mapInfo.Name) and switch to MapEditor scene
                        button.onClick.AddListener(() => {
                            Debug.Log("Edit clicked!");
                            PlayerPrefs.SetString("EditMapFileToLoad",
                                mapInfo.Name + "_" + mapInfo.GhostNum + "_" + mapInfo.Difficulty);
                            SceneManager.LoadScene("MapEditor");
                        });
                    }
                }
            }

            return true;
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
                $"{_renamedMapOldInfo.Name}_{_renamedMapOldInfo.GhostNum}_{_renamedMapOldInfo.Difficulty}.json");
            string newPath = Path.Combine(_saveDirectory,
                $"{newName}_{_renamedMapOldInfo.GhostNum}_{_renamedMapOldInfo.Difficulty}.json");

            if (File.Exists(oldPath)) {
                // File operation
                File.Move(oldPath, newPath);
                Debug.Log($"Renamed {oldPath} -> {newPath}");

                UpdateEditMapList(); // Update the data & UI

                // Re-set the map name in the json file
                string json = File.ReadAllText(newPath);

                // Replace the old name with the new one
                string updatedJson = System.Text.RegularExpressions.Regex.Replace(
                    json,
                    "\"name\"\\s*:\\s*\"[^\"]*\"",
                    $"\"name\": \"{newName}\""
                );

                File.WriteAllText(newPath, updatedJson);

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
            // Number of ghosts: 2 Ghosts
            // Difficulty: Easy

            // Default map path
            string defaultMapPath = _saveDirectory + "/Default/DEFAULT_MAP.json";

            // Path of the map to be created
            string targetMapPath = _saveDirectory + "/" + newMapName + "_2_E.json";

            if (!File.Exists(defaultMapPath)) {
                Debug.LogError("Default map file does not exist!");
                return;
            }

            try {
                // Read all the text in the default map file json
                string json = File.ReadAllText(defaultMapPath);

                // Replace the default name with the new one
                string newMapJson = System.Text.RegularExpressions.Regex.Replace(
                    json,
                    "\"name\"\\s*:\\s*\"[^\"]*\"",
                    $"\"name\": \"{newMapName}\""
                );

                // And write it to the new map file
                File.WriteAllText(targetMapPath, newMapJson);
            } catch (IOException e) {
                Debug.LogError($"Create map error: {e.Message}");
                return;
            }

            // Player preference setting: the file name to load
            PlayerPrefs.SetString("EditMapFileToLoad", newMapName + "_2_E");

            // Jump to Map Editor
            SceneManager.LoadScene("MapEditor");
        }

        /* Button action listeners setting */
        // Back button: back to home page
        private void OnBackButtonClick() {
            // Disable this page
            editMapPage.SetActive(false);
            
            // Enable the home page
            homePage.SetActive(true);
        }

        // Create new map button: Enters the create window
        private void OnCreateNewMapButtonClick() {
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