using SP_Meta;
using UnityEngine;

namespace FPSController
{
    public class FPSController : MonoBehaviour
    {
        public float movementForce;
        public float movementForceCrouch;
        public float movementMaxSpeed;
        public float accelerationSpeed;
        public float jumpForce;
        public float airControl;
        public float interactionRange;

        public AudioSource footStep1;
        public AudioSource footStep2;
        public AudioSource footLand;

        private CharacterController player;
        private GameObject playerCamera;

        private float playerBaseHeight;
        private float playerCrouchHeight;

        private float footstepAlternationStep;
        private int footstepAlternation = 1;

        private float xAxis_input;
        private float yAxis_input;

        private float airTime = 0.0f;
        private readonly float swapWeaponCooldownTime = 1;
        private float swapWeaponCooldown = 0;

        private bool inAir;
        private bool crouchHold = false; //Prevent players from releasing from crouch when insuficiante space above head
        private bool jump_input;
        private bool crouch_input;

        private Vector3 cmd_Move;
        private Vector3 velocity;

        private readonly float terminalVelocity = Physics.gravity.y * 2;

        private void Start()
        {
            player = transform.parent.GetComponent<CharacterController>();
            playerCamera = transform.parent.Find("Camera").gameObject;
            velocity = Vector3.zero;

            playerBaseHeight = player.height;
            playerCrouchHeight = playerBaseHeight * 0.5f;
        }

        private void Update()
        {
            jump_input = Input.GetKey(KeyCode.Space);
            crouch_input = Input.GetKey(KeyCode.LeftControl);
        }

        private void FixedUpdate()
        {
            //Movement
            UpdatePlayerPosition();
            UpdateVelocity();
            UpdateCrouch();
            UpdateCooldown();
        }

        #region Updates

        private void UpdatePlayerPosition()
        {
            //Get Input
            xAxis_input = Input.GetAxisRaw("Horizontal");
            yAxis_input = Input.GetAxisRaw("Vertical");

            cmd_Move = new Vector3(xAxis_input, 0, yAxis_input);

            if (cmd_Move.magnitude > 1)
                cmd_Move = cmd_Move.normalized;
            cmd_Move = player.transform.TransformVector(cmd_Move);

            player.Move(velocity * Time.deltaTime);
        }

        private void UpdateVelocity()
        {
            Vector3 cmd_Velocity;

            if (crouchHold)
                cmd_Velocity = cmd_Move * movementForceCrouch;
            else
                cmd_Velocity = cmd_Move * movementForce;

            if (cmd_Velocity.magnitude > movementMaxSpeed)
                cmd_Velocity = cmd_Velocity.normalized * movementMaxSpeed;

            if (player.isGrounded)
            {
                velocity.x = Mathf.Lerp(velocity.x, cmd_Velocity.x, Time.deltaTime * accelerationSpeed * GameSettings.PHYSICS_ACCELERATION_FACTOR);
                velocity.z = Mathf.Lerp(velocity.z, cmd_Velocity.z, Time.deltaTime * accelerationSpeed * GameSettings.PHYSICS_ACCELERATION_FACTOR);
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, (velocity.x * (1 - airControl)) + (cmd_Velocity.x * airControl), Time.deltaTime * accelerationSpeed * GameSettings.PHYSICS_ACCELERATION_FACTOR);
                velocity.z = Mathf.Lerp(velocity.z, (velocity.z * (1 - airControl)) + (cmd_Velocity.z * airControl), Time.deltaTime * accelerationSpeed * GameSettings.PHYSICS_ACCELERATION_FACTOR);
            }

            if (jump_input && player.isGrounded && !crouch_input)
            {
                velocity.y = jumpForce;
            }
            else if (!player.isGrounded)
            {
                velocity.y = Mathf.SmoothDamp(velocity.y, terminalVelocity, ref velocity.y, -Physics.gravity.y * Time.deltaTime * GameSettings.PHYSICS_TICK_RATE);
            }
        }

        private void UpdateFootstep()
        {
            if (player.isGrounded)
            {
                Ray ray = new Ray(player.transform.position, -player.transform.up);

                //Surface Check
                if (Physics.Raycast(ray, out RaycastHit _, player.height / 2))
                {
                    //Checkpoint between isGrounded and inAir indicates landing
                    if (inAir)
                    {
                        inAir = false;
                        if (footLand != null && airTime >= 0.35)
                            footLand.Play();
                        airTime = 0.0f;
                    }
                }

                //Play Footsteps
                if (Mathf.Abs(xAxis_input) + Mathf.Abs(yAxis_input) > 0)
                {
                    float velocityGround = Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z);

                    if (footstepAlternation == 1)
                    {
                        if (footstepAlternationStep <= footstepAlternation - 0.1f)
                            footstepAlternationStep = Mathf.Lerp(footstepAlternationStep, footstepAlternation, Time.deltaTime * velocityGround);
                        else if (footStep1 != null)
                        {
                            footStep1.Play();
                            footstepAlternation *= -1;
                        }
                    }
                    else if (footstepAlternation == -1)
                    {
                        if (footstepAlternationStep >= footstepAlternation + 0.1f)
                            footstepAlternationStep = Mathf.Lerp(footstepAlternationStep, footstepAlternation, Time.deltaTime * velocityGround);
                        else if (footStep2 != null)
                        {
                            footStep2.Play();
                            footstepAlternation *= -1;
                        }
                    }
                }
                else
                {
                    footstepAlternation = 1;
                    footstepAlternationStep = 0;
                }
            }
        }

        private void UpdateCrouch()
        {
            if (crouch_input)
            {
                crouchHold = true;
            }
            else if (!crouchHold) //No reason to check further if crouching is not in use
            {
                player.height = playerBaseHeight; return;
            }

            var ray = new Ray(player.transform.position + new Vector3(0, playerCrouchHeight, 0), player.transform.up);

            if (crouch_input || (crouchHold && Physics.Raycast(ray, out RaycastHit _, playerBaseHeight)))
            {
                player.height = playerCrouchHeight;
            }
            else
            {
                player.height = playerBaseHeight;
                crouchHold = false;
            }
        }

        private void UpdateCooldown()
        {
            if (swapWeaponCooldown < swapWeaponCooldownTime)
            {
                swapWeaponCooldown += Time.deltaTime;
            }
        }

        #endregion Updates

        #region Meta

        /// <summary>
        /// Returns the raycast hit location of the camera center, helps with aiming
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPointOfFocus()
        {
            var hasHit = Physics.Raycast(new Ray(playerCamera.transform.position, playerCamera.transform.forward), out RaycastHit hit, 99999);

            if (hasHit)
            {
                return hit.point;
            }
            return playerCamera.transform.forward * 99999; //Practically infinite distance hit
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }

        #endregion Meta
    }
}