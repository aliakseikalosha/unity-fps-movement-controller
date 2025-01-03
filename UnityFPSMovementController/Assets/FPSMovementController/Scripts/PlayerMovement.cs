﻿using UnityEngine;
// By B0N3head 
// All yours, use this script however you see fit, feel free to give credit if you want

namespace FPSMovementController
{
    [AddComponentMenu("FPSMovementController/Player")]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Camera Settings")]

        [Tooltip("Lock the cursor to the game screen on play")]
        [SerializeField] bool lockCursor = true;
        [Tooltip("Clamp the camera angle (Stop the camera form \"snapping its neck\")")]
        [SerializeField] Vector2 clampInDegrees = new Vector2(360f, 180f);
        [Tooltip("The mouse sensitivity, both x and y")]
        [SerializeField] Vector2 sensitivity = new Vector2(2f, 2f);
        [Tooltip("Smoothing of the mouse movement (Try with and without)")]
        [SerializeField] Vector2 smoothing = new Vector2(1.5f, 1.5f);
        [Tooltip("Needs to be the same name as your main cam")]
        [SerializeField] string cameraName = "Camera";

        //----------------------------------------------------
        [Space]
        [Header("Movement Settings")]
        [Tooltip("Max walk speed")]
        [SerializeField] float walkMoveSpeed = 7.5f;
        [Tooltip("Max sprint speed")]
        [SerializeField] float sprintMoveSpeed = 11f;
        [Tooltip("Max jump speed")]
        [SerializeField] float jumpMoveSpeed = 6f;
        [Tooltip("Max crouch speed")]
        [SerializeField] float crouchMoveSpeed = 4f;
        //----------------------------------------------------
        [Header("Crouch Settings")]

        [Tooltip("How long it takes to crouch")]
        [SerializeField] float crouchDownSpeed = 0.2f;
        [Tooltip("How tall the character is when they crouch")]
        [SerializeField] float crouchHeight = 0.68f; //change for how large you want when crouching
        [Tooltip("How tall the character is when they stand")]
        [SerializeField] float standingHeight = 1f;
        [Tooltip("Lerp between crouching and standing")]
        [SerializeField] bool smoothCrouch = true;
        [Tooltip("Can you crouch while in the air")]
        [SerializeField] bool jumpCrouching = true;
        //----------------------------------------------------
        [Header("Jump Settings")]
        [Tooltip("Initial jump force")]
        [SerializeField] float jumpForce = 110f;
        [Tooltip("Continuous jump force")]
        [SerializeField] float jumpAccel = 10f;
        [Tooltip("Max jump up time")]
        [SerializeField] float jumpTime = 0.4f;
        [Tooltip("How long you have to jump after leaving a ledge (seconds)")]
        [SerializeField] float coyoteTime = 0.2f;
        [Tooltip("How long I should buffer your jump input for (seconds)")]
        [SerializeField] float jumpBuffer = 0.1f;
        [Tooltip("How long do I have to wait before I can jump again")]
        [SerializeField] float jumpCooldown = 0.6f;
        [Tooltip("Fall quicker")]
        [SerializeField] float extraGravity = 0.1f;
        [Tooltip("The tag that will be considered the ground")]
        [SerializeField] string groundTag = Constants.GroundTag;
        //----------------------------------------------------
        [Header("Input")]
        [SerializeField] UserInputProxy userInput;
        //----------------------------------------------------
        [Space]
        [Header("Debug Info")]

        [Tooltip("Are we on the ground?")]
        [SerializeField] bool areWeGrounded = true;
        [Tooltip("Are we crouching?")]
        [SerializeField] bool areWeCrouching = false;
        [Tooltip("The current speed I should be moving at")]
        [SerializeField] float currentSpeed;

        //----------------------------------------------------
        // Reference vars (These are the vars used in calculations, they don't need to be set by the user)
        private Rigidbody rb;
        private GameObject cam;
        Vector3 moveInput = Vector3.zero;
        Vector2 _mouseAbsolute, _smoothMouse, targetDirection, targetCharacterDirection;
        private float startJumpTime, endJumpTime;
        private Counter coyoteTimeCounter, jumpBufferCounter, jumpAllowCounter;
        private bool wantingToJump = false, wantingToCrouch = false, wantingToSprint = false;

        private void Awake()
        {
            // Just set rb to the rigidbody of the GameObject containing this script
            rb = gameObject.GetComponent<Rigidbody>();
            // Try find our camera amongst the child objects
            cam = gameObject.transform.Find(cameraName).gameObject;
            // Set the currentSpeed to walking as no keys should be pressed yet
            currentSpeed = walkMoveSpeed;

            // Set target direction to the camera's initial orientation.
            targetDirection = transform.localRotation.eulerAngles;
            // Set target direction for the character body to its initial state.
            targetCharacterDirection = transform.localRotation.eulerAngles;
            userInput.OnToggleCursor += ToggleCursorLock;

            // Coyote timer (When the player leaves the ground, start counting down from the set value coyoteTime)
            // This allows players to jump late. After they have left 
            coyoteTimeCounter = new Counter(() => areWeGrounded, coyoteTime);
            jumpBufferCounter = new Counter(() => !wantingToJump, jumpBuffer);
            jumpAllowCounter = new Counter(() => false, jumpCooldown);
        }
        /// <summary>
        /// Unity update run each frame
        /// </summary>
        private void Update()
        {
            // Update the camera pos
            CameraUpdate();

            // Move all input to Update(), then use given input on FixedUpdate()

            // WSAD movement
            moveInput = new Vector3(userInput.Move.x, 0, userInput.Move.y);
            // Jump key
            wantingToJump = userInput.Jump;
            // Crouch key
            wantingToCrouch = userInput.Crouch;
            // Sprint key
            wantingToSprint = userInput.Sprint;
        }
        /// <summary>
        ///  Unity Physic run each physics frame
        /// </summary>
        private void FixedUpdate()
        {
            // Lock cursor handling
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;

            // Double check if we are on the ground or not (Changes current speed if true)
            // --- QUICK EXPLINATION --- 
            // transform.position.y - transform.localScale.y + 0.1f
            // This puts the start of the ray 0.1f above the bottom of the player
            // We then shoot a ray 0.15f down, this exists the player with 0.5f to hit objects
            // Removing this +- of 0.1f and having it shoot directly under the player can skip the ground as sometimes the capsules bottom clips through the ground
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y + 0.1f, transform.position.z), Vector3.down, 0.15f))
            {
                HandleHitGround();
            }
            else
            {
                areWeGrounded = false;
            }

            // Sprinting
            if (areWeGrounded && !areWeCrouching)
            {
                currentSpeed = wantingToSprint ? sprintMoveSpeed : walkMoveSpeed;
            }

            // Crouching 
            // Can be simplified to Crouch((wantingToCrouch && jumpCrouching)); though the bellow is more readable
            Crouch(wantingToCrouch && jumpCrouching);
            UpdateTimers();

            // If the coyote timer has not run out and our jump buffer has not run out and we our cool down (canJump) is now over
            if ((coyoteTimeCounter.Running || areWeGrounded) && jumpBufferCounter.Running && jumpAllowCounter.Ended && wantingToJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                areWeGrounded = false;
                currentSpeed = jumpMoveSpeed;
                endJumpTime = Time.time + jumpTime;

                jumpAllowCounter.Set();
            }
            else if (wantingToJump && !areWeGrounded && endJumpTime > Time.time)
            {
                // Hold down space for a further jump (until the timer runs out)
                rb.AddForce(Vector3.up * jumpAccel, ForceMode.Acceleration);
            }

            // WSAD movement
            moveInput = moveInput.normalized;
            Vector3 forwardVel = transform.forward * currentSpeed * moveInput.z;
            Vector3 horizontalVel = transform.right * currentSpeed * moveInput.x;
            rb.linearVelocity = horizontalVel + forwardVel + new Vector3(0, rb.linearVelocity.y, 0);

            //Extra gravity for more nicer jumping
            rb.AddForce(new Vector3(0, -extraGravity, 0), ForceMode.Impulse);
        }

        private void UpdateTimers()
        {
            coyoteTimeCounter.Update();
            jumpBufferCounter.Update();
        }


        public void ToggleCursorLock()
        {
            lockCursor = !lockCursor;
        }
        /// <summary>
        /// Update Camera rotation
        /// </summary>
        public void CameraUpdate()
        {
            // Allow the script to clamp based on a desired target value.
            var targetOrientation = Quaternion.Euler(targetDirection);
            var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

            // Get raw mouse input for a cleaner reading on more sensitive mice.
            var mouseDelta = userInput.Look;

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            _mouseAbsolute += _smoothMouse;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
            {
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
            }

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
            {
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
            }

            cam.transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
            transform.localRotation = yRotation * targetCharacterOrientation;
        }

        /// <summary>
        /// Handle Crouch input
        /// </summary>
        /// <param name="crouch">set to true if crouching</param>
        private void Crouch(bool crouch)
        {
            areWeCrouching = crouch;

            if (crouch)
            {
                // If the player is crouching
                currentSpeed = crouchMoveSpeed;

                if (smoothCrouch)
                {
                    transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, crouchHeight, crouchDownSpeed), transform.localScale.z);
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - crouchHeight, transform.position.z), crouchDownSpeed);
                }
                else if (transform.localScale != new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z))
                {
                    transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, transform.position.y - crouchHeight / 2, transform.position.z);
                }
            }
            else
            {
                // If the player is standing
                if (smoothCrouch)
                {
                    transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, standingHeight, crouchDownSpeed), transform.localScale.z);
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - standingHeight / 2, transform.position.z), crouchDownSpeed);
                }
                else if (transform.localScale != new Vector3(transform.localScale.x, standingHeight, transform.localScale.z))
                {
                    transform.localScale = new Vector3(transform.localScale.x, standingHeight, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, transform.position.y + standingHeight / 2, transform.position.z);
                }
            }
        }

        /// <summary>
        /// Unity method that is called when player <c>Rigidbody</c> collided with anything.<br/>
        /// Current use is:<br/>
        /// Ground check make sure whatever you want to be the ground in your game matches the tag set in the script<br/>
        /// </summary>
        /// <param name="collisionObject">Object with information about collision</param>
        private void OnCollisionEnter(Collision collisionObject)
        {
            if (collisionObject.gameObject.CompareTag(groundTag))
            {
                HandleHitGround();
            }
        }

        /// <summary>
        /// Reset state after player step on the ground  
        /// </summary>
        private void HandleHitGround()
        {
            currentSpeed = areWeCrouching ? crouchMoveSpeed : walkMoveSpeed;

            areWeGrounded = true;
        }

        /// <summary>
        /// Used to setup all components and initial setting
        /// </summary>
        public void SetupCharacter()
        {
            gameObject.tag = Constants.PlayerTag;
            if (!gameObject.GetComponent<Rigidbody>())
            {
                Rigidbody rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                rb.mass = 10;
            }
            else
            {
                Debug.Log("Rigidbody already exists");
            }
            if (userInput == null)
            {
                if ((userInput = gameObject.GetComponentInChildren<UserInputProxy>()) == null)
                {
                    userInput = gameObject.AddComponent<OldUserInput>();
                    Debug.LogWarning($"Add Old User Input to player, I using new one you will need to add a {typeof(UserInputProxy)} implementation and set it as {nameof(userInput)}.");
                }
            }
            if (!gameObject.transform.Find("Camera"))
            {
                Vector3 old = transform.position;
                gameObject.transform.position = new Vector3(0, -0.8f, 0);
                GameObject go = new GameObject("Camera");
                go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
                go.transform.rotation = new Quaternion(0, 0, 0, 0);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.parent = transform;
                gameObject.transform.position = old;
                Debug.Log("Camera created");
            }
            else
            {
                Debug.Log("Camera already exists");
            }
        }
    }

}