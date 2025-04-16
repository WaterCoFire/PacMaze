using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Pacman {
    /**
     * Manages the camera of the pacman.
     * - Initialization
     * - "Turn Back" operation (Default key: B, only works in FPV)
     * - Switch View operation (Default key: V)
     */
    public class PacmanCamera : MonoBehaviour {
        // The camera game object
        private Camera _camera;

        // Key code for camera operation (B & V by default, they can be customized in Setting)
        private KeyCode _turnBackKeyCode;
        private KeyCode _switchViewKeyCode;

        // Status indicating if the pacman camera is controllable
        private bool _controllable;

        // Position offsets of the camera, third person view and first person view
        private readonly Vector3 _thirdPersonOffset = new(0, 1.76f, -1.78f);
        private readonly Vector3 _firstPersonOffset = new(0, 0.722f, 0.366f);

        // The rotation speed and transition (FPV <> TPV) speed of the camera
        private readonly float _cameraRotationSpeed = 2f;
        private readonly float _cameraTransitionSpeed = 10f;

        // Horizontal and vertical rotation angles of the camera
        private float _yaw; // Y-axis rotation (left-right look)
        private float _pitch; // X-axis rotation (up-down look)

        // Current offset of the camera from Pacman (depends on view mode)
        private Vector3 _currentOffset;

        // Status indicating if the player is in third person view
        private bool _inThirdPersonView;

        // Pacman movement component
        private PacmanMovement _pacmanMovement;

        // START FUNCTION
        private void Start() {
            Debug.Log("PacmanCamera START");

            // Set the camera game and pacman movement object
            _camera = gameObject.GetComponentInChildren<Camera>();
            _pacmanMovement = gameObject.GetComponent<PacmanMovement>();

            // Get the keycode set for look back & switch view operations
            _turnBackKeyCode = GetKeyCode("TurnBackKeyCode", KeyCode.B);
            _switchViewKeyCode = GetKeyCode("SwitchViewKeyCode", KeyCode.V);

            _currentOffset = _thirdPersonOffset;
            _camera.transform.localPosition = _currentOffset;
            _pacmanMovement.SetViewMode(true);

            // TEST ONLY
            _controllable = true;
            _inThirdPersonView = true;

            _yaw = transform.eulerAngles.y;
        }

        private void LateUpdate() {
            if (!_controllable) return;

            float mouseX = Input.GetAxis("Mouse X"); // Horizontal camera control
            float mouseY = Input.GetAxis("Mouse Y"); // Vertical camera control

            if (_inThirdPersonView) {
                // Third person: Orbit camera around Pacman
                _yaw += mouseX * _cameraRotationSpeed;
                _pitch -= mouseY * _cameraRotationSpeed;
                _pitch = Mathf.Clamp(_pitch, -30f, 60f); // Limit vertical rotation

                Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
                Vector3 desiredPosition = transform.position + rotation * _thirdPersonOffset;

                // Smooth camera position transition
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPosition,
                    Time.deltaTime * _cameraTransitionSpeed);
                _camera.transform.LookAt(transform.position + Vector3.up * 1.0f); // Look at Pacman’s head
            } else {
                // First person: Control Pacman's view direction
                _yaw += mouseX * _cameraRotationSpeed;
                _pitch -= mouseY * _cameraRotationSpeed;
                _pitch = Mathf.Clamp(_pitch, -45f, 45f); // Narrower FPV pitch range

                Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0);

                // Smoothly move camera to FPV position
                _camera.transform.position = Vector3.Lerp(
                    _camera.transform.position,
                    transform.position + Quaternion.Euler(0, _yaw, 0) * _firstPersonOffset,
                    Time.deltaTime * _cameraTransitionSpeed
                );

                // Smoothly rotate camera to match FPV view
                _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, targetRotation,
                    Time.deltaTime * _cameraTransitionSpeed);

                // Pacman orientation follows yaw (horizontal only)
                transform.rotation = Quaternion.Euler(0, _yaw, 0);
            }

            // Switch between FPV and TPV
            if (Input.GetKeyDown(_switchViewKeyCode)) {
                _inThirdPersonView = !_inThirdPersonView;
                _pacmanMovement.SetViewMode(_inThirdPersonView);

                // Camera offset reset
                _currentOffset = _inThirdPersonView ? _thirdPersonOffset : _firstPersonOffset;

                if (_inThirdPersonView) {
                    // Reset yaw/pitch for a better third-person camera start angle
                    _yaw = transform.eulerAngles.y;
                    _pitch = 15f;
                }
            }

            // Quick turn back
            // Only works in FIRST PERSON VIEW
            if (Input.GetKeyDown(_turnBackKeyCode) && !_inThirdPersonView) {
                // Instantly turn the camera around
                _yaw += 180f;
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