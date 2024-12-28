using UnityEngine;
namespace FPSMovementController
{
    public class OldUserInput : UserInputProxy
    {
        //----------------------------------------------------
        [Space]
        [Header("Keyboard Settings")]
        [Tooltip("The key used to jump")]
        [SerializeField] KeyCode jump = KeyCode.Space;
        [Tooltip("The key used to sprint")]
        [SerializeField] KeyCode sprint = KeyCode.LeftShift;
        [Tooltip("The key used to crouch")]
        [SerializeField] KeyCode crouch = KeyCode.Z;
        [Tooltip("The key used to toggle the cursor")]
        [SerializeField] KeyCode lockToggle = KeyCode.Q;

        public override Vector2 Move { get; protected set; } = Vector2.zero;

        public override Vector2 Look { get; protected set; } = Vector2.zero;

        public override bool Jump { get; protected set; } = false;

        public override bool Crouch { get; protected set; } = false;
        public override bool IsSprint { get; protected set; } = false;

        public void Update()
        {

            // Move all input to Update(), then use given input on FixedUpdate()
            Look = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            // WSAD movement
            Move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // Jump key
            Jump = Input.GetKey(jump);
            // Crouch key
            Crouch = Input.GetKey(crouch);
            // Sprint key
            IsSprint = Input.GetKey(sprint);

            // Mouse lock toggle (KeyDown only fires once)
            if (Input.GetKeyDown(lockToggle))
            {
                RaiseOnToggleCursor();
            }
        }

    }
}
