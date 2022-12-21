using System.Collections;
using Characters.BaseStats;
using Characters.Upgrades;
using Cinemachine;
using UnityEngine;
using Weapons;

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

        [Header("Player Only")]
        [SerializeField] private float wallSpeed;
        [SerializeField] private float maxSpeedMultOnWall;

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
        public ShootingCapability ShootingCapability { get; private set; }

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

            // ------------------------- Handle Controls ------------------------------
            controls = new PlayerControls();
            controls.Enable();
            
            controls.InGame.Jump.started += _ =>  tryingToJump = true;
            controls.InGame.Jump.canceled += _ =>  tryingToJump = false;
            controls.InGame.Shoot.started += _ =>
            {
                if (canAttack) SetWeaponState(true);
            };
            controls.InGame.Shoot.canceled += _ => SetWeaponState(false);
            controls.InGame.WeaponToggle1.started += _ => ToggleWeaponSlot(0);
            controls.InGame.WeaponToggle2.started += _ => ToggleWeaponSlot(1);
            controls.InGame.WeaponToggle3.started += _ => ToggleWeaponSlot(2);
            controls.InGame.Scroll.started += value =>
            {
                int i = weaponIndex + (int)value.ReadValue<float>();
                if (i >= ShootingCapability.Len)
                    i = 0;
                else if (i < 0)
                    i = ShootingCapability.Len-1;
                ToggleWeaponSlot(i);
            };
            
            controls.InGame.Movement.performed += ctx => directionVector = ctx.ReadValue<Vector2>();
            
            controls.InGame.Camera.performed += ctx => mouseDir = ctx.ReadValue<Vector2>();
            
            controls.UI.EscapeMenu.performed += _ => GameManager.Instance.TogglePause();
           

            GameManager.Instance.onPauseGameUnpaused += () => controls.InGame.Enable();
            GameManager.Instance.onPauseGamePaused += () => controls.InGame.Disable();


            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            ui.SetHealth(stats.MaxHealth, curHealth);
            
            ShootingCapability = GetComponent<ShootingCapability>();
            ShootingCapability.Init(this);
            for (int i = 0; i < ShootingCapability.Len; ++i)
            {
                ui.SetWeapon(i, ShootingCapability.Weapons[weaponIndex].GetStats<WeaponStatsSo>().Sprite);
            }
            
            ui.SetCurrentWeapon(0,0);
            source.loop = true;
        }

        private void SetWeaponState(bool x)
        {
            Weapon w = ShootingCapability.Weapons[weaponIndex];
            print("Testing: " + x + ", " + w.GetStats<WeaponStatsSo>().Name);
            w.tryingToShoot = x;
            animator.SetBool(attackAnimID, x);
        }

        // Update is called once per frame
        protected override void TrueUpdate()
        {
            base.TrueUpdate();
            if(tryingToJump && CanJump())
                Jump();
            
            RotateCamera();
            HandleWallRun();
            
            //Recoil pull down
            if (curRecoilTime != 0)
            {
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

        protected override void Die(Character attacker, string attackerName)
        {
            source.PlayOneShot(stats.DieSound);
            ui.UpdateHealth(curHealth, 1);
            ui.OnDie(attackerName);
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
            float mSpeed = maxSpeed;
            if (isWallRunning)
            {
                float dir = wallRunScalar;

                //Gives backwards control
                if (directionVector.x <= 0 && directionVector.y <= 0)
                {
                    dir *= -0.6f; // Also reduce speed to 60%
                }
                rb.AddForce(wallSpeed * dir * wallForward, ForceMode.Force);
                mSpeed *= maxSpeedMultOnWall;
            }
            else
            {
                rb.AddForce(speed * directionVector.x * transform.forward, ForceMode.Force);
                rb.AddForce(speed * directionVector.y * transform.right, ForceMode.Force);
            }
            
            
            

            //Handle drag
            rb.drag = isGrounded ? stats.FloorDrag : stats.AirDrag;
        
            //Clamp speed
            Vector3 velocity = rb.velocity;
            Vector2 clamped = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.z),mSpeed);
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
            if (!isGrounded && jumpTime > MaxJumpTime && directionVector != Vector2.zero && (onLeftWall || onRightWall))
            {
                wallRunTime += Time.deltaTime;
                //Add custom gravity
                Vector3 velocity = rb.velocity;
                velocity = new Vector3(velocity.x, 0, velocity.z);
                rb.velocity = velocity;
                
                
                Vector3 wallNormal = onLeftWall ? leftWallCast.normal : rightWallCast.normal;
                float wallAngle = Vector3.Dot(wallNormal, trans.forward);
                
                //If the dot is positive, it needs to go down. If the dot is negative it needs to go up.

                //If we need to force the cameras rotation
                /*
                if (Mathf.Abs(wallAngle) >= wallRunHorLim)
                {
                    float dir = (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.z)) ? velocity.x : velocity.z;
                    transform.rotation *= Quaternion.AngleAxis(wallAngle * dir, Vector3.up);
                }
                */
                

                print($"Dot: {wallAngle}");
                if (!isWallRunning) // DO not repeat this code when already wall running!
                {
                    //Start wall running
                    isWallRunning = true;
                    curJump = 0;
                    
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
            
            transform.rotation *= Quaternion.AngleAxis(intendedDirection.x * mouseSensitivity * Time.deltaTime, Vector3.up);
            head.rotation *= Quaternion.AngleAxis(intendedDirection.y * mouseSensitivity * Time.deltaTime, Vector3.right);
        
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
            if (slot <  ShootingCapability.Len)
            {
                ui.SetCurrentWeapon(weaponIndex, slot);
                source.PlayOneShot(weaponChangeSound, 0.1f);
                SetWeaponState(false);
                animator.SetBool(ShootingCapability.Hashes[weaponIndex], false);
                animator.SetBool(ShootingCapability.Hashes[slot], true);
                weaponIndex = slot;
                SetLoopedNoise(ShootingCapability.Weapons[weaponIndex].idleClip);
            }
        }
        #endregion
        #endregion
        
        #region ExposedFucntions
        public void AddRecoil(Vector2 recoilPattern)
        {
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
            SetWeaponState(false);
        }

        //Why do I have the one below??
        //Temp code used for action block
        public void UpgradeAttackCharacter(WeaponUpgradeSo upgrade)
        {
            UpgradeAttackCharacter(upgrade, (int)upgrade.MyApplicableWeapons);
        }

        public void UpgradeAttackCharacter(WeaponUpgradeSo upgrade, int val)
        {
#if  UNITY_EDITOR
            bool oneFound = false;
#endif
            
            foreach (Weapon wep in ShootingCapability.Weapons)
            {
                if (((int)wep.GetStats<WeaponStatsSo>().WeaponType & val) != 0)
                {
                    wep.Upgrade(upgrade);
#if  UNITY_EDITOR
                    oneFound = true;
#endif
                }
            }
#if  UNITY_EDITOR
            if(!oneFound)
                Debug.LogError("Upgrade was not applied!");
#endif
            //Play sound and FXs
            source.PlayOneShot(stats.UpgradeSound, 0.1f);
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