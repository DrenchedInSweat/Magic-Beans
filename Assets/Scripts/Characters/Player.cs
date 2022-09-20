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

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _controls = new PlayerControls();
        _controls.InGame.Enable();

        _controls.InGame.Jump.performed += x => Jump();
        _controls.InGame.Movement.performed += ctx => directionVector = ctx.ReadValue<Vector2>();
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
        _rb.AddForce(moveSpeed * Time.fixedDeltaTime * new Vector3(directionVector.x, 0, directionVector.y), ForceMode.Impulse);
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
    }
}
