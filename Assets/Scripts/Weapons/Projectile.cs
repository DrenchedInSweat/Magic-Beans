using System;
using Characters;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        private enum EProjectileType
        {
            Explosion,
            Electrocute
        }
        [SerializeField] private EProjectileType type;
        private delegate void OnHitDelegate(Transform hitObj);
    
        private Character myOwner;
        private GameObject myOwnerObj;
        private OnHitDelegate onHit;
        private float aoe;
        private int recursion;
        private int bounces;
        private float damage;

        private Rigidbody rb;
    
        public void Init(Character owner, float areaOfEffect, float dmg, int recursionFactor, int bounceFactor)
        {
            myOwner = owner;
            myOwnerObj = owner.gameObject;
            aoe = areaOfEffect;
            recursion = recursionFactor;
            bounces = bounceFactor;
            damage = dmg;
        }

        private void Awake()
        {
            print("Testing: " + type);
            rb = GetComponent<Rigidbody>();
            switch (type)
            {
                case EProjectileType.Electrocute:
                    onHit = Electric;
                    break;
                case EProjectileType.Explosion:
                    onHit = Explosive;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(transform.position, rb.velocity.normalized, out RaycastHit hit, 1))
            {
                #if UNITY_EDITOR
                Debug.DrawRay(transform.position, rb.velocity, Color.red, 10);
                #endif
                GameObject go = hit.transform.gameObject;
                print("Collided with: " + go.name);
                //First, check to see if the hit object is this object OR the last hit object / original owner
                if (go != gameObject && go != myOwnerObj)
                {
                    //If it is, then it shouldn't hit...



                    onHit.Invoke(hit.transform);

                    if (bounces > 0)
                    {
                        //Vector3 t = transform.position;
                        //transform.position = col.ClosestPoint(t);
                        rb.velocity = Vector3.Reflect( rb.velocity, hit.normal);
#if UNITY_EDITOR
                        Debug.DrawRay(transform.position, rb.velocity, Color.green, 10);
#endif
                        bounces -= 1;
                        myOwnerObj = go;
                    }
                    else
                    {
                        //If it's not a character
                        Destroy(gameObject);
                    }
                }
            }
        }


        //TODO: Implement bomb recursion
        private void Explosive(Transform hitObj)
        {
            print("Doing Electric");
            RaycastHit[] results = new RaycastHit[15]; // This is how many it can hit...
            Transform t = transform;
            int num = Physics.SphereCastNonAlloc(t.position, aoe * 0.5f, t.forward, results,
                aoe * 0.5f, GameManager.Instance.HittableLayers);

            for (int i = 0; i < num; i++)
            {
                //Does this existing imply that a spherecast only works on objs with rbs?
                Rigidbody rb = results[i].rigidbody;
                rb.AddForce(aoe * -(results[i].point - transform.position).normalized, ForceMode.Force); // This may feel better as impulse
            }
        }
    
        private void Electric(Transform hitObj)
        {
            print("Doing Electric");
            ElectricRecurse(hitObj, hitObj, recursion);
        }

        private void ElectricRecurse(Transform prv, Transform hitObj, int remainingHits)
        {
            //Base Case
            if (remainingHits == 0)
                return;
        
            Electrocute(hitObj, remainingHits);
        
            RaycastHit[] results = new RaycastHit[10]; // This is how many it can hit...
            Transform t = transform;
            int num = Physics.SphereCastNonAlloc(t.position, remainingHits * 2.5f, t.forward, results, remainingHits * 2.5f, GameManager.Instance.ElectricLayers);

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
                    character.TakeDamage(myOwner,  (power + 1f) / recursion * damage);
            }
            else if (hitObj.TryGetComponent(out PuzzleSwitch hitSwitch))
            {
                print("Electrocuted a lever");
                hitSwitch.Activate();
            }
        }
    }
}
