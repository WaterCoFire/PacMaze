using System;
using System.IO;
using System.Text.RegularExpressions;
using Entity.Map;
using Entity.Pacman;
using Newtonsoft.Json;
using PlayMap.UI;
using Setting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayMap {
    /**
     * Manages the map that the player plays in (controls the pacman).
     */
    public class PlayMapController : MonoBehaviour {
        // Map data save directory
        private string _saveDirectory = Path.Combine(Application.dataPath, "Data", "Maps");
        private Regex _regex = new(@"^([^_]+)_(\d+)_([A-Za-z])");

        private char _difficulty; // Difficulty of the current game

        private bool _gamePlaying; // Status telling if the game is currently in progress or not

        // GAME TIMER
        private float _gameTimer;

        // In-game UI
        public GameObject pausePage; // Pause page
        public GameObject winPage; // Game over page: player wins
        public GameObject losePage; // Game over page: player loses

        // Singleton instance
        public static PlayMapController Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("PlayMapController AWAKE");
            // Set singleton instance
            Instance = this;
        }

        /**
         * THE START FUNCTION
         * Get the file name of the map to be played
         * and call InitMap() to initialize the map
         */
        private void Start() {
            Debug.Log("MapController START");

            string mapFileName = PlayerPrefs.GetString("PlayMapFileToLoad", null);
            if (mapFileName == null) {
                Debug.LogError("Play map: File name not properly set!");
                return;
            }

            InitMap(mapFileName); // Initialize the map
            // UI initialization
            pausePage.SetActive(false);
            winPage.SetActive(false);
            losePage.SetActive(false);
            
            Time.timeScale = 1f; // Reset time scale
            _gameTimer = 0f; // Reset timer
            _gamePlaying = true;
        }

        /**
         * Initializes the map:
         * - Generates the wall according to file data
         * - Places all the fixed props
         * - Randomly places the remaining props (num for each prop: total - fixed)
         * - Places the ghostrons at their fixed spawn point
         * - Randomly generates the remaining ghostrons (num: total - fixed)
         * - Generates dots for all the spare tiles
         * - Generates the pacman
         */
        private bool InitMap(string mapFileName) {
            // Parse the map file name
            Match match = _regex.Match(mapFileName);

            if (!match.Success) {
                Debug.LogError("File match error when loading map file to play! File name: " + mapFileName);
                return false;
            }

            // Obtain the file path
            string path = Path.Combine(_saveDirectory, mapFileName + ".json");
            if (!File.Exists(path)) {
                Debug.LogError("Load map to play error: File not found!");
                return false;
            }

            // Read the file
            string json = File.ReadAllText(path);

            // Parse the json data and store in the wrapper
            MapJsonWrapper wrapper = JsonConvert.DeserializeObject<MapJsonWrapper>(json);

            // Difficulty setting
            _difficulty = wrapper.difficulty;

            // Update difficulty in GhostronManager to for setting the behaviours of the ghostrons
            GhostronManager.Instance.SetDifficulty(_difficulty);

            // Generate walls
            gameObject.GetComponent<WallGenerator>()
                .InitWalls(new WallData(wrapper.HorizontalWallStatus, wrapper.VerticalWallStatus));

            // Generate props
            PlayerPrefs.SetInt("GhostronCount", 0);
            PlayerPrefs.SetString("GameObjectReadMode", "PLAY");
            gameObject.GetComponent<PropGenerator>().InitProps(new PropData(wrapper.PropPositions(),
                wrapper.FixedPropCounts, wrapper.TotalPropCounts));

            // Update operation key binding info
            KeyBindingManager.LoadInfoFromFile();

            return true;
        }

        // UPDATE FUNCTION
        private void Update() {
            // No action is game is currently paused
            if (!_gamePlaying) return;

            // Update timer
            _gameTimer += Time.deltaTime;

            // Listening for ESC pause
            if (Input.GetKeyDown(KeyCode.Escape)) {
                PauseGame();
            }
        }


        /**
         * Pauses the game.
         */
        public void PauseGame() {
            GameObject pacman = GameObject.FindGameObjectWithTag("Pacman");
            pacman.GetComponent<PacmanMovement>().DisableMovement();
            pacman.GetComponent<PacmanCamera>().DisableCameraOperation();
            pacman.GetComponent<PacmanPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the time scale

            pausePage.SetActive(true); // Display the pause page
        }

        /**
         * Resumes the game.
         */
        public void ResumeGame() {
            GameObject pacman = GameObject.FindGameObjectWithTag("Pacman");
            pacman.GetComponent<PacmanMovement>().EnableMovement();
            pacman.GetComponent<PacmanCamera>().EnableCameraOperation();
            pacman.GetComponent<PacmanPropOperation>().EnablePropOperation();

            Time.timeScale = 1f; // Resume the time scale

            pausePage.SetActive(false); // Close the pause page
        }

        /**
         * The player wins the game.
         * Called when all dots are eaten by the pacman.
         */
        public void Win() {
            _gamePlaying = false;

            // Disable pacman control
            GameObject pacman = GameObject.FindGameObjectWithTag("Pacman");
            pacman.GetComponent<PacmanMovement>().DisableMovement();
            pacman.GetComponent<PacmanCamera>().DisableCameraOperation();
            pacman.GetComponent<PacmanPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the time scale

            // Display win page
            winPage.SetActive(true);
            winPage.GetComponent<WinPage>().UpdateText();
        }

        /**
         * The player loses the game.
         * Called when a ghostron catches the pacman.
         */
        public void Lose() {
            _gamePlaying = false;

            // Disable pacman control
            GameObject pacman = GameObject.FindGameObjectWithTag("Pacman");
            pacman.GetComponent<PacmanMovement>().DisableMovement();
            pacman.GetComponent<PacmanCamera>().DisableCameraOperation();
            pacman.GetComponent<PacmanPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the time scale

            // Display lose page
            losePage.SetActive(true);
        }

        /**
         * Returns the difficulty of the current game.
         */
        public char GetDifficulty() {
            return _difficulty;
        }

        /**
         * Returns the elapsed time of the current game.
         * Used by the game over win page to display the time used.
         */
        public float GetTime() {
            // Warning if game is in progress
            if (_gamePlaying) {
                Debug.LogWarning("Game is still in progress but there is an attempt to obtain the game time!");
            }

            return _gameTimer;
        }
    }
}