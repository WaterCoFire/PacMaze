using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage {
    public class EditMapView : MonoBehaviour {
        public ScrollRect editMapScrollRect;
        public GameObject mapInfoPrefab;

        public Button prefabEditButton;
        public Button prefabRenameButton;
        public Button prefabDeleteButton;

        private List<string> _mapNames;
    }
}