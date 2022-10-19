using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    
    private delegate void OnHitDelegate(Collider hitObj);
    
    private Character myOwner;
    private EProjectileType myType;
    private OnHitDelegate onHit;
    private int level;
    private float damage;
    
    public void Init(Character owner, EProjectileType effect, int lv = 1, float dmg = 1)
    {
        myType = effect;
        myOwner = owner;
        level = lv;
        damage = dmg;
        switch (effect)
        {
            case EProjectileType.Fling: // This effect will happen instantly
                Rigidbody rb = myOwner.GetComponent<Rigidbody>();
                rb.AddForce(level * transform.forward, ForceMode.Force);
                Destroy(gameObject);
                break;
            case EProjectileType.Pull:
                onHit = Pull;
                break;
            case EProjectileType.Punch:
                onHit = Punch;
                break;
            case EProjectileType.Explosive:
                onHit = Explosive;
                break;
            case EProjectileType.Shock:
                onHit = Shock;
                break;
            case EProjectileType.Electric:
                onHit = Electric;
                break;
        }
    }
    private void OnTriggerEnter(Collider Collider)
    {
        GameObject go = Collider.gameObject;
        
        //First, check to see if the hit object is this object...
        if (go == gameObject || go == myOwner.gameObject)
        {
            //If it is, then it shouldn't hit...
            return;
        }
        
        print("Bullet hit: " + go.name);
        onHit.Invoke(Collider);
        //If it's not a character, 
        

        Destroy(gameObject);
    }

    private void Pull(Collider hitObj)
    {
        if (!hitObj.gameObject.TryGetComponent(out Character c)) return;
        Rigidbody rb = c.GetComponent<Rigidbody>();
        rb.AddForce(level * -transform.forward, ForceMode.Force); // This may feel better as impulse
    }
    
    private void Punch(Collider hitObj)
    {
        if (!hitObj.gameObject.TryGetComponent(out Character c)) return;
        Rigidbody rb = c.GetComponent<Rigidbody>();
        rb.AddForce(level * transform.forward, ForceMode.Force); // This may feel better as impulse
    }
    
    private void Explosive(Collider hitObj)
    {
        RaycastHit[] results = new RaycastHit[level * 5]; // This is how many it can hit...
        int num = Physics.SphereCastNonAlloc(transform.position, level * 0.5f, transform.forward, results,
            level * 0.5f, GameManager.Instance.HittableLayers);

        for (int i = 0; i < num; i++)
        {
            //Does this existing imply that a spherecast only works on objs with rbs?
            Rigidbody rb = results[i].rigidbody;
            rb.AddForce(level * -(results[i].point - transform.position).normalized, ForceMode.Force); // This may feel better as impulse
        }
    }
    
    private void Shock(Collider hitObj)
    {
        Electrocute(hitObj.transform, level);
    }
    
    private void Electric(Collider hitObj)
    {
        ElectricRecurse(hitObj.transform, hitObj.transform, level);
    }

    private void ElectricRecurse(Transform prv, Transform hitObj, int remainingHits)
    {
        //Base Case
        if (remainingHits == 0)
            return;
        
        Electrocute(hitObj, remainingHits);
        
        RaycastHit[] results = new RaycastHit[10]; // This is how many it can hit...
        int num = Physics.SphereCastNonAlloc(transform.position, remainingHits * 2.5f, transform.forward, results, remainingHits * 2.5f, GameManager.Instance.ElectricLayers);

        //Search through and get the correct instance...
        float curMinDist = 100f;
        Transform curTarg = null;
        for (int i = 0; i < num; i++)
        {
            //The object his is the object OR it's the previous
            if (results[i].transform == hitObj || results[i].transform == prv)
                continue;

            float dist = results[i].distance;
            if (dist < curMinDist)
            {
                curTarg = results[i].transform;
                curMinDist = dist;
            }
        }

        //Failed to riccochet
        if (curTarg == null)
            return;
        //Recurse
        ElectricRecurse(hitObj, curTarg, remainingHits-1);

    }

    //Should this be outside of this scope??
    private void Electrocute(Transform hitObj, int power)
    {
        
        if (hitObj.TryGetComponent(out Character character))
        {
            print("Electrocuted a character");
            if (character is Slime s) //Slime logic
                s.Electrocute();  
            else //Take damage
                character.TakeDamage(myOwner, power * damage);
        }
        else if (hitObj.TryGetComponent(out PuzzleSwitch hitSwitch))
        {
            print("Electrocuted a lever");
            hitSwitch.Activate();
        }
    }
}
