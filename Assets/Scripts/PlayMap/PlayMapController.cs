using System.IO;
using System.Text.RegularExpressions;
using Entity.Map;
using Entity.Pacboy;
using Newtonsoft.Json;
using PlayMap.UI;
using Setting;
using TMPro;
using UnityEngine;

namespace PlayMap {
    /**
     * Manages the map that the player plays in (controls the Pacboy).
     */
    public class PlayMapController : MonoBehaviour {
        // Map data save directory
        private readonly string _saveDirectory = Path.Combine(Application.dataPath, "Data", "Maps");
        private readonly Regex _regex = new(@"^([^_]+)_(\d+)_([A-Za-z])");

        private char _difficulty; // Difficulty of the current game
        private int _currentScore; // Current game score

        private bool _gamePlaying; // Status telling if the game is currently in progress or not

        // GAME TIMER
        private float _gameTimer;

        // Pacboy game object
        private GameObject _pacboy;

        // Score Display UI
        public TMP_Text scoreText; // Score text
        public GameObject superBonusPrompt; // Prompt when Super Bonus is triggered

        public GameObject pausePage; // Pause page
        public GameObject winPage; // Game over page: player wins
        public GameObject losePage; // Game over page: player loses

        // Event logic - Super Bonus active/disactive (Double the score obtained)
        private bool _superBonus;
        private bool _airWall;
        public GameObject walls; // Wall game object

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
            _superBonus = false; // By default there is no Super Bonus
            _airWall = false; // By default there is no Air Wall
            _gamePlaying = true;

            // Reset score UI
            _currentScore = 0;
            scoreText.text = "Score: " + _currentScore;
            scoreText.color = Color.white;
            superBonusPrompt.SetActive(false);
        }

        /**
         * Initializes the map:
         * - Generates the wall according to file data
         * - Places all the fixed props
         * - Randomly places the remaining props (num for each prop: total - fixed)
         * - Places the ghostrons at their fixed spawn point
         * - Randomly generates the remaining ghostrons (num: total - fixed)
         * - Generates dots for all the spare tiles
         * - Generates the Pacboy
         */
        private void InitMap(string mapFileName) {
            // Parse the map file name
            Match match = _regex.Match(mapFileName);

            if (!match.Success) {
                Debug.LogError("File match error when loading map file to play! File name: " + mapFileName);
                return;
            }

            // Obtain the file path
            string path = Path.Combine(_saveDirectory, mapFileName + ".json");
            if (!File.Exists(path)) {
                Debug.LogError("Load map to play error: File not found!");
                return;
            }

            // Read the file
            string json = File.ReadAllText(path);

            // Parse the json data and store in the wrapper
            MapJsonWrapper wrapper = JsonConvert.DeserializeObject<MapJsonWrapper>(json);

            // Difficulty setting
            _difficulty = wrapper.difficulty;

            // Update difficulty in GhostronManager to for setting the behaviours of the ghostrons
            GhostronManager.Instance.SetDifficulty(_difficulty);

            // Set the event status in EventManager (enable/disable event)
            EventManager.Instance.SetEventStatus(wrapper.eventEnabled);

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
            GameObject pacboy = GameObject.FindGameObjectWithTag("Pacboy");
            pacboy.GetComponent<PacboyMovement>().DisableMovement();
            pacboy.GetComponent<PacboyCamera>().DisableCameraOperation();
            pacboy.GetComponent<PacboyPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the time scale
            _gamePlaying = false;

            pausePage.SetActive(true); // Display the pause page
        }

        /**
         * Resumes the game.
         */
        public void ResumeGame() {
            GameObject pacboy = GameObject.FindGameObjectWithTag("Pacboy");
            pacboy.GetComponent<PacboyMovement>().EnableMovement();
            pacboy.GetComponent<PacboyCamera>().EnableCameraOperation();
            pacboy.GetComponent<PacboyPropOperation>().EnablePropOperation();

            Time.timeScale = 1f; // Resume the time scale
            _gamePlaying = true;

            pausePage.SetActive(false); // Close the pause page
        }

        /**
         * The player wins the game.
         * Called when all dots are eaten by the Pacboy.
         */
        public void Win() {
            _gamePlaying = false;

            // Disable Pacboy control
            _pacboy.GetComponent<PacboyMovement>().DisableMovement();
            _pacboy.GetComponent<PacboyCamera>().DisableCameraOperation();
            _pacboy.GetComponent<PacboyPropOperation>().DisablePropOperation();

            Time.timeScale = 0f; // Stop the time scale

            // Display win page
            winPage.SetActive(true);
            winPage.GetComponent<WinPage>().UpdateText();
        }

        /**
         * The player loses the game.
         * Called when a ghostron catches the Pacboy.
         */
        public void Lose() {
            _gamePlaying = false;

            // Disable Pacboy control
            _pacboy.GetComponent<PacboyMovement>().DisableMovement();
            _pacboy.GetComponent<PacboyCamera>().DisableCameraOperation();
            _pacboy.GetComponent<PacboyPropOperation>().DisablePropOperation();

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

        /**
         * Returns the game score.
         * Used by the game over win page to display the score.
         */
        public int GetScore() {
            // Warning if game is in progress
            if (_gamePlaying) {
                Debug.LogWarning("Game is still in progress but there is an attempt to obtain the game score!");
            }

            return _currentScore;
        }

        /**
         * Adds some score.
         */
        public void AddScore(int score) {
            // Show error if game is NOT in progress
            if (!_gamePlaying) {
                Debug.LogError("Error: Game is NOT in progress but score is being added!");
                return;
            }

            // Update score
            if (_superBonus) {
                // Super Bonus - double score
                _currentScore += score * 2;
            } else {
                // No Super Bonus - Normal
                _currentScore += score;
            }

            scoreText.text = "Score: " + _currentScore;
        }

        /**
         * Deducts some score.
         */
        public void DeductScore(int score) {
            // Show error if game is NOT in progress
            if (!_gamePlaying) {
                Debug.LogError("Error: Game is NOT in progress but score is being added!");
                return;
            }

            // Update score
            _currentScore -= score;
            // Cap the score to 0 if it is less than 0
            if (_currentScore < 0) _currentScore = 0;
            scoreText.text = "Score: " + _currentScore;
        }

        /**
         * Sets the Pacboy game object of the current game.
         */
        public void SetPacboy(GameObject pacboy) {
            _pacboy = pacboy;
        }

        /**
         * Gets the Pacboy game object.
         */
        public GameObject GetPacboy() {
            return _pacboy;
        }

        /**
         * Sets the status of Super Bonus.
         * Called by EventManager when the Super Bonus should be on/off.
         */
        public void SetSuperBonus(bool on) {
            if (on) {
                if (_superBonus) {
                    Debug.LogError("Error: Super Bonus is already active!");
                    return;
                }

                // Set Super Bonus
                _superBonus = true;

                // UI Reset
                scoreText.color = Color.yellow;
                superBonusPrompt.SetActive(true);
            } else {
                if (!_superBonus) {
                    Debug.LogError("Error: Super Bonus is not active!");
                    return;
                }

                // Cancel Super Bonus
                _superBonus = false;

                // UI Reset
                scoreText.color = Color.white;
                superBonusPrompt.SetActive(false);
            }
        }

        /**
         * Sets the status of Air Wall.
         * Called by EventManager when the Air Wall should be on/off.
         */
        public void SetAirWall(bool on) {
            if (on) {
                if (_airWall) {
                    Debug.LogError("Error: Air Wall is already active!");
                    return;
                }

                // Make all the walls invisible
                _airWall = true;

                // Get all Renderers in children of wall object and disable them
                Renderer[] renderers = walls.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers) {
                    renderer.enabled = false;
                }
            } else {
                if (!_airWall) {
                    Debug.LogError("Error: Air Wall is not active!");
                    return;
                }

                // Make the walls visible again
                _airWall = false;

                // Get all Renderers in children of wall object and enable them
                Renderer[] renderers = walls.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers) {
                    renderer.enabled = true;
                }
            }
        }
    }
}