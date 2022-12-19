using System;
using System.Collections;
using Characters;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private VisualEffect onHit;
        [SerializeField] private float onHitDur;
        [SerializeField] private VisualEffect onDestroy;
        [SerializeField] private float onDestroyDur;
        [SerializeField] protected AudioClip onHitSound;

        private GameObject myOwnerObj;
        protected Rigidbody rb;

        protected int bounces;
        protected Character myOwner;
        protected int recursion;
        protected float aoe;
        protected float damage;

        protected Vector3 n;

        protected GameObject obj;

        public void Init(Character owner, float areaOfEffect, float dmg, int recursionFactor, int bounceFactor,
            GameObject original, Vector3 statsBulletSpeed)
        {
            myOwner = owner;
            myOwnerObj = owner.gameObject;
            aoe = areaOfEffect;
            recursion = recursionFactor;
            bounces = bounceFactor;
            damage = dmg;
            obj = original;
            rb = GetComponent<Rigidbody>();
            rb.AddForce(statsBulletSpeed, ForceMode.Impulse);
            CheckHit(statsBulletSpeed);
        }


        private void FixedUpdate()
        {
           CheckHit(rb.velocity);
        }
        
        private void CheckHit(Vector3 dir)
        { //Bit flip to prevent self collisions ;)
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 1, ~gameObject.layer))
            {
#if UNITY_EDITOR
                Debug.DrawRay(transform.position, dir, Color.red, 10);
#endif
                Transform t = hit.transform;
                GameObject go = t.gameObject;
                print("Collided with: " + go.name);
                //First, check to see if the hit object is this object OR the last hit object / original owner
                if (go != gameObject && go != myOwnerObj)
                {
                    myOwnerObj = go;
                    print("SUCCESSFULLY Collided with: " + go.name +" I am: " + gameObject.name);
                    //If it is, then it shouldn't hit...
                    transform.position = hit.point;
                    OnHit(t);
                    n = hit.normal;
                    if (onHit)
                    {
                        Transform r = transform;

                        Destroy(
                            Instantiate(onHit.gameObject, r.position,
                                Quaternion.LookRotation(Vector3.Cross(t.right, hit.normal), hit.normal)), onHitDur);
                    }
                    
                    if (bounces-- > 0)
                    {
                        rb.velocity = Vector3.Reflect(dir, n);
                    }
                    else
                    {
                        //If it's not a character
                        Destroy(gameObject);
                    }
                }
            }
            
        }

        protected virtual void OnHit(Transform hitObject)
        {
            if (hitObject.TryGetComponent(out Character c))
            {
                c.TakeDamage(myOwner, damage, rb.velocity);
            }
        }
        protected virtual void OnDestroy()
        {
            if (onDestroy)
            {
                Transform t = transform;
                Destroy(Instantiate(onDestroy.gameObject, t.position, t.rotation), onDestroyDur);
            }
        }
    
        
    }
}
