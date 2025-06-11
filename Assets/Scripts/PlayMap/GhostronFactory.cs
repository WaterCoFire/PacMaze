using Entity.Ghostron;
using UnityEngine;

namespace PlayMap {
    /**
     * Used for creating new Ghostrons.
     */
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

        /**
         * Get a Ghostron game object.
         */
        public GameObject GetGhostron(GhostronType ghostronType) {
            switch (ghostronType) {
                case GhostronType.Red:
                    return redGhostronPrefab;
                case GhostronType.Blue:
                    return blueGhostronPrefab;
                case GhostronType.Yellow:
                    return yellowGhostronPrefab;
                case GhostronType.Pink:
                    return pinkGhostronPrefab;
                case GhostronType.Green:
                    return greenGhostronPrefab;
                case GhostronType.Tenacious:
                    return tenaciousGhostronPrefab;
                default:
                    Debug.LogError("Invalid Ghostron type name: " + ghostronType);
                    return null;
            }
        }
    }
}