using UnityEngine;
using UnityEngine.UI;

namespace MapEditor {
    /**
     * Event editor
     * Sets whether or not the game events can be triggered
     * A game event might be triggered everytime a dot is eaten by Pacboy
     * POSSIBLE EVENTS
     * - Super Bonus (All scores awarded are doubled)
     * - Crazy Party (Pacboy and Ghostrons are way faster)
     * - Air Walls (Walls become invisible)
     */
    public class EventEditor : MonoBehaviour {
        public Button enableEventButton;
        public Button disableEventButton;

        private bool _eventEnabled;

        // BUTTON COLORS
        private readonly Color _normalColor = new(140f / 255f, 121f / 255f, 255f / 255f);
        private readonly Color _selectedColor = new(228f / 255f, 255f / 255f, 73f / 255f);

        // Enabled/Disabled text prompts
        public GameObject eventEnabledPrompt;
        public GameObject eventDisabledPrompt;
        
        public static EventEditor Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("TileChecker AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            SetButtonActionListener();
        }

        /**
         * Obtains the event status data.
         * Used by Map Editor when saving the map data.
         */
        public bool GetEventStatusData() {
            return _eventEnabled;
        }

        /**
         * Sets the event status data.
         * Used by Map Editor when entering the event setting mode.
         */
        public void SetEventStatusData(bool eventEnabled) {
            _eventEnabled = eventEnabled;
        }

        /**
         * Enters the event setting mode.
         * Used by Map Editor when user clicks the event setting mode.
         */
        public void EnterEventMode() {
            // Update UI based on the event status level
            if (_eventEnabled) {
                OnEventEnabledButtonClick();
            } else {
                OnEventDisabledButtonClick();
            }
        }

        /**
         * Quit the event setting mode.
         * Used by Map Editor when user quits the map editor or enters other setting modes.
         * NO LOGIC HERE
         */
        public void QuitEventMode() { }

        /**
         * Sets the color of a button.
         */
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

        private void SetButtonActionListener() {
            enableEventButton.onClick.AddListener(OnEventEnabledButtonClick);
            disableEventButton.onClick.AddListener(OnEventDisabledButtonClick);
        }

        /* Event status button operations */
        private void OnEventEnabledButtonClick() {
            // UI update
            SetButtonStatus(enableEventButton, true);
            SetButtonStatus(disableEventButton, false);

            // Prompt update
            eventEnabledPrompt.SetActive(true);
            eventDisabledPrompt.SetActive(false);

            // Set event status data
            _eventEnabled = true;
        }

        private void OnEventDisabledButtonClick() {
            // UI update
            SetButtonStatus(disableEventButton, true);
            SetButtonStatus(enableEventButton, false);

            // Prompt update
            eventDisabledPrompt.SetActive(true);
            eventEnabledPrompt.SetActive(false);

            // Set event status data
            _eventEnabled = false;
        }
    }
}