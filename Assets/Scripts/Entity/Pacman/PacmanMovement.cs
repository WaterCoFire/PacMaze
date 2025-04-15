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
        private float _moveSpeed;
        private float _rotateSpeed;

        // Key code for moving (default WASD, can be customized)
        private KeyCode _forwardKeyCode;
        private KeyCode _backwardKeyCode;
        private KeyCode _leftwardKeyCode;
        private KeyCode _rightwardKeyCode;

        // Status indicating if the pacman is controllable
        // Should be false: when e.g. game paused, game ended
        private bool _controllable;

        // START FUNCTION
        private void Start() {
            // TODO get the keycode set
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
            var moveX = Input.GetAxis("Horizontal") * Time.deltaTime * _moveSpeed;
            var moveZ = Input.GetAxis("Vertical") * Time.deltaTime * _moveSpeed;
            Vector3 dir = new Vector3(moveX, 0, moveZ);


            if (dir != Vector3.zero) {
                transform.Translate(moveX, 0, moveZ);
            }
        }

        /**
         * Manages the rotation of the pacman.
         * This is controlled by the mouse dragging.
         */
        private void Rotate() {
            float mouseX = Input.GetAxis("Mouse X") * _rotateSpeed;
            transform.Rotate(0, mouseX, 0);
        }

        /**
         * Allows the player to control the movement of the pacman.
         */
        public void AllowMovement() {
            _controllable = true;
        }

        /**
         * Stops the player from controlling the movement of the pacman.
         */
        public void ProhibitMovement() {
            _controllable = false;
        }
    }
}