using System.Text.RegularExpressions;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HomePage {
    public class EditMapCreateWindow : MonoBehaviour {
        public GameObject editMapViewWindow;

        // Create map window UI elements
        public GameObject createWindow;
        public TMP_InputField createInputField;
        public GameObject createWarningPrompt;
        public TMP_Text warningText;
        public Button createCloseButton;
        public Button createConfirmButton;

        private void Start() {
            SetButtonActionListener();
        }

        private void SetButtonActionListener() {
            createConfirmButton.onClick.AddListener(OnCreateConfirmButtonClick);
            createCloseButton.onClick.AddListener(OnCreateCloseButtonClick);
        }

        public void ShowCreateWindow() {
            editMapViewWindow.SetActive(false);
            createWindow.SetActive(true);
            createWarningPrompt.SetActive(false);

            createInputField.text = "";
        }

        private void OnCreateConfirmButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Get the name
            string newNameInput = createInputField.text;
            
            // Check name validity
            if (!CheckNameValidity(newNameInput)) {
                // If the name is invalid
                // Play warning sound
                SoundManager.Instance.PlaySoundOnce(SoundType.Warning);
                
                // Display warning prompt
                createWarningPrompt.SetActive(true);
                return;
            }

            // Transform to upper letters
            newNameInput = newNameInput.ToUpper();
            
            editMapViewWindow.GetComponent<EditMapPage>().CreateMap(newNameInput);
            
            // Close create window
            createWindow.SetActive(false);
            createWarningPrompt.SetActive(false);
            
            // Enable the map view window
            editMapViewWindow.SetActive(true);
        }

        private void OnCreateCloseButtonClick() {
            // Play click sound
            SoundManager.Instance.PlaySoundOnce(SoundType.Click);
            
            // Disable the warning prompt
            createWarningPrompt.SetActive(false);

            // Close rename window
            createWindow.SetActive(false);

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
            
            // Name conflict
            if (editMapViewWindow.GetComponent<EditMapPage>().CheckNameConflict(nameInput.ToUpper())) {
                warningText.text = "Pacboy is confused because there is another map named this!";
                return false;
            }

            return true;
        }
    }
}