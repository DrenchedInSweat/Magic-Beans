using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Animator))] // All are enemies going to have Rigids?
public class Character : MonoBehaviour
{
    protected Animator _animator;
    
    //----------------------MOVEMENT--------------------//
    [Header("Movement")]
    
    [Tooltip("XZ move speed of the character")]
    [SerializeField, Min(0)] protected float moveSpeed;
    
    [Tooltip("The Absolute speed limit of the object")]
    [SerializeField] protected float maxSpeed;

    [Tooltip("The drag against the player while on the ground (Limiting slide)")]
    [SerializeField] protected float floorDrag;
    
    [Tooltip("The drag against the player while on the ground (Limiting slide)")]
    [SerializeField] protected float airDrag;

    [Tooltip("Jump force of the character")]
    [SerializeField, Min(0)] protected float jumpForce;

    [Tooltip("The number of jumps a character can preform from the ground. ")]
    [SerializeField, Min(1)] protected int maxJumps = 1;
    protected int curJump;

    [Header("Floor Stuff")]
    [Tooltip("The transform where the center of the footsies are")]
    [SerializeField] protected Transform feetCenter;

    [Tooltip("Range of the footsies")]
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask floorLayers; // TODO: Make safe accessible elsewhere
    
    

    protected const float MaxJumpTime = 0.2f;
    protected float jumpTime;

    protected Vector3 directionVector;
    protected bool isGrounded;



    //------------------Character Stats---------------//
    [Header("Character Stats")]
    [SerializeField] protected float health;
    
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        jumpTime += Time.deltaTime;
        Move();
        CheckFloor();
    }

    /// <summary>
    /// Check for allowed floors below... 
    /// </summary>
    private void CheckFloor()
    {
       
        isGrounded = Physics.Raycast(feetCenter.position, Vector3.down, range, floorLayers);
        
        #if UNITY_EDITOR
        Debug.DrawRay(feetCenter.position, Vector3.down * range, isGrounded?Color.green:Color.red);
        #endif
        
        if (isGrounded)
        {
            curJump = 0;
        }
    }

    /// <summary>
    /// Makes a character take damage. Attacker can be used to handle achievements
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="amount"></param>
    protected void TakeDamage(Character attacker, float amount)
    {
        health -= amount;
        if (health < 0)
        {
            Die(attacker, amount);
        }
    }

    /// <summary>
    /// Kills and deletes a Character after a set time
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="amount"></param>
    /// TODO: Play animation / Ragdoll --> Sink into floor --> Delete
    protected virtual void Die(Character attacker, float amount)
    {
        
    }

    /// <summary>
    /// Set the max jumps of this character
    /// </summary>
    /// <param name="newMaxJumps"></param>
    public void SetMaxJumps(int newMaxJumps)
    {
        maxJumps = newMaxJumps;
    }

    protected virtual void Move()
    {
        
    }

}
