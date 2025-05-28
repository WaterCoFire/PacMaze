using System.Collections.Generic;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial {
    /**
     * Manages the UI in the tutorial level.
     */
    public class TutorialUI : MonoBehaviour {
        /* Tip UI */
        public GameObject tipPanel; // The panel used for displaying tips
        public TMP_Text tipText; // The TextMeshPro tip text area
        public TMP_Text actionPromptText; // The separate text area prompting the action the player should do now

        private List<List<string>> _allTips; // The tips displayed at all checkpoints

        /* Nice Bomb UI */
        // Displayed when the player has nice bombs (This button cannot be actually clicked)
        public Button niceBombButton;

        public TMP_Text niceBombNumText; // Inside the button, shows the number of nice bombs the player currently has

        /* Other UI Pages */
        public GameObject pausePage; // Pause page
        public GameObject finishPage; // Tutorial finish page

        /* Tip texts */
        public List<string> startTips;
        public List<string> powerPelletTips;
        public List<string> wheelsTips;
        public List<string> niceBombUseTips;
        public List<string> niceBombDeployTips;
        public List<string> badCherryTips;
        public List<string> finishTips;

        private bool _tipDisplaying; // Whether a tip is currently being displayed
        private List<string> _currentTip; // The current tip being displayed
        private int _currentTipIndex; // The index of the current tip
        private int _currentTipPage; // The current page No. of the tip

        // Singleton instance
        public static TutorialUI Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("TutorialUI AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            Debug.Log("TutorialUI START");

            // Initialise the tips list
            _allTips = new List<List<string>>();
            _allTips.Add(startTips);
            _allTips.Add(powerPelletTips);
            _allTips.Add(wheelsTips);
            _allTips.Add(niceBombUseTips);
            _allTips.Add(niceBombDeployTips);
            _allTips.Add(badCherryTips);
            _allTips.Add(finishTips);
        }

        // UPDATE FUNCTION
        private void Update() {
            // When displaying a tip
            // Check if the "Enter" key is pressed (Go to the next page)
            if (_tipDisplaying && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) {
                NextPage();
            }
        }

        /**
         * Display the given index.
         * Called by TutorialController when Pacboy reaches a checkpoint.
         */
        public void DisplayTip(int index) {
            if (_tipDisplaying) {
                Debug.LogWarning(
                    "Warning: A tip is being displayed while another tip is set to be displayed. Overriding.");
            }
            
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            // Close the action prompt area
            CloseActionPrompt();

            // Temporarily stop the timescale
            Time.timeScale = 0f;

            _currentTipIndex = index + 1; // Plus 1 as the Start Tip does not belong to a checkpoint
            _currentTip = _allTips[_currentTipIndex];
            _tipDisplaying = true;
            _currentTipPage = 0;

            // UI initialise setting
            tipText.text = _currentTip[_currentTipPage];
            tipPanel.SetActive(true);
            actionPromptText.gameObject.SetActive(false);
        }

        /**
         * Go to the next page when displaying a tip.
         * If the previous page is already the last page, close the display panel.
         */
        private void NextPage() {
            if (!_tipDisplaying) {
                Debug.LogError("Error: Tip is not being displayed!");
                return;
            }
            
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);

            if (_currentTipIndex != _allTips.Count - 1) {
                // For all tips except for the last (finish) tip:
                // The last text of the tip should be displayed in the action area (to better prompt the player)
                // The tip panel should also be closed
                if (_currentTipPage == _currentTip.Count - 2) {
                    tipPanel.SetActive(false);
                    _tipDisplaying = false;
                    Time.timeScale = 1f; // Resume the timescale

                    // Update the action prompt area
                    actionPromptText.gameObject.SetActive(true);
                    actionPromptText.text = _currentTip[_currentTipPage + 1];

                    return;
                }
            } else {
                // For the last tip:
                // Check if the current page is the last page
                if (_currentTipPage == _currentTip.Count - 1) {
                    // If so, close the tip panel
                    tipPanel.SetActive(false);
                    _tipDisplaying = false;

                    TutorialController.Instance.Finish(); // Last page of the last tip - finish the tutorial

                    return;
                }
            }

            // Display the next page
            _currentTipPage++;
            tipText.text = _currentTip[_currentTipPage];
        }

        /**
         * Hides the action prompt.
         * Called everytime the expected action is completed by the player.
         */
        public void CloseActionPrompt() {
            actionPromptText.gameObject.SetActive(false);
        }

        /**
         * Displays/Hides the Nice Bomb prompt UI.
         * Called by TutorialPacboy when Pacboy has 0 to 1 or 1 to 0 Nice Bomb.
         */
        public void SetNiceBombPrompt(bool on) {
            niceBombButton.gameObject.SetActive(on);
        }

        /**
         * Updates the number of Nice Bomb that Pacboy currently has.
         * Called by TutorialPacboy everytime that number changes.
         */
        public void UpdateNiceBombNum(int num) {
            niceBombNumText.text = num.ToString();
        }

        /**
         * Displays/Hides the pause page.
         * Called by TutorialController when the tutorial pauses/resumes.
         */
        public void SetPausePage(bool on) {
            pausePage.SetActive(on);
        }

        /**
         * Displays the finish page.
         * Called by TutorialController when the player completes the tutorial.
         */
        public void DisplayFinishPage() {
            finishPage.SetActive(true);
        }
    }
}