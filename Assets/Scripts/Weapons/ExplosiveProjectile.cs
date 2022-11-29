using System;
using Characters;
using UnityEngine;

namespace Weapons
{
    public class ExplosiveProjectile : Projectile
    {
        private readonly int recursiveProjectiles = 6;
        private readonly float recursiveForce = 10;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // explode
            RaycastHit[] results = new RaycastHit[15]; // This is how many it can hit...
            Transform t = transform;
            Vector3 position = t.position;
            AudioSource.PlayClipAtPoint(onHitSound, position, 0.2f);
            int num = Physics.SphereCastNonAlloc(position, aoe, t.forward, results,
                aoe, GameManager.Instance.HittableLayers);
            for (int i = 0; i < num; i++)
            {
                //Does this existing imply that a spherecast only works on objs with rbs?
                Rigidbody rb = results[i].rigidbody;
                Vector3 d = aoe * -(results[i].point - position).normalized;
                
                if(results[i].transform.TryGetComponent(out Character c))
                    c.TakeDamage(myOwner, damage, d);
                else if (rb)
                    rb.AddForce(d, ForceMode.Impulse);

            }
            
            if(recursion == 0) return;
            // explode
            float degs = 360f / recursiveProjectiles;
            print("Spawning objects");
            for (int i = 0; i < recursiveProjectiles; ++i)
            {
                GameObject go = Instantiate(obj, position, Quaternion.identity);

                Vector3 forward = Vector3.Cross(n, Vector3.right);
                Vector3 rot = Quaternion.AngleAxis(i * degs,  n) * forward;
                rot = Vector3.RotateTowards(rot, n, 1, 15);
                    
                #if UNITY_EDITOR
                Debug.DrawRay(position, rot * recursiveForce, new Color(0.5f,0.5f,0), 2f);
                #endif
                go.GetComponent<ExplosiveProjectile>().Init(myOwner, aoe, damage, recursion-1, bounces, obj);
                go.GetComponent<Rigidbody>().AddForce(rot * recursiveForce, ForceMode.Impulse);
                Destroy(go, 10);
            }
        }
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, aoe);
        }
        #endif
    }
}
