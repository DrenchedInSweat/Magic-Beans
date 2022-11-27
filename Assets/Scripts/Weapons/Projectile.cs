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
            Shock,
            Electrocute
        }
        [SerializeField] private EProjectileType type;
        private delegate void OnHitDelegate(Collider hitObj);
    
        private Character myOwner;
        private OnHitDelegate onHit;
        private float aoe;
        private int recursion;
        private float damage;
    
        public void Init(Character owner, float areaOfEffect, float dmg, int recursionFactor)
        {
            myOwner = owner;
            aoe = areaOfEffect;
            recursion = recursionFactor;
            damage = dmg;

            switch (type)
            {
                case EProjectileType.Electrocute:
                    onHit = Electric;
                    break;
                case EProjectileType.Explosion:
                    onHit = Explosive;
                    break;
                case EProjectileType.Shock:
                    onHit = Shock;
                    break;
            }

        }
        private void OnTriggerEnter(Collider col)
        {
            GameObject go = col.gameObject;
        
            //First, check to see if the hit object is this object...
            if (go == gameObject || go == myOwner.gameObject)
            {
                //If it is, then it shouldn't hit...
                return;
            }

            #if UNITY_EDITOR
            print("Bullet hit: " + go.name);
            #endif
            
            onHit.Invoke(col);
            
            //If it's not a character
            Destroy(gameObject);
        }
    
        //TODO: Implement bomb recursion
        private void Explosive(Collider hitObj)
        {
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
    
        private void Shock(Collider hitObj)
        {
            Electrocute(hitObj.transform, recursion);
        }
    
        private void Electric(Collider hitObj)
        {
            Transform t = hitObj.transform;
            ElectricRecurse(t, t, recursion);
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
                    character.TakeDamage(myOwner, power * damage);
            }
            else if (hitObj.TryGetComponent(out PuzzleSwitch hitSwitch))
            {
                print("Electrocuted a lever");
                hitSwitch.Activate();
            }
        }
    }
}
