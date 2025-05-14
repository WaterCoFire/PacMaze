using UnityEngine;

namespace PlayMap {
    public class GhostronFactory : MonoBehaviour {
        public GameObject redGhostronPrefab;
        public GameObject blueGhostronPrefab;
        public GameObject yellowGhostronPrefab;
        public GameObject pinkGhostronPrefab;
        public GameObject greenGhostronPrefab;
        public GameObject tenaciousGhostronPrefab;

        // Singleton instance
        public static GhostronFactory Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("GhostronFactory AWAKE");
            // Set singleton instance
            Instance = this;
        }

        public GameObject GetGhostron(string type) {
            switch (type) {
                case "Red":
                    return redGhostronPrefab;
                case "Blue":
                    return blueGhostronPrefab;
                case "Yellow":
                    return yellowGhostronPrefab;
                case "Pink":
                    return pinkGhostronPrefab;
                case "Green":
                    return greenGhostronPrefab;
                case "Tenacious":
                    return tenaciousGhostronPrefab;
                default:
                    Debug.LogError("Invalid Ghostron type name: " + type);
                    return null;
            }
        }
    }
}