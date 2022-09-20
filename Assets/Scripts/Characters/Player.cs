using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private bool tryingToJump;
    private Vector2 mouseDir;
    
    
    //TODO: If we go in the route of shooting,use Impulse Source -- Do this for landing aswell

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _controls = new PlayerControls();
        _controls.InGame.Enable();

        _controls.InGame.Jump.performed += x =>  tryingToJump = !tryingToJump;
        _controls.InGame.LeftRight.performed += ctx => directionVector.z = ctx.ReadValue<float>();
        _controls.InGame.ForwardBack.performed += ctx => directionVector.x = ctx.ReadValue<float>();
        _controls.InGame.CameraX.performed += ctx => mouseDir.y = ctx.ReadValue<float>();
        _controls.InGame.CameraY.performed += ctx => mouseDir.x = ctx.ReadValue<float>();
        
        _controls.InGame.Interact.performed += x => Interact(); // This should 
    }

    /// <summary>
    /// Casts a ray forward -- If the object hit implements the IInteractable interface? Or should it be a component? do things
    /// </summary>
    private void Interact()
    {
        
    }

    
    protected override void MovePlayer()
    {
        _rb.AddForce(moveSpeed * Time.fixedDeltaTime * directionVector.x * transform.forward, ForceMode.Impulse);
        _rb.AddForce(moveSpeed * Time.fixedDeltaTime * directionVector.z * transform.right, ForceMode.Impulse);
    }

    private void RotateCamera()
    {
        head.rotation *= Quaternion.AngleAxis(mouseDir.x * mouseSensitivity, Vector3.up);
        head.rotation *= Quaternion.AngleAxis(mouseDir.y * mouseSensitivity, Vector3.right);
        
        Vector3 angles = head.localEulerAngles;

        angles.z = 0;
        
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
        print("Attempted jump");
        //Implement a delay to prevent all jumps from being used instantly
        if (jumpTime >= MaxJumpTime)
        {
            print("Time valid");
            //If the player can jump OR is grounded
            if (++curJump < maxJumps || isGrounded) // ORDER IS IMPORTANT!
            {
                print("Jump valid");
                jumpTime = 0;
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                _rb.AddForce(jumpForce * Time.fixedDeltaTime  * Vector3.up, ForceMode.Impulse);
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
    }
}
