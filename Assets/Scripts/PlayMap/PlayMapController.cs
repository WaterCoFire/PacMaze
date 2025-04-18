using System.IO;
using System.Text.RegularExpressions;
using Entity.Map;
using Newtonsoft.Json;
using UnityEngine;

namespace PlayMap {
    /**
     * Manages the map that the player plays in (controls the pacman).
     */
    public class PlayMapController : MonoBehaviour {
        // Map data save directory
        private string _saveDirectory = Path.Combine(Application.dataPath, "Maps");
        private Regex _regex = new(@"^([^_]+)_(\d+)_([A-Za-z])");

        /**
         * THE START FUNCTION
         * Get the file name of the map to be played
         * and call InitMap() to initialize the map
         */
        private void Start() {
            Debug.Log("MapController START");
            
            // TODO TEST ONLY
            PlayerPrefs.SetString("PlayMapFileToLoad", "ABC123_5_E");

            string mapFileName = PlayerPrefs.GetString("PlayMapFileToLoad", null);
            if (mapFileName == null) {
                Debug.LogError("Play map: File name not properly set!");
                return;
            }

            InitMap(mapFileName); // Initialize the map
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
            // TODO Lucky Dice result probability
            
            // Update difficulty in GhostronManager to for setting the behaviours of the ghostrons
            gameObject.GetComponent<GhostronManager>().SetDifficulty(wrapper.difficulty);

            // Generate walls
            gameObject.GetComponent<WallGenerator>()
                .InitWalls(new WallData(wrapper.HorizontalWallStatus, wrapper.VerticalWallStatus));

            // Generate props
            PlayerPrefs.SetInt("GhostronCount", 0);
            PlayerPrefs.SetString("GameObjectReadMode", "PLAY");
            gameObject.GetComponent<PropGenerator>().InitProps(new PropData(wrapper.PropPositions(),
                wrapper.FixedPropCounts, wrapper.TotalPropCounts));

            return true;
        }
    }
}