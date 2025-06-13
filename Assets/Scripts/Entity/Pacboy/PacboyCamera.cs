using Entity.Map;
using PlayMap;
using UnityEngine;

namespace Entity.Pacboy {
    /**
     * Manages the camera of the Pacboy.
     * - Initialisation
     * - "Turn Back" operation (Default key: Q, only works in FPV)
     * - Switch View operation (Default key: V)
     */
    public class PacboyCamera : MonoBehaviour {
        // The camera game object
        private Camera _camera;

        // The mini map panel on the top right
        private GameObject _mapPanel;

        // Key code for camera operation (B/V/M by default, they can be customized in Setting)
        private KeyCode _turnBackKeyCode;
        private KeyCode _switchViewKeyCode;
        private KeyCode _openMapKeyCode;

        // Status indicating if the Pacboy camera is controllable
        private bool _controllable;

        // Position offsets of the camera, third person view and first person view
        private readonly Vector3 _thirdPersonOffset = new(0, 1.76f, -1.78f);
        private readonly Vector3 _firstPersonOffset = new(0, 0.722f, 0.366f);

        // The rotation speed and transition (FPV <> TPV) speed of the camera
        private readonly float _cameraRotationSpeed = 1.5f;
        private readonly float _cameraTransitionSpeed = 10f;

        // Horizontal and vertical rotation angles of the camera
        private float _yaw; // Y-axis rotation (left-right look)
        private float _pitch; // X-axis rotation (up-down look)

        // Current offset of the camera from Pacboy (depends on view mode)
        private Vector3 _currentOffset;

        // Status indicating if the player is in third person view
        private bool _inThirdPersonView;

        // Pacboy movement component
        private PacboyMovement _pacboyMovement;

        // START FUNCTION
        private void Start() {
            // Set the camera game and Pacboy movement object
            _camera = gameObject.GetComponentInChildren<Camera>();
            _pacboyMovement = gameObject.GetComponent<PacboyMovement>();

            _currentOffset = _thirdPersonOffset;
            _camera.transform.localPosition = _currentOffset;
            _pacboyMovement.SetViewMode(true);

            _inThirdPersonView = true;
            
            // By default, the map is not displayed
            _mapPanel = GameObject.Find("MapPanel");
            _mapPanel.SetActive(false);

            _yaw = transform.eulerAngles.y;
            
            EnableCameraOperation(); // Enable Pacboy camera operation when game starts
        }

        // Each Frame
        private void LateUpdate() {
            if (!_controllable) return;

            float mouseX = Input.GetAxis("Mouse X"); // Horizontal camera control
            float mouseY = Input.GetAxis("Mouse Y"); // Vertical camera control

            if (_inThirdPersonView) {
                // Third person: Orbit camera around Pacboy
                _yaw += mouseX * _cameraRotationSpeed;
                _pitch -= mouseY * _cameraRotationSpeed;
                _pitch = Mathf.Clamp(_pitch, -30f, 60f); // Limit vertical rotation

                Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
                Vector3 desiredPosition = transform.position + rotation * _thirdPersonOffset;

                // Smooth camera position transition
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPosition,
                    Time.deltaTime * _cameraTransitionSpeed);
                _camera.transform.LookAt(transform.position + Vector3.up * 1.0f); // Look at Pacboy’s head
            } else {
                // First person: Control Pacboy's view direction
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

                // Pacboy orientation follows yaw (horizontal only)
                transform.rotation = Quaternion.Euler(0, _yaw, 0);
            }

            // Switch between FPV and TPV
            if (Input.GetKeyDown(_switchViewKeyCode)) {
                _inThirdPersonView = !_inThirdPersonView;
                _pacboyMovement.SetViewMode(_inThirdPersonView);

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
            
            // Map logic
            // Open/Close the map
            if (Input.GetKeyDown(_openMapKeyCode)) {
                SetMapView(!_mapPanel.activeSelf);
            }
        }

        /**
         * Sets the open/close status of the map in game.
         */
        private void SetMapView(bool open) {
            // Operation is not allowed in hard mode
            if (PlayMapController.Instance.GetDifficulty() == DifficultyType.Hard) {
                Debug.LogWarning("Map cannot be shown in hard mode.");
                return;
            }
            
            _mapPanel.SetActive(open); // Display/Hide the map camera
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
         * Allows the player to control the camera of the Pacboy.
         */
        public void EnableCameraOperation() {
            // Update KeyCode
            _turnBackKeyCode = GetKeyCode("TurnBackKeyCode", KeyCode.Q);
            _switchViewKeyCode = GetKeyCode("SwitchViewKeyCode", KeyCode.V);
            
            _controllable = true;
        }

        /**
         * Stops the player from controlling the camera of the Pacboy.
         */
        public void DisableCameraOperation() {
            _controllable = false;
        }
    }
}