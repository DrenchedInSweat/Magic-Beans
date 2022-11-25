using System;
using System.Collections;
using UnityEngine;

namespace Characters
{
    public class Ant : Enemy
    {
        private const int CheckLines = 8;
       [SerializeField]  private bool isRolling;

       private Rigidbody rb;
       private Transform tempPlayer;
       protected override void Awake()
       {
           base.Awake();
           //rb = GetComponent<Rigidbody>();
           tempPlayer = GameObject.Find("Player").transform;
       }

       // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            
            if (!isGrounded)
            {
                CheckAroundAnt();
                print("I'm Not on the ground");
            }
            else
            {
                //rb.velocity = Vector3.zero;
                //rb.angularVelocity = Vector3.zero;
                //rb.useGravity = false;
                Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, range);
                
                
                
                Debug.DrawLine(feetCenter.position, hit.point);
                //Vector3 deltaGap = hit.distance / range * (hit.point - transform.position);
                ///if(deltaGap.sqrMagnitude > 1)
                print($"ANT {(hit.point -   feetCenter.position)}, {moveSpeed * Time.deltaTime * transform.forward}");
                //transform.LookAt(tempPlayer);
                transform.position = hit.point + hit.normal * 0.5f + moveSpeed * Time.deltaTime * transform.forward;
                //transform.up = hit.normal;
                transform.LookAt(tempPlayer.position, hit.normal); //TODO: Redefine how look at works -- It should maintain it, up, but rotate the around the X and Z
                //transform.rotation = Quaternion.LookRotation(tempPlayer.position, hit.normal);
                
                //transform.position += (hit.point - feetCenter.position) + moveSpeed * Time.deltaTime * transform.forward;
                
                
                //When moving move towards the player
            }
        }

        private void CheckAroundAnt()
        {
            if (isRolling)
                return;
            
            float rads = 360f / CheckLines * Mathf.Deg2Rad;
            Vector3 transPos = transform.position;
            Vector2 direction = -Vector2.up;
            float shortest = 100;
            float rot = 0;
            
               for (int i = 0; i < CheckLines; i++)
            {
                //SOH CAH TOA
                float curRot = rads * i;
                //float y = Mathf.Cos(curRot), X = Mathf.Sin(curRot);
                //direction = direction * Mathf.Cos(curRot) + Vector3.Cross(direction fwd)
                
                direction.y =Mathf.Cos(curRot);
                direction.x =Mathf.Sin(curRot);
#if UNITY_EDITOR
                Debug.DrawRay(transPos, direction * range, Color.yellow);
#endif
                if (Physics.Raycast(transPos, direction, out RaycastHit hit, range))
                {
                    //Can't jut be rads, has to be normal of object... The up needs to be the normal.
                    float dist = hit.distance;
                    if (dist < shortest)
                    {
                        shortest = dist;
                        rot = rads * i; // Roll

                        print("Found an available target");
                    }
                }
            }

            
            //Shortest is set.
            if (!isRolling && shortest != 100)
            {
                rot *= Mathf.Rad2Deg;
                isRolling = true;
                //If greater than just going clockwise
                if (rot > 180)
                {
                    rot = 180 - rot;
                }

                StartCoroutine(Roll(rot));
            }
        }

        private IEnumerator Roll(float n)
        {
            float maxRotTime = 0.3f;
            float rotTime = 0;
            Vector3 old = transform.eulerAngles;
            
            float oldZ = old.z;
            while (!isGrounded && rotTime < maxRotTime)
            {
                rotTime += Time.deltaTime;
                old.z = Mathf.Lerp(oldZ, n, rotTime / maxRotTime);
                transform.eulerAngles = old;
                yield return null;
            }

            isRolling = false;
        }

        private void OnDrawGizmos()
        {
            float rads = 360f / CheckLines * Mathf.Deg2Rad;
            Vector3 transPos = transform.position;
            Vector2 direction = -Vector2.up;
            for (int i = 0; i < CheckLines; i++)
            {
                //SOH CAH TOA
                float curRot = rads * i;
                //float y = Mathf.Cos(curRot), X = Mathf.Sin(curRot);
                //direction = direction * Mathf.Cos(curRot) + Vector3.Cross(direction fwd)
                direction.y =Mathf.Cos(curRot);
                direction.x =Mathf.Sin(curRot);
#if UNITY_EDITOR
                Debug.DrawRay(transPos, direction * range, Color.yellow);
#endif
            }
        }
    }
    
}
