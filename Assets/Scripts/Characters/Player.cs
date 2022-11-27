using System.Collections;
using Characters.BaseStats;
using Characters.Upgrades;
using Cinemachine;
using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Character
    {
        // -------------------------- Serialized Properties  -------------------------- //
        
        [Header("Player Controls")]
        public float mouseSensitivity;
        
        [Tooltip("Limits the max rotation of the cam")]
        [SerializeField, Range(0,89.9f)] private float viewLockY;
        
        [SerializeField] private float recoilResetTime = 0.4f;
        
        [SerializeField] private Transform head;

        [field: Header("Player Only")]
        [field: SerializeField] public float WallSpeed { get; private set; }

        // -------------------------- Primitives  -------------------------- //
        private Rigidbody rb;
        private PlayerControls controls;
        private CinemachineVirtualCamera cmv;
        //private readonly Weapon[] weapons = new Weapon[3];
        
        private Vector2 intendedDirection;
        private Vector2 mouseDir;
        private Vector3 wallForward;

        private RaycastHit leftWallCast;
        private RaycastHit rightWallCast;
        
        private const float WallDist = 0.75f;
        private float wallRunTime;
        private float curRecoilTime;
        //float invicibilityTime = 0.05f;
        //float invincTimer;
        
        private bool onLeftWall;
        private bool onRightWall;
        private bool isWallRunning;
        private bool tryingToJump;
        private bool usingLeftHand; //TODO: for animations

        private int wallRunScalar; // Used to help the player run backwards and rotate the cam
        private int weaponIndex;

        //[SerializeField] int weaponUnlockCount;
        
        [Header("Audio")]
        [SerializeField] AudioClip weaponChangeSound;
        [SerializeField] AudioClip weaponUpgradeSound;
        [SerializeField] AudioClip weaponUpgradeFailSound;

        private PlayerUI ui;
        private ShootingCapability shootingCapability;
        
        // -------------------------- Logged Stats  -------------------------- //
        private float longestWallRun;
        private float totalTimeWallRunning;
        
        #region Overrides

        protected override void Awake()
        {
            base.Awake();
            //Get Components
            rb = GetComponent<Rigidbody>();
            ui = GetComponent<PlayerUI>();
            cmv = transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();

            //Handle Controls
            controls = new PlayerControls();
            controls.InGame.Enable();
            controls.InGame.Jump.started += _ =>  tryingToJump = true;
            controls.InGame.Jump.canceled += _ =>  tryingToJump = false;
            controls.InGame.Shoot.started += _ =>
            {
                if (canAttack)
                    shootingCapability.Weapons[weaponIndex].tryingToShoot = true;
            };
            controls.InGame.Shoot.canceled += _ => shootingCapability.Weapons[weaponIndex].tryingToShoot = false;

            controls.InGame.WeaponToggle1.started += _ => ToggleWeaponSlot(0);
            controls.InGame.WeaponToggle2.started += _ => ToggleWeaponSlot(1);
            controls.InGame.WeaponToggle3.started += _ => ToggleWeaponSlot(2); 
            
            controls.InGame.LeftRight.performed += ctx => directionVector.z = ctx.ReadValue<float>();
            controls.InGame.ForwardBack.performed += ctx => directionVector.x = ctx.ReadValue<float>();
            controls.InGame.CameraX.performed += ctx => mouseDir.y = ctx.ReadValue<float>();
            controls.InGame.CameraY.performed += ctx => mouseDir.x = ctx.ReadValue<float>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            ui.SetHealth(stats.MaxHealth, curHealth);
            
            shootingCapability = GetComponent<ShootingCapability>();
            shootingCapability.Init(this);
            for (int i = 0; i < shootingCapability.Weapons.Length; ++i)
            {
                ui.SetWeapon(i, shootingCapability.Weapons[i].GetStats<WeaponStatsSo>().Sprite);
            }
            
            ui.SetCurrentWeapon(0,0);
        }

        
    
        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if(tryingToJump && CanJump())
                Jump();
            
            RotateCamera();
            HandleWallRun();
            
            //Recoil pull down
            if (curRecoilTime != 0)
            {
            
                print($"Performing the Action: {mouseDir}, {intendedDirection} --> {curRecoilTime / recoilResetTime}");
                curRecoilTime += Time.deltaTime;
            
            
                intendedDirection = Vector2.Lerp(intendedDirection, mouseDir, curRecoilTime / recoilResetTime);
            
                if (curRecoilTime > recoilResetTime) // this may be sketchy
                    curRecoilTime = 0;
            }
            else
            {
                intendedDirection = mouseDir;
            }
        }

        public override void TakeDamage(Character attacker, float amount, Vector3 force)
        {
            base.TakeDamage(attacker, amount, force);
            rb.AddForce(force);
            ui.UpdateHealth(curHealth, amount);
        }
        
        public override void TakeDamage(Character attacker, float amount)
        {
            base.TakeDamage(attacker, amount);
            ui.UpdateHealth(curHealth, amount);
        }

        protected override void Heal(float amount)
        {
            base.Heal(amount);
            ui.UpdateHealth(curHealth, amount);
        }

        protected override void Die(Character attacker, float amount)
        {
            base.Die(attacker, amount);
            ui.UpdateHealth(curHealth, amount);
        }

        public override void UpgradeCharacter(CharacterUpgradeSo upgrade)
        {
            base.UpgradeCharacter(upgrade);
            ui.SetHealth(stats.MaxHealth, curHealth);
            ui.UpdateUpgradeUI();
        }
        #endregion
        
        #region Core

        #region CoreMovement
    
        protected override void Move()
        {
            if (isWallRunning)
            {
                float dir = wallRunScalar;

                //Gives backwards control
                if (directionVector.x < 0 || directionVector.z < 0)
                {
                    dir *= -0.6f; // Also reduce speed to 60%
                }
                rb.AddForce(WallSpeed * dir * wallForward, ForceMode.Force);
            }
            else
            {
                rb.AddForce(stats.MoveSpeed * directionVector.x * transform.forward, ForceMode.Force);
                rb.AddForce(stats.MoveSpeed * directionVector.z * transform.right, ForceMode.Force);
            }
            
            
            

            //Handle drag
            rb.drag = isGrounded ? stats.FloorDrag : stats.AirDrag;
        
            //Clamp speed
            Vector3 velocity = rb.velocity;
            Vector2 clamped = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.z), stats.MaxSpeed);
            rb.velocity = new Vector3(clamped.x, velocity.y, clamped.y);
            isMoving = rb.velocity.magnitude > 0.1f;
        }

        private void Jump()
        {
            jumpTime = 0;
            Vector3 newDir = Vector3.up;
            if (isWallRunning)
            {
                newDir = 4 * Vector3.RotateTowards(Vector3.Cross(-wallForward, Vector3.up), Vector3.up, 0.26f, 1);
            }
            else if (!isGrounded) // ORDER IS IMPORTANT!
            {
                Vector3 velocity = rb.velocity;
                velocity = new Vector3(velocity.x, 0, velocity.z);
                rb.velocity = velocity;
            }
            #if UNITY_EDITOR
            Debug.DrawRay(transform.position, newDir * 10, Color.red, 5);
            #endif
            rb.AddForce(stats.JumpForce * newDir, ForceMode.Impulse);
        }

        #endregion

        #region WallRunning
        private void HandleWallRun()
        {
            if (jumpTime < MaxJumpTime)
                return;
            Transform trans = transform;
            Vector3 right = trans.right;
            Vector3 position = trans.position;
            onRightWall = Physics.Raycast(position, right, out rightWallCast, WallDist, stats.FloorLayers);
            onLeftWall = Physics.Raycast(position, -right, out leftWallCast, WallDist, stats.FloorLayers);
            #if UNITY_EDITOR
            Debug.DrawRay(transform.position, right * WallDist, onRightWall?Color.green:Color.red);
            Debug.DrawRay(transform.position, -right * WallDist, onLeftWall?Color.green:Color.red);
            Debug.DrawRay(transform.position, trans.forward * WallDist, isWallRunning?Color.cyan:Color.yellow);
            #endif
            //print("iSGrounded: " + isGrounded);
            if (!isGrounded && jumpTime > MaxJumpTime && directionVector != Vector3.zero && (onLeftWall || onRightWall))
            {
                wallRunTime += Time.deltaTime;
                //Add custom gravity
                Vector3 velocity = rb.velocity;
                velocity = new Vector3(velocity.x, 0, velocity.z);
                rb.velocity = velocity;
                if (!isWallRunning) // DO not repeat this code when already wall running!
                {
                    //Start wall running
                    isWallRunning = true;
                    curJump = 0;
                    Vector3 wallNormal = onLeftWall ? leftWallCast.normal : rightWallCast.normal;
                    wallForward = Vector3.Cross(wallNormal, Vector3.up);
                    wallRunScalar = Mathf.RoundToInt(Vector3.Dot(wallForward, trans.forward));
                    #if UNITY_EDITOR
                    print($"Dot: {Vector3.Dot(wallForward, trans.forward)} - Rounded: {wallRunScalar}");
                    #endif
                    StartCoroutine(SmoothRotCam(wallRunScalar * -15, 0.2f));
                }
            }
            else
            {
                //ONLY RUNS ONCE
                if (isWallRunning)
                {
                    //Stop wall running
                    StartCoroutine(SmoothRotCam(0, 0.2f));
                }
                
                //For achievements.
                totalTimeWallRunning += wallRunTime;
                longestWallRun = wallRunTime;
                wallRunTime = 0;
                
                isWallRunning = false;
            }
        }
        private IEnumerator SmoothRotCam(int newRot, float duration)
        {
            print("Rotating Cam: " + newRot);
            float prvRot = cmv.m_Lens.Dutch;
            float curTime = 0;
            while (curTime < duration)
            {
                curTime += Time.deltaTime;
                cmv.m_Lens.Dutch = Mathf.Lerp(prvRot,newRot, curTime / duration);
                yield return null;
            }

            yield return null;
        }
        private void RotateCamera()
        {
            //Pause handling
            if (Time.timeScale == 0) return;
            
            transform.rotation *= Quaternion.AngleAxis(intendedDirection.x * mouseSensitivity, Vector3.up);
            head.rotation *= Quaternion.AngleAxis(intendedDirection.y * mouseSensitivity, Vector3.right);
        
            Vector3 angles = Vector3.zero;

            angles.x = head.localEulerAngles.x;
            //print($"rotating {angles}");
            //Up/Down clamped
            if (angles.x > 180 && angles.x < 360 - viewLockY)
            {
                angles.x = 360 - viewLockY;
            }
            else if (angles.x < 180 && angles.x > viewLockY)
            {
                angles.x = viewLockY;
            }


            head.localEulerAngles = angles;
        }
        
        //takes in an input and checks if the the slot can be changed to before swapping to it
        private void ToggleWeaponSlot(int slot)
        {
            ui.SetCurrentWeapon(weaponIndex, slot);
            if (slot <=  shootingCapability.Weapons.Length)
            {
                source.PlayOneShot(weaponChangeSound);
                shootingCapability.Weapons[weaponIndex].tryingToShoot = false;
                weaponIndex = slot;
            }
        }
        #endregion
        #endregion
        
        #region ExposedFucntions
        public void AddRecoil(Vector2 recoilPattern)
        {
            print("Adding recoil");
            curRecoilTime = 0.01f;
            intendedDirection += recoilPattern;
        }

        public void SetStateHUD(bool state)
        {
            ui.SetUI(state);
        }

        public void SetStateHUDAndWeapon(bool state)
        {
            SetStateHUD(state);
            shootingCapability.Weapons[weaponIndex].tryingToShoot = false;
        }
        
        public void UpgradeAttackCharacter(WeaponUpgradeSo upgrade, int idx)
        {
            shootingCapability.Weapons[idx].Upgrade(upgrade);
            source.PlayOneShot(stats.UpgradeSound);
        }
        
        //increases weapon slots available 
        //TODO: ???
        /*public void IncreaseWeaponSlots()
        {
            if (weaponUnlockCount < 3)
            {
                source.PlayOneShot(weaponUpgradeSound);
                weaponUnlockCount++;
            }
            else
            {
                source.PlayOneShot(weaponUpgradeFailSound);
            }
        }*/
        #endregion
        
#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            var transform1 = transform;
            var position = transform1.position;
            var right = transform1.right;
            Gizmos.DrawRay(position, right * WallDist);
            Gizmos.DrawRay(position, -right * WallDist);
            Gizmos.DrawRay(position, transform.forward * WallDist);
        }
#endif
    }
}