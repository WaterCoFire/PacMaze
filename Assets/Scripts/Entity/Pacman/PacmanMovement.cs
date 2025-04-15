using UnityEngine;

namespace Entity.Pacman {
    /**
     * Manages the movement of pacman:
     * (Controlled by the player)
     * - Go Forward / Backward / Leftward / Rightward
     * - Rotate (controlled by mouse dragging)
     */
    public class PacmanMovement : MonoBehaviour {
        // Move speed and rotate speed
        public float moveSpeed;
        public float rotateSpeed;

        // Key code for moving (WASD by default, they can be customized in Setting)
        private KeyCode _forwardKeyCode;
        private KeyCode _backwardKeyCode;
        private KeyCode _leftwardKeyCode;
        private KeyCode _rightwardKeyCode;

        // Status indicating if the pacman is controllable
        // Should be false: when e.g. game paused, game ended
        private bool _controllable;
        
        private float _mouseX;
        private float _rotationY;

        // START FUNCTION
        private void Start() {
            // Get the keycode set for four direction operations
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

            Move();
            Rotate();
        }

        /**
         * Manages the movement of the pacman.
         * (Forward, Backward, Leftward, Rightward)
         */
        private void Move() {
            Vector3 direction = Vector3.zero;

            // Forward movement
            if (Input.GetKey(_forwardKeyCode)) {
                direction += transform.forward;
            }

            // Backward movement
            if (Input.GetKey(_backwardKeyCode)) {
                direction -= transform.forward;
            }

            // Leftward movement
            if (Input.GetKey(_leftwardKeyCode)) {
                direction -= transform.right;
            }

            // Rightward movement
            if (Input.GetKey(_rightwardKeyCode)) {
                direction += transform.right;
            }

            direction.y = 0; // Prevent the impact of Y-axis movement
            
            // Update the position
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }

        /**
         * Manages the rotation of the pacman.
         * This is controlled by the mouse dragging.
         */
        private void Rotate() {
            _mouseX = Input.GetAxis("Mouse X"); // Mouse movement on X-axis
            _rotationY += _mouseX * rotateSpeed; // Calculate the Y-axis rotation

            // Rotate the pacman object
            transform.rotation = Quaternion.Euler(0f, _rotationY, 0f);
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