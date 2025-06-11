using System.Text.RegularExpressions;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HomePage {
    /**
     * Manages the "Rename Map" name setting window.
     */
    public class EditMapRenameWindow : MonoBehaviour {
        public GameObject editMapViewWindow;

        // Rename map window UI elements
        public GameObject renameWindow;
        public TMP_InputField renameInputField;
        public GameObject renameWarningPrompt;
        public TMP_Text warningText;
        public Button renameCloseButton;
        public Button renameConfirmButton;

        private string _originName;

        // START FUNCTION
        private void Start() {
            SetButtonActionListener();
        }

        /**
         * Sets the action listeners for all buttons.
         */
        private void SetButtonActionListener() {
            renameConfirmButton.onClick.AddListener(OnRenameConfirmButtonClick);
            renameCloseButton.onClick.AddListener(OnRenameCloseButtonClick);
        }

        /**
         * Initialises and displays the rename map window.
         */
        public void ShowRenameWindow(string originName) {
            editMapViewWindow.SetActive(false);
            renameWindow.SetActive(true);
            renameWarningPrompt.SetActive(false);

            _originName = originName;
            renameInputField.text = _originName;
        }

        /* Button Action Listeners */
        // Confirm button
        private void OnRenameConfirmButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Get the name
            string newNameInput = renameInputField.text;
            
            // Check name validity
            if (!CheckNameValidity(newNameInput)) {
                // If the name is invalid
                // Play warning sound
                SoundManager.Instance.PlaySoundOnce(SoundType.Warning);
                
                // Display warning prompt
                renameWarningPrompt.SetActive(true);
                return;
            }

            // Transform to upper letters
            newNameInput = newNameInput.ToUpper();

            editMapViewWindow.GetComponent<EditMapPage>().RenameMap(newNameInput);

            // Close rename window
            renameWindow.SetActive(false);
            renameWarningPrompt.SetActive(false);

            // Enable the map view window
            editMapViewWindow.SetActive(true);
        }

        // Close button
        private void OnRenameCloseButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Disable the warning prompt
            renameWarningPrompt.SetActive(false);

            // Close rename window
            renameWindow.SetActive(false);

            // Enable the map view window
            editMapViewWindow.SetActive(true);
        }

        /**
         * Checks if the name input is valid or not.
         */
        private bool CheckNameValidity(string nameInput) {
            // Check name length: too long
            if (nameInput.Length > 15) {
                warningText.text = "Pacboy feels pressure because that's too long!";
                return false;
            }
            
            // Check name length: too short
            if (nameInput.Length < 1) {
                warningText.text = "Pacboy feels empty, just like this name!";
                return false;
            }

            // No space at both ends
            if (nameInput.StartsWith(" ") || nameInput.EndsWith(" ")) {
                warningText.text = "Pacboy is unhappy because it hates space at the edges!";
                return false;
            }

            // No invalid characters
            if (!Regex.IsMatch(nameInput, @"^[A-Za-z0-9 ]+$")) {
                warningText.text = "Pacboy is scared because it sees some unusual characters!";
                return false;
            }
            
            // Name does not change at all
            if (nameInput == _originName) {
                warningText.text = "Pacboy is sad because nothing has changed at all!";
                return false;
            }

            // Name conflict
            if (editMapViewWindow.GetComponent<EditMapPage>().CheckNameConflict(nameInput.ToUpper())) {
                warningText.text = "Pacboy is confused because there is another map named this!";
                return false;
            }

            return true;
        }
    }
}