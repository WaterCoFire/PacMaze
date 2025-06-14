﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Entity.Map;
using Sound;
using TMPro;
using UnityEngine.SceneManagement;

namespace HomePage {
    /**
     * Manages the Play Maps page.
     */
    public class PlayMapPage : MonoBehaviour {
        // Scroll Rect for displaying all map infos
        public ScrollRect playMapScrollRect;
        public GameObject mapInfoPrefab; // Prefab for each map info

        // Buttons at the top
        public Button backButton;

        // This play map page
        public GameObject playMapPage;

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
        
        // Map difficulty text color
        private readonly Color _easyTextColor = new Color(65f / 255f, 255f / 255f, 0f / 255f);
        private readonly Color _normalTextColor = new Color(255f / 255f, 255f / 255f, 0f / 255f);
        private readonly Color _hardTextColor = new Color(255f / 255f, 37f / 255f, 0f / 255f);

        // START FUNCTION
        private void Start() {
            SetButtonActionListener();

            _mapInfos = new List<MapInfo>();
            UpdatePlayMapList();
        }

        // Sets the button action listeners
        private void SetButtonActionListener() {
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        private void UpdatePlayMapList() {
            // Empty the whole display list first
            foreach (Transform child in playMapScrollRect.content.transform) {
                Destroy(child.gameObject);
            }

            playMapScrollRect.verticalNormalizedPosition = 0f;

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
                GameObject mapInfoObject = Instantiate(mapInfoPrefab, playMapScrollRect.content);
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

                    if (objName == "PlayButton") {
                        // PLAY THIS MAP
                        // Go to the map scene
                        button.onClick.AddListener(() => {
                            // Play click sound
                            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

                            // Get the map file name
                            // And set the play reference
                            PlayerPrefs.SetString("PlayMapFileToLoad",
                                mapInfo.Name + "_" + mapInfo.GhostronNum + "_" + (int)mapInfo.Difficulty);

                            // Load the map scene
                            SceneManager.LoadScene("PlayMap");
                        });
                    }
                }
            }
        }

        /* Button action listeners setting */
        // Back button: back to home page
        private void OnBackButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Set the UI location information
            PlayerPrefs.SetInt("MainPageAt", 0);

            // Disable this page
            playMapPage.SetActive(false);

            // Enable the home page
            homePage.SetActive(true);
        }
    }
}