using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage {
    public class EditMapCreateWindow : MonoBehaviour {
        public GameObject editMapViewWindow;

        // Create map window UI elements
        public GameObject createWindow;
        public TMP_InputField createInputField;
        public GameObject createWarningPrompt;
        public Button createCloseButton;
        public Button createConfirmButton;

        private void Start() {
            SetButtonActionListener();
        }

        private void SetButtonActionListener() {
            createConfirmButton.onClick.AddListener(OnCreateConfirmButtonClick);
            createCloseButton.onClick.AddListener(OnCreateCloseButtonClick);
        }

        public void ShowCreateWindow(string originName) {
            editMapViewWindow.SetActive(false);
            createWindow.SetActive(true);

            createInputField.text = "";
        }

        private void OnCreateConfirmButtonClick() {
            string newNameInput = createInputField.text;
            // TODO Check name validity
            
            editMapViewWindow.GetComponent<EditMapView>().CreateMap("newNameInput");
            
            // Close create window
            createWindow.SetActive(false);
            
            // Enable the map view window
            editMapViewWindow.SetActive(true);
        }

        private void OnCreateCloseButtonClick() {
            // Disable the warning prompt
            createWarningPrompt.SetActive(false);

            // Close rename window
            createWindow.SetActive(false);

            // Enable the map view window
            editMapViewWindow.SetActive(true);
        }
    }
}