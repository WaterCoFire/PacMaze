using System.Linq;
using UnityEngine;

namespace Tutorial.Entities.TutorialPacboy {
    /**
     * For tutorial only
     * Manages the movement of Pacboy in tutorial.
     */
    public class TutorialPacboyMovement : MonoBehaviour {
        // Move speed and rotate speed
        private float _pacboyMoveSpeed;
        private readonly float _pacboyRotateSpeed = 5.0f;

        // Pacboy's normal speed
        private float _pacboyNormalMoveSpeed = 5.0f;

        // Key code for moving (WASD by default, not changeable in tutorial)
        private const KeyCode ForwardKeyCode = KeyCode.W;
        private const KeyCode BackwardKeyCode = KeyCode.S;
        private const KeyCode LeftwardKeyCode = KeyCode.A;
        private const KeyCode RightwardKeyCode = KeyCode.D;

        // Whether the Pacboy is controllable
        // Should be false: when e.g. tutorial paused, tutorial ended
        private bool _controllable;

        // Whether the player is in Third Person View
        private bool _inThirdPersonView;

        private float _mouseX;
        private float _rotationY;

        // Half dimensions of the player's collider (shrunk slightly)
        private Vector3 _boxHalfExtents;

        // Speed buff logic variables
        private float _speedBuffTimer; // The timer of a speed buff
        private readonly float _speedBuffDuration = 3.0f; // Duration that a speed buff lasts (Slightly smaller than normal game)
        private bool _speedBuffInEffect; // Whether currently there is a speed buff or not

        // START FUNCTION
        private void Start() {
            // Get slightly reduced box size for collision tolerance
            _boxHalfExtents = GetComponent<BoxCollider>().bounds.extents * 0.9f;

            // Set to normal speed
            _pacboyMoveSpeed = _pacboyNormalMoveSpeed;

            EnableMovement(); // Enable Pacboy movement when tutorial starts
        }


        // UPDATE FUNCTION
        private void Update() {
            if (!_controllable) return;

            Move(); // Pacboy movement operation

            // Check if currently there is a speed buff
            // If there is, keep updating the timer until it reaches the duration
            if (_speedBuffInEffect) {
                _speedBuffTimer += Time.deltaTime;

                if (_speedBuffTimer >= _speedBuffDuration) {
                    // Buff time is over
                    // Set the speed back to normal speed
                    _pacboyMoveSpeed = _pacboyNormalMoveSpeed;

                    // Reset timer and status
                    _speedBuffTimer = 0f;
                    _speedBuffInEffect = false;
                }
            }
        }

        /**
         * Handles player movement with wall collision detection and sliding logic.
         * Prevents moving into walls while allowing smooth sliding along them.
         * In TPV, also rotates Pacboy to face the movement direction.
         */
        private void Move() {
            float h = 0f, v = 0f;

            // Capture directional input
            if (Input.GetKey(ForwardKeyCode)) v += 1f;
            if (Input.GetKey(BackwardKeyCode)) v -= 1f;
            if (Input.GetKey(RightwardKeyCode)) h += 1f;
            if (Input.GetKey(LeftwardKeyCode)) h -= 1f;

            Vector3 inputDir = new Vector3(h, 0, v);

            // Don't move if input is too small
            if (inputDir.sqrMagnitude < 0.01f) return;

            inputDir.Normalize();

            // Get the horizontal forward and right directions from the camera
            Transform cam = Camera.main.transform;
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Compute the world movement direction relative to camera
            Vector3 moveDir = inputDir.z * camForward + inputDir.x * camRight;
            moveDir.Normalize();

            // --- Collision Detection and Sliding Logic ---
            float moveDistance = _pacboyMoveSpeed * Time.deltaTime;

            // Raise the cast origin slightly so it aligns with Pacboy's body center
            Vector3 castOrigin = transform.position + Vector3.up * _boxHalfExtents.y;

            // Cast a box in the move direction to detect any nearby obstacles
            RaycastHit[] hits = Physics.BoxCastAll(
                castOrigin,
                _boxHalfExtents,
                moveDir,
                Quaternion.identity,
                moveDistance
            );

            // Filter only wall hits
            RaycastHit[] wallHits = hits.Where(hit => hit.collider.CompareTag("Wall")).ToArray();

            // If there's at least one wall, find the closest one
            if (wallHits.Length > 0) {
                RaycastHit wallHit = wallHits.OrderBy(hit => hit.distance).First();

                // Slide direction = project moveDir onto plane perpendicular to wall
                Vector3 slideDir = Vector3.ProjectOnPlane(moveDir, wallHit.normal).normalized;

                // Decide whether the sliding direction is safe or not
                bool blocked = IsDangerousSlide(castOrigin, slideDir, _boxHalfExtents, moveDistance, wallHit);

                if (!blocked) {
                    // Slide along the wall if it's a safe direction
                    transform.position += slideDir * moveDistance;
                } else {
                    // Block movement completely
                    return;
                }
            } else {
                // No wall ahead, move freely
                transform.position += moveDir * moveDistance;
            }

            // In TPV, rotate Pacboy to face the movement direction
            if (_inThirdPersonView) {
                if (moveDir.sqrMagnitude > 0.01f) {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                        _pacboyRotateSpeed * Time.deltaTime);
                }
            }
        }

        /**
         * Determines if sliding along a wall is unsafe based on the angle and distance.
         * Blocks sliding when the direction would likely cause the player to clip through the wall,
         * or if there’s still a wall directly in the slide direction.
         * Returns true if the slide direction is dangerous or blocked, otherwise false.
         *
         * PARAMS
         * origin - Start point of the box cast (usually just above the player's center)
         * direction - The intended slide direction (projected against the wall normal)
         * halfExtents - Half dimensions of the player's collider (shrunk slightly)
         * distance - Distance Pacboy wants to move in this frame
         * wallHit - The original wall hit data, used to calculate angles
         */
        private bool IsDangerousSlide(Vector3 origin, Vector3 direction, Vector3 halfExtents, float distance,
            RaycastHit wallHit) {
            // Compute the angle between the slide direction and the wall's normal
            float dot = Vector3.Dot(direction.normalized, wallHit.normal);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            // If the angle is too small, the direction is almost pushing into the wall → unsafe
            if (angle < 10f) return true;

            // Perform another box cast in the slide direction to check for direct collision
            if (Physics.BoxCast(origin, halfExtents, direction, out RaycastHit hit, Quaternion.identity, distance)) {
                if (hit.collider.CompareTag("Wall")) return true;
            }

            // Slide direction is safe and not colliding
            return false;
        }

        /**
         * Temporarily sets a speed for the Pacboy.
         * Used when Fast Wheel / Slow Wheel is triggered.
         */
        public void SetSpeedBuff(float buffSpeed) {
            // Set speed
            _pacboyMoveSpeed = buffSpeed;

            // Reset timer and statue
            _speedBuffTimer = 0f;
            _speedBuffInEffect = true;
        }

        /**
         * Sets the current view mode.
         */
        public void SetViewMode(bool thirdPerson) {
            _inThirdPersonView = thirdPerson;
        }

        /**
         * Allows the player to control the movement of the Pacboy.
         */
        public void EnableMovement() {
            // Set cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _controllable = true;
        }

        /**
         * Stops the player from controlling the movement of the Pacboy.
         */
        public void DisableMovement() {
            // Set cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _controllable = false;
        }
    }
}