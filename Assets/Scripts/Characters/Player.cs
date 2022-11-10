using System;using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : Character
{
    private Rigidbody _rb;
    private PlayerControls _controls;

    [Header("Player Controls")]
    [SerializeField] private float mouseSensitivity;
    [Tooltip("Limits the max rotation of the cam")]
    [SerializeField, Range(0,89.9f)] private float viewLockY;

    [SerializeField] private Transform head;

    [Header("Wall Running")]
    [Tooltip("Speed on wall")]
    [SerializeField] private float wallSpeed;
    [Tooltip("How long the player can wall run for")]
    [SerializeField] private float maxWallRunDuration;
    [Tooltip("Time until fully recovered")]
    [SerializeField] private float wallRecoverySpeed = 1;
    [Tooltip("Time until recovery starts")]
    [SerializeField] private float wallRecoveryDelay = 2;
    private float delayTimer;
    
    [Tooltip("The gravity on the wall")]
    [SerializeField] private float wallRunGravity;
    
    private float wallRunTime;
    private float wallDist = 0.75f;

    private RaycastHit leftWallCast;
    private bool onLeftWall;
    private RaycastHit rightWallCast;
    private bool onRightWall;

    private int wallRunScalar; // Used to help the player run backwards and rotate the cam

    private GameObject lastWall;

    private bool isWallRunning;

    private bool tryingToJump;
    private Vector2 mouseDir;

    private Vector3 wallForward;

    [Header("UI")] [SerializeField] private Slider slider;
    [SerializeField] private CinemachineVirtualCamera cmv;

    [SerializeField] private RecoilController rc;
    
    //TODO: If we go in the route of shooting,use Impulse Source -- Do this for landing aswell

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _controls = new PlayerControls();
        _controls.InGame.Enable();

        _controls.InGame.Jump.performed += x =>  tryingToJump = !tryingToJump;
        _controls.InGame.Jump.canceled += x =>  tryingToJump = !tryingToJump;
        _controls.InGame.Shoot.performed += x =>  tryingToShoot = !tryingToShoot;
        _controls.InGame.Shoot.canceled += x =>  tryingToShoot = !tryingToShoot;
        _controls.InGame.LeftRight.performed += ctx => directionVector.z = ctx.ReadValue<float>();
        _controls.InGame.ForwardBack.performed += ctx => directionVector.x = ctx.ReadValue<float>();
        _controls.InGame.CameraX.performed += ctx => mouseDir.y = ctx.ReadValue<float>();
        _controls.InGame.CameraY.performed += ctx => mouseDir.x = ctx.ReadValue<float>();
        
        
        _controls.InGame.Interact.performed += x => Interact(); // This should 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Casts a ray forward -- If the object hit implements the IInteractable interface? Or should it be a component? do things
    /// </summary>
    private void Interact()
    {
        
    }

    
    protected override void Move()
    {

        if (isWallRunning)
        {
            //Cast a ray from player to wall,
            //if the angle between wall and camera is positive, move in a positive direction
            //else move in a negative direction
            //print(Mathf.RoundToInt(Vector3.Dot(wallForward, transform.forward)));

            float dir = wallRunScalar;

            //Gives backwards control
            if (directionVector.x < 0 || directionVector.z < 0)
            {
                dir *= -0.6f; // Also reduce speed to 60%
            }
            _rb.AddForce(wallSpeed * Time.fixedDeltaTime * dir * wallForward, ForceMode.Force);
        }
        else
        {
            _rb.AddForce(moveSpeed * Time.fixedDeltaTime * directionVector.x * transform.forward, ForceMode.Force);
            _rb.AddForce(moveSpeed * Time.fixedDeltaTime * directionVector.z * transform.right, ForceMode.Force);
        }

        //Handle drag
        _rb.drag = isGrounded ? floorDrag : airDrag;
        
        //Clamp speed
        Vector2 clamped = Vector2.ClampMagnitude(new Vector2(_rb.velocity.x, _rb.velocity.z), maxSpeed);
        _rb.velocity = new Vector3(clamped.x, _rb.velocity.y, clamped.y);
    }

    private void RotateCamera()
    {
        transform.rotation *= Quaternion.AngleAxis(mouseDir.x * mouseSensitivity, Vector3.up);
        head.rotation *= Quaternion.AngleAxis(mouseDir.y * mouseSensitivity, Vector3.right);
        
        Vector3 angles = Vector3.zero;

        angles.x = head.localEulerAngles.x;
        print($"rotating {angles}");
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

    private void Jump()
    {
        //Implement a delay to prevent all jumps from being used instantly
        if (jumpTime >= MaxJumpTime)
        {
            //If the player can jump OR is grounded
            if (curJump++ < maxJumps)
            {
                jumpTime = 0;
                if (isWallRunning)
                {
                    //Reflection into the walls surface?
                    //Add force orthagonal to the wall?
                    
                    _rb.AddForce(jumpForce * 100 *  Vector3.Cross(-wallForward, Vector3.up), ForceMode.Impulse);
                    _rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
                }
                else if (!isGrounded) // ORDER IS IMPORTANT!
                {
                    _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                }
                _rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            }
        }
    }

    /*
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    } */

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(tryingToJump)
            Jump();
        RotateCamera();

        if (Input.GetKeyDown(KeyCode.Tab))
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        
        HandleWallRun();
        slider.value = GetRemainingWallPercent();
    }

    private void HandleWallRun()
    {
        onRightWall = Physics.Raycast(transform.position, transform.right, out rightWallCast, wallDist, floorLayers);
        onLeftWall = Physics.Raycast(transform.position, -transform.right, out leftWallCast, wallDist, floorLayers);
        
        #if UNITY_EDITOR
        Debug.DrawRay(transform.position, transform.right * wallDist, onRightWall?Color.green:Color.red);
        Debug.DrawRay(transform.position, -transform.right * wallDist, onLeftWall?Color.green:Color.red);
        Debug.DrawRay(transform.position, transform.forward * wallDist, isWallRunning?Color.cyan:Color.yellow);
        #endif

        //print("iSGrounded: " + isGrounded);
        if (!isGrounded && directionVector != Vector3.zero && (onLeftWall || onRightWall) && wallRunTime > 0)
        {
            wallRunTime -= Time.deltaTime;
            //Add custom gravity
            _rb.velocity = new Vector3(_rb.velocity.x, wallRunGravity, _rb.velocity.z);
            if (!isWallRunning) // DO not repeat this code when already wall running!
            {
                isWallRunning = true;
                curJump = 0;
                Vector3 wallNormal = onLeftWall ? leftWallCast.normal : rightWallCast.normal;
                wallForward = Vector3.Cross(wallNormal, Vector3.up);
                wallRunScalar = Mathf.RoundToInt(Vector3.Dot(wallForward, transform.forward));
                print($"Dot: {Vector3.Dot(wallForward, transform.forward)} - Rounded: {wallRunScalar}");
                
                
                StartCoroutine(SmoothRotCam(wallRunScalar * -15, 0.2f));
                
            }

        }
        else
        {
            //ONLY RUNS ONCE
            if (isWallRunning)
            {
                print("off the wall");
                StopWallRun();
            }

            isWallRunning = false;
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                wallRunTime = Mathf.Min(Time.deltaTime * wallRecoverySpeed + wallRunTime, maxWallRunDuration);
            }
        }
    }

    private void StopWallRun()
    {
        delayTimer = wallRecoveryDelay;
        StartCoroutine(SmoothRotCam(0, 0.2f));
    }

    private IEnumerator SmoothRotCam(int newRot, float duration)
    {
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


    //For my UI friends
    public float GetRemainingWallPercent()
    {
        return wallRunTime / maxWallRunDuration;
    }

    public void AddRecoil(Vector3 recoilPattern)
    {
        rc.AddRecoil(recoilPattern);
    }
    
    
}
