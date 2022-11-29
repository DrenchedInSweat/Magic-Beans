using Characters;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private VisualEffect onHit;
        [SerializeField] private float onHitDur;
        [SerializeField] private VisualEffect onDestroy;
        [SerializeField] private float onDestroyDur;
        
        private GameObject myOwnerObj;
        private Rigidbody rb;
        
        protected int bounces;
        protected Character myOwner;
        protected int recursion;
        protected float aoe;
        protected float damage;

        protected Vector3 n;

        protected GameObject obj;

        public void Init(Character owner, float areaOfEffect, float dmg, int recursionFactor, int bounceFactor, GameObject original)
        {
            myOwner = owner;
            myOwnerObj = owner.gameObject;
            aoe = areaOfEffect;
            recursion = recursionFactor;
            bounces = bounceFactor;
            damage = dmg;
            obj = original;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            //Bit flip to prevent self collisions ;)
            if (Physics.Raycast(transform.position, rb.velocity.normalized, out RaycastHit hit, 1, ~gameObject.layer))
            {
                #if UNITY_EDITOR
                Debug.DrawRay(transform.position, rb.velocity, Color.red, 10);
                #endif
                Transform t = hit.transform;
                GameObject go = t.gameObject;
                print("Collided with: " + go.name);
                //First, check to see if the hit object is this object OR the last hit object / original owner
                if (go != gameObject && go != myOwnerObj)
                {
                    //If it is, then it shouldn't hit...
                    
                    OnHit(t);
                    n = hit.normal;
                    if (bounces > 0)
                    {
                        //Vector3 t = transform.position;
                        //transform.position = col.ClosestPoint(t);
                        rb.velocity = Vector3.Reflect( rb.velocity, n);
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
                        return;
                    }

                    if (onHit)
                    {
                        Transform r = transform;
                        Destroy(Instantiate(onHit.gameObject, r.position, r.rotation), onHitDur);
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
