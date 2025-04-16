using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Pacman {
    /**
     * Manages the movement of pacman:
     * (Controlled by the player)
     * - Go Forward / Backward / Leftward / Rightward
     * - Rotate (controlled by mouse dragging)
     */
    public class PacmanMovement : MonoBehaviour {
        // Move speed and rotate speed
        private float _pacmanMoveSpeed = 5f;
        private float _pacmanRotateSpeed = 5f;

        // Key code for moving (WASD by default, it can be customized in Setting)
        private KeyCode _forwardKeyCode;
        private KeyCode _backwardKeyCode;
        private KeyCode _leftwardKeyCode;
        private KeyCode _rightwardKeyCode;

        // Status indicating if the pacman is controllable
        // Should be false: when e.g. game paused, game ended
        private bool _controllable;
        
        // Status indicating if the player is in third person view
        private bool _inThirdPersonView;

        private float _mouseX;
        private float _rotationY;

        // START FUNCTION
        private void Start() {
            Debug.Log("PacmanMovement START");

            // Get the keycode set for moving operations
            _forwardKeyCode = GetKeyCode("ForwardKeyCode", KeyCode.W);
            _backwardKeyCode = GetKeyCode("BackwardKeyCode", KeyCode.S);
            _leftwardKeyCode = GetKeyCode("LeftwardKeyCode", KeyCode.A);
            _rightwardKeyCode = GetKeyCode("RightwardKeyCode", KeyCode.D);

            Cursor.lockState = CursorLockMode.Locked; // Lock mouse
            Cursor.visible = false;

            // TEST ONLY
            _controllable = true;
        }

        // UPDATE FUNCTION
        private void Update() {
            if (!_controllable) return;
                
            Move(); // Pacman movement operation
        }

        /**
         * Manages the movement of the pacman.
         */
        private void Move() {
            float h = 0f, v = 0f;

            if (Input.GetKey(_forwardKeyCode)) v += 1f;
            if (Input.GetKey(_backwardKeyCode)) v -= 1f;
            if (Input.GetKey(_rightwardKeyCode)) h += 1f;
            if (Input.GetKey(_leftwardKeyCode)) h -= 1f;

            Vector3 inputDir = new Vector3(h, 0, v);

            if (inputDir.sqrMagnitude < 0.01f) return;

            inputDir.Normalize();

            // Get the forward and right vectors of the camera
            // (excluding y-component)
            Transform cam = Camera.main.transform;
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Calculate camera-based movement direction
            Vector3 moveDir = inputDir.z * camForward + inputDir.x * camRight;
            moveDir.Normalize();

            // Pacman movement
            transform.position += moveDir * _pacmanMoveSpeed * Time.deltaTime;
            
            // Make the pacman face the current direction of movement
            // ONLY IN THIRD PERSON VIEW
            if (_inThirdPersonView) {
                if (moveDir.sqrMagnitude > 0.01f) {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _pacmanRotateSpeed * Time.deltaTime);
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
         * Sets the current view mode.
         */
        public void SetViewMode(bool thirdPerson) {
            _inThirdPersonView = thirdPerson;
        }

        /**
         * Allows the player to control the movement of the pacman.
         */
        public void EnableMovement() {
            _controllable = true;
        }

        /**
         * Stops the player from controlling the movement of the pacman.
         */
        public void DisableMovement() {
            _controllable = false;
        }
    }
}