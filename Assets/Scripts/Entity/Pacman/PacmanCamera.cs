using UnityEngine;

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

        // Key code for camera operation (B & V by default, it can be customized in Setting)
        private KeyCode _turnBackKeyCode;
        private KeyCode _switchViewKeyCode;

        // Status indicating if the pacman camera is controllable
        private bool _controllable;

        // Position offsets of the camera, third person view and first person view
        private readonly Vector3 _thirdPersonOffset = new(0, 1.76f, -1.78f);
        private readonly Vector3 _firstPersonOffset = new(0, 0.722f, 0.366f);

        public float rotationSpeed = 2f;
        public float transitionSpeed = 5f;

        private float _yaw;
        private float _pitch;
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

        // UPDATE FUNCTION
        private void LateUpdate() {
            if (!_controllable) return;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (_inThirdPersonView) {
                _yaw += mouseX * rotationSpeed;
                _pitch -= mouseY * rotationSpeed;
                _pitch = Mathf.Clamp(_pitch, -30f, 60f);

                Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
                Vector3 desiredPosition = transform.position + rotation * _thirdPersonOffset;

                _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPosition, Time.deltaTime * transitionSpeed);
                _camera.transform.LookAt(transform.position + Vector3.up * 1.0f);
            } else {
                _yaw += mouseX * rotationSpeed;
                _pitch -= mouseY * rotationSpeed;
                _pitch = Mathf.Clamp(_pitch, -45f, 45f);

                Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0);

                _camera.transform.position = Vector3.Lerp(
                    _camera.transform.position,
                    transform.position + Quaternion.Euler(0, _yaw, 0) * _firstPersonOffset,
                    Time.deltaTime * transitionSpeed
                );

                _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, targetRotation, Time.deltaTime * transitionSpeed);

                // Pacman orientation
                transform.rotation = Quaternion.Euler(0, _yaw, 0);
            }

            if (Input.GetKeyDown(_switchViewKeyCode)) {
                _inThirdPersonView = !_inThirdPersonView;
                _currentOffset = _inThirdPersonView ? _thirdPersonOffset : _firstPersonOffset;
                _pacmanMovement.SetViewMode(_inThirdPersonView);

                if (_inThirdPersonView) {
                    _yaw = transform.eulerAngles.y;
                    _pitch = 15f;
                }
            }

            // Quick turn back
            // Only works in FIRST PERSON VIEW
            if (Input.GetKeyDown(_turnBackKeyCode) && !_inThirdPersonView) {
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