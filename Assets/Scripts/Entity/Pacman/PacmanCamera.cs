using UnityEngine;

namespace Entity.Pacman {
    /**
     * Manages the camera of the pacman.
     * - Initialization
     * - "Look Back" operation (Default key: B)
     * - Switch View operation (Default key: V)
     */
    public class PacmanCamera : MonoBehaviour {
        // The camera game object
        private Camera _camera;

        // Key code for moving (W by default, it can be customized in Setting)
        private KeyCode _lookBackKeyCode;
        private KeyCode _switchViewKeyCode;

        // Status indicating if the pacman camera is controllable
        private bool _controllable;

        // Positions of the camera
        private Vector3 _fpVector3 = new Vector3(0f, 0.722f, 0.366f);
        private Vector3 _fpVector3Backward = new Vector3(0f, 0.722f, -0.315f);
        private Vector3 _tpVector3 = new Vector3(0, 1.76f, -1.78f);
        private Vector3 _tpVector3Backward = new Vector3(0, 1.76f, 1.78f);

        // Status indicating if the player is in third person view
        private bool _inThirdPersonView;

        // START FUNCTION
        private void Start() {
            Debug.Log("PacmanCamera START");

            // Set the camera game object
            _camera = gameObject.GetComponentInChildren<Camera>();

            // Get the keycode set for look back & switch view operations
            _lookBackKeyCode = GetKeyCode("LookBackKeyCode", KeyCode.B);
            _switchViewKeyCode = GetKeyCode("SwitchViewKeyCode", KeyCode.V);

            // TEST ONLY
            _controllable = true;
            _inThirdPersonView = false;

            if (!_inThirdPersonView) {
                _camera.transform.localPosition = _fpVector3;
            } else {
                _camera.transform.localPosition = _tpVector3;
            }
        }

        // UPDATE FUNCTION
        private void Update() {
            if (!_controllable) return;

            // Look back key down
            if (Input.GetKeyDown(_lookBackKeyCode)) {
                if (!_inThirdPersonView) {
                    _camera.transform.localPosition = _fpVector3Backward;
                } else {
                    _camera.transform.localPosition = _tpVector3Backward;
                }

                // Set the rotation of the camera: look backward
                Vector3 currentEuler = _camera.transform.localEulerAngles;
                _camera.transform.localRotation = Quaternion.Euler(currentEuler.x, 180f, currentEuler.z);
            }

            // Look back key up
            if (Input.GetKeyUp(_lookBackKeyCode)) {
                if (!_inThirdPersonView) {
                    _camera.transform.localPosition = _fpVector3;
                } else {
                    _camera.transform.localPosition = _tpVector3;
                }
                
                // Set the rotation of the camera: look forward
                Vector3 currentEuler = _camera.transform.localEulerAngles;
                _camera.transform.localRotation = Quaternion.Euler(currentEuler.x, 0f, currentEuler.z);
            }

            // Switch view key pressed
            if (Input.GetKeyDown(_switchViewKeyCode)) {
                // Reset the rotation of the camera
                
                if (!_inThirdPersonView) {
                    // Switch to third person view
                    _camera.transform.localPosition = _tpVector3;
                    _camera.transform.localRotation = Quaternion.Euler(25f, 0f, 0f);
                    
                    _inThirdPersonView = true;
                } else {
                    // Switch to first person view
                    _camera.transform.localPosition = _fpVector3;
                    _camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    
                    _inThirdPersonView = false;
                }
            }
        }

        /**
         * Trys to obtain the corresponding key code in player preferences.
         * If failed, a warning is shown.
         */
        private KeyCode GetKeyCode(string key, KeyCode defaultKeyCode) {
            string keyString = PlayerPrefs.GetString(key, defaultKeyCode.ToString());
            if (System.Enum.TryParse(keyString, out KeyCode result)) {
                return result;
            } else {
                // Default key code logic due to failure in parsing
                Debug.LogWarning(
                    $"The key value in player preferences {keyString} cannot be transformed to KeyCode, using default KeyCode: {defaultKeyCode}");
                return defaultKeyCode;
            }
        }

        /**
         * Allows the player to control the camera of the pacman.
         */
        public void EnableCameraOperation() {
            _controllable = true;
        }

        /**
         * Stops the player from controlling the camera of the pacman.
         */
        public void DisableCameraOperation() {
            _controllable = false;
        }
    }
}