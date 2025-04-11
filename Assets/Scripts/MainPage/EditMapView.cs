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

        private List<MapInfo> _mapInfos;

        private float _cumulativeHeight;
        private float _prefabHeight = 170f;
        private float _padding = 1f;

        private string _saveDirectory = Path.Combine(Application.dataPath, "Maps");

        private MapInfo _renamedMapOldInfo;
        private string _renamedMapNewName;

        private void Start() {
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
                        // TODO set colors
                    } else if (objName == "DifficultyText") {
                        // Map difficulty
                        // TODO set colors
                        switch (mapInfo.Difficulty) {
                            case 'E':
                                text.text = "EASY";
                                break;
                            case 'N':
                                text.text = "NORMAL";
                                break;
                            case 'H':
                                text.text = "HARD";
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
                            string path = Path.Combine(_saveDirectory, $"{mapInfo.Name}_{mapInfo.GhostNum}.json");
                            if (File.Exists(path)) {
                                File.Delete(path);
                            } else {
                                Debug.LogError("Error occurred when trying to delete file!");
                            }

                            Destroy(mapInfoObject);
                            Debug.Log($"Deleted map: {mapInfo.Name}");

                            // UpdateEditMapList(); // Update then TODO figure this out
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

                UpdateEditMapList(); // UI update
                
                // TODO Delete issue

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
            // TODO Create logic

            // Create new map file
            // BY DEFAULT, THE MAP:
            // Number of ghosts: 2 Ghosts
            // Difficulty: Easy


            // Player preference setting: the file name to load
            PlayerPrefs.SetString("EditMapFileToLoad", newMapName + "_2_E");

            // Jump to Map Editor
            SceneManager.LoadScene("MapEditor");
        }

        /* Button action listeners setting */
        // Back button: back to home page
        private void OnBackButtonClick() {
            // TODO
        }

        // Create new map button: Enters the create window
        private void OnCreateNewMapButtonClick() {
            gameObject.GetComponent<EditMapCreateWindow>().ShowCreateWindow();
        }
    }
}