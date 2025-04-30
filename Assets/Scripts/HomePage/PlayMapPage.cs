using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.SceneManagement;

namespace HomePage {
    public class PlayMapPage : MonoBehaviour {
        public ScrollRect playMapScrollRect;

        public GameObject mapInfoPrefab;
        public Transform scrollRectContent;

        // Buttons at the top
        public Button backButton;

        // This play map page
        public GameObject playMapPage;

        // Home page
        public GameObject homePage;

        // Prompt for no map
        public GameObject noMapPrompt;

        private List<MapInfo> _mapInfos;

        private float _cumulativeHeight;
        private readonly float _prefabHeight = 170f;
        private readonly float _padding = 1f;

        private readonly string _saveDirectory = Path.Combine(Application.dataPath, "Data", "Maps");
        private readonly Regex _regex = new(@"^([^_]+)_(\d+)_([A-Za-z])\.json$");

        // Map difficulty text color
        private readonly Color _easyTextColor = new Color(65f / 255f, 255f / 255f, 0f / 255f);
        private readonly Color _normalTextColor = new Color(255f / 255f, 255f / 255f, 0f / 255f);
        private readonly Color _hardTextColor = new Color(255f / 255f, 37f / 255f, 0f / 255f);

        private void Start() {
            Debug.Log("PlayMapPage START");

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
            // MAP NAME + GHOSTRONS (NUM) + DIFFICULTY (LETTER)

            // Read all file names
            foreach (var filePath in files) {
                string fileName = Path.GetFileName(filePath);
                Match match = _regex.Match(fileName);

                // File name matched
                if (match.Success) {
                    string mapName = match.Groups[1].Value;
                    int ghostrons = int.Parse(match.Groups[2].Value);
                    char difficulty = match.Groups[3].Value[0];

                    MapInfo mapInfo = new MapInfo(mapName, ghostrons, difficulty);
                    Debug.Log("Play Matched: Map info " + mapName + ", " + ghostrons + ", " + difficulty);
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
                GameObject mapInfoObject = Instantiate(mapInfoPrefab, scrollRectContent);
                RectTransform itemTransform = mapInfoObject.GetComponent<RectTransform>();
                itemTransform.anchoredPosition = new Vector2(0f, -_cumulativeHeight);
                _cumulativeHeight += _prefabHeight + _padding;

                // Find the map name TMP_Text and the ghostron number TMP_Text in the prefab
                TMP_Text[] objectTexts = mapInfoObject.GetComponentsInChildren<TMP_Text>();
                foreach (var text in objectTexts) {
                    string objName = text.gameObject.name;

                    if (objName == "MapNameText") {
                        // Map name
                        text.text = mapInfo.Name;
                    } else if (objName == "TotalGhostronNumText") {
                        // Number of ghostrons
                        text.text = mapInfo.GhostronNum.ToString();
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
                                return;
                        }
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
                            Debug.Log("Play clicked!");
                            
                            // Get the map file name
                            // And set the play reference
                            PlayerPrefs.SetString("PlayMapFileToLoad",
                                mapInfo.Name + "_" + mapInfo.GhostronNum + "_" + mapInfo.Difficulty);
                            
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
            // Set the UI location information
            PlayerPrefs.SetInt("MainPageAt", 0);
            
            // Disable this page
            playMapPage.SetActive(false);

            // Enable the home page
            homePage.SetActive(true);
        }
    }
}