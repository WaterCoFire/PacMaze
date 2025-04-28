using System.Linq;
using UnityEngine;

namespace Entity.Pacboy {
    /**
     * Manages the movement of Pacboy:
     * (Controlled by the player)
     * - Go Forward / Backward / Leftward / Rightward
     * - Rotate (controlled by mouse dragging)
     */
    public class PacboyMovement : MonoBehaviour {
        // Move speed and rotate speed
        private float _pacboyMoveSpeed;
        private readonly float _pacboyRotateSpeed = 5.0f;

        // Pacboy's normal speed
        private float _pacboyNormalMoveSpeed = 5.0f;

        // Key code for moving (WASD by default, it can be customized in Setting)
        private KeyCode _forwardKeyCode;
        private KeyCode _backwardKeyCode;
        private KeyCode _leftwardKeyCode;
        private KeyCode _rightwardKeyCode;

        // Status indicating if the Pacboy is controllable
        // Should be false: when e.g. game paused, game ended
        private bool _controllable;

        // Status indicating if the player is in third person view
        private bool _inThirdPersonView;

        private float _mouseX;
        private float _rotationY;

        // Half dimensions of the player's collider (shrunk slightly)
        private Vector3 _boxHalfExtents;

        // Speed buff logic variables
        private float _speedBuffTimer; // The timer of a speed buff
        private readonly float _speedBuffDuration = 5.0f; // Duration that a speed buff lasts
        private bool _speedBuffInEffect; // Status telling whether currently there is a speed buff or not

        // Event logic - Crazy Party active/disactive (Double the speed)
        private bool _crazyParty;
        
        // START FUNCTION
        private void Start() {
            Debug.Log("PacboyMovement START");

            // Get the keycode set for moving operations
            _forwardKeyCode = GetKeyCode("ForwardKeyCode", KeyCode.W);
            _backwardKeyCode = GetKeyCode("BackwardKeyCode", KeyCode.S);
            _leftwardKeyCode = GetKeyCode("LeftwardKeyCode", KeyCode.A);
            _rightwardKeyCode = GetKeyCode("RightwardKeyCode", KeyCode.D);

            Cursor.lockState = CursorLockMode.Locked; // Lock mouse
            Cursor.visible = false;

            // Get slightly reduced box size for collision tolerance
            _boxHalfExtents = GetComponent<BoxCollider>().bounds.extents * 0.9f;

            // Set to normal speed
            _pacboyMoveSpeed = _pacboyNormalMoveSpeed;

            _crazyParty = false; // No Crazy Party event by default
            _controllable = true;
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
            if (Input.GetKey(_forwardKeyCode)) v += 1f;
            if (Input.GetKey(_backwardKeyCode)) v -= 1f;
            if (Input.GetKey(_rightwardKeyCode)) h += 1f;
            if (Input.GetKey(_leftwardKeyCode)) h -= 1f;

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
         * Allows the player to control the movement of the Pacboy.
         */
        public void EnableMovement() {
            // Update KeyCode
            _forwardKeyCode = GetKeyCode("ForwardKeyCode", KeyCode.W);
            _backwardKeyCode = GetKeyCode("BackwardKeyCode", KeyCode.S);
            _leftwardKeyCode = GetKeyCode("LeftwardKeyCode", KeyCode.A);
            _rightwardKeyCode = GetKeyCode("RightwardKeyCode", KeyCode.D);
            
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
        
        /**
         * Sets the status of Crazy Party.
         * Called by EventManager via GhostronManager when the Crazy Party should be on/off.
         */
        public void SetCrazyParty(bool on) {
            if (on) {
                if (_crazyParty) {
                    Debug.LogError("Error: Crazy Party is already active!");
                    return;
                }

                // Double the speed
                // For both the current speed and the normal speed
                _crazyParty = true;
                _pacboyMoveSpeed *= 2;
                _pacboyNormalMoveSpeed *= 2;
            } else {
                if (!_crazyParty) {
                    Debug.LogError("Error: Crazy Party is not active!");
                    return;
                }
                
                // Set back to normal speed
                // For both the current speed and the normal speed
                _crazyParty = false;
                _pacboyMoveSpeed /= 2;
                _pacboyNormalMoveSpeed /= 2;
            }
        }
    }
}