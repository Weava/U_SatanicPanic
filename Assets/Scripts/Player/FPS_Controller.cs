using SP_Meta;
using UnityEngine;

namespace FPS_Controller
{
    public class FPS_Controller : MonoBehaviour
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
        private Inventory inventory;
        private GameObject camera;

        private float playerBaseHeight;
        private float playerCrouchHeight;

        private float footstepAlternationStep;
        private int footstepAlternation = 1;

        private float xAxis_input;
        private float yAxis_input;

        private float airTime = 0.0f;
        private float interactCooldownTime = 0.5f;
        private float interactCooldown = 0;
        private float swapWeaponCooldownTime = 1;
        private float swapWeaponCooldown = 0;

        private bool inAir;
        private bool crouchHold = false; //Prevent players from releasing from crouch when insuficiante space above head
        private bool jump_input;
        private bool crouch_input;

        private bool playerShooting_input = false;
        private bool playerInteraction_input = false;
        private bool playerDrop_input = false;
        private bool playerSwapWeapon_input = false;

        private Vector3 cmd_Move;
        private Vector3 velocity;
        private Vector3 player_dir;

        private float terminalVelocity = Physics.gravity.y * 2;

        private void Start()
        {
            player = transform.parent.GetComponent<CharacterController>();
            inventory = transform.GetComponentInChildren<Inventory>();
            camera = transform.parent.Find("Camera").gameObject;
            velocity = Vector3.zero;

            playerBaseHeight = player.height;
            playerCrouchHeight = playerBaseHeight * 0.5f;
        }

        private void Update()
        {
            jump_input = Input.GetKey(KeyCode.Space);
            crouch_input = Input.GetKey(KeyCode.LeftControl);
            playerShooting_input = Input.GetKey(KeyCode.Mouse0);
            playerInteraction_input = Input.GetKeyDown(KeyCode.E);
            playerDrop_input = Input.GetKeyDown(KeyCode.C);
            playerSwapWeapon_input = Input.GetKeyDown(KeyCode.Q);
        }

        private void FixedUpdate()
        {
            //Movement
            UpdatePlayerPosition();
            //UpdateFootstep();
            UpdateVelocity();
            UpdateCrouch();
            UpdateCooldown();

            //Weapons / Interaction
            //UpdateWeapon();
            if (playerInteraction_input) UpdateInteraction();
        }

        #region Updates

        //void UpdateWeapon()
        //{
        //    inventory.SetWeaponOffset(camera.transform.position + camera.transform.forward, camera.transform.rotation);

        //    if (playerDrop_input) { inventory.Unequip(); }

        //    if(playerSwapWeapon_input && swapWeaponCooldown >= swapWeaponCooldownTime) { swapWeaponCooldown = 0; inventory.SwapWeapon();  }

        //    if (playerShooting_input) { inventory.FireCurrentWeapon(); }
        //}

        private void UpdateInteraction()
        {
            RaycastHit hit;
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);

            if (Physics.Raycast(ray, out hit, interactionRange, (1 << 8))/*8 = Interaction layer*/)
            {
                //TODO: Bring this back
                //var interactionInstance = hit.transform.gameObject.GetComponent<Trigger>();

                //if(interactionInstance != null && interactionInstance.isInteractable)
                //{
                //    interactionInstance.Evoke();
                //}
            } //else if(Physics.Raycast(ray, out hit, interactionRange, (1 << 9))/*9 = Weapon layer*/)
            //{
            //    var weaponInstance = hit.transform.gameObject.GetComponent<Weapon_Base>();

            //    if(weaponInstance != null)
            //    {
            //        inventory.Equip(weaponInstance.Pickup());
            //    }
            //}
        }

        private void UpdatePlayerPosition()
        {
            //Get Input
            xAxis_input = Input.GetAxisRaw("Horizontal");
            yAxis_input = Input.GetAxisRaw("Vertical");

            cmd_Move = new Vector3(xAxis_input, 0, yAxis_input);

            if (cmd_Move.magnitude > 1)
                cmd_Move = cmd_Move.normalized;

            player_dir = player.transform.forward;
            cmd_Move = player.transform.TransformVector(cmd_Move);

            if (!player.isGrounded)
            {
                inAir = true;
                airTime += Time.deltaTime;
            }

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
            Collider surface;

            if (player.isGrounded)
            {
                RaycastHit hit;
                Ray ray = new Ray(player.transform.position, -player.transform.up);

                //Surface Check
                if (Physics.Raycast(ray, out hit, player.height / 2))
                {
                    if (hit.transform.GetComponent<Collider>() != null)
                    {
                        surface = hit.transform.GetComponent<Collider>();

                        if (surface.material.name.Contains("Wood"))
                        {
                            UpdateFootstepSounds("Surface_Wood");
                        }
                        else if (surface.material.name.Contains("Metal"))
                        {
                            UpdateFootstepSounds("Surface_Metal");
                        }
                        else
                        {
                            UpdateFootstepSounds("Surface_Generic");
                        }
                    }

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
            { crouchHold = true; }
            else if (!crouchHold) //No reason to check further if crouching is not in use
            { player.height = playerBaseHeight; return; }

            var raycastHit = new RaycastHit();
            var ray = new Ray(player.transform.position + new Vector3(0, playerCrouchHeight, 0), player.transform.up);

            if (crouch_input || (crouchHold && Physics.Raycast(ray, out raycastHit, playerBaseHeight)))
            {
                player.height = playerCrouchHeight;
            }
            else
            {
                player.height = playerBaseHeight;
                crouchHold = false;
            }
        }

        private void UpdateFootstepSounds(string Name)
        {
            //footStep1 = GameObject.Find(Name).GetComponent<Surface>().surfaceSound[0];
            //footStep2 = GameObject.Find(Name).GetComponent<Surface>().surfaceSound[1];
            //footLand = GameObject.Find(Name).GetComponent<Surface>().surfaceSound[2];
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
            RaycastHit hit;
            var hasHit = Physics.Raycast(new Ray(camera.transform.position, camera.transform.forward), out hit, 99999);

            if (hasHit)
            {
                return hit.point;
            }
            return camera.transform.forward * 99999; //Practically infinite distance hit
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }

        #endregion Meta
    }
}