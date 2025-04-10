using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage {
    public class EditMapRenameWindow : MonoBehaviour {
        public GameObject editMapViewWindow;

        // Rename map window UI elements
        public GameObject renameWindow;
        public TMP_InputField renameInputField;
        public GameObject renameWarningPrompt;
        public Button renameCloseButton;
        public Button renameConfirmButton;

        private string _originName;
        
        private void Start() {
            SetButtonActionListener();
        }
        
        private void SetButtonActionListener() {
            renameConfirmButton.onClick.AddListener(OnRenameConfirmButtonClick);
            renameCloseButton.onClick.AddListener(OnRenameCloseButtonClick);
        }

        public void ShowRenameWindow(string originName) {
            editMapViewWindow.SetActive(false);
            renameWindow.SetActive(true);

            _originName = originName;
            renameInputField.text = _originName;
        }

        private void OnRenameConfirmButtonClick() {
            string newNameInput = renameInputField.text;
            // TODO Check name validity
            
            editMapViewWindow.GetComponent<EditMapView>().RenameMap("newNameInput");
            
            // Close rename window
            renameWindow.SetActive(false);
            
            // Enable the map view window
            editMapViewWindow.SetActive(true);
        }

        private void OnRenameCloseButtonClick() {
            // Disable the warning prompt
            renameWarningPrompt.SetActive(false);
            
            // Close rename window
            renameWindow.SetActive(false);
            
            // Enable the map view window
            editMapViewWindow.SetActive(true);
        }
    }
}