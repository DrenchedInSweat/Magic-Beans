using System;
using System.Collections;
using Characters.BaseStats;
using UnityEngine;

namespace Characters
{
    public class Enemy : Character
    {
        [SerializeField] protected EnemyStatsSo enemyStats;
        [SerializeField] private Transform head;
        private Player ply;
        
        private void Start()
        {
            ply = GameManager.Instance.Player;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            //Player is in range
            Vector3 dir = ply.transform.position - head.position;
            if (dir.magnitude < enemyStats.MaxEyeDist)
            {
#if UNITY_EDITOR
                Debug.DrawRay(transform.position, dir, Color.blue);
#endif
                //print($"ENEMY: In Range, Checking visibility {Mathf.Abs(Vector3.Angle(dir, head.up))} , {enemyStats.EyeAngle} ");
                if(Mathf.Abs(Vector3.Angle(dir, head.up)) < enemyStats.EyeAngle)
                {
                    print("ENEMY: Targeting Enemy!");
                    #if UNITY_EDITOR
                    Debug.DrawRay(transform.position, dir, Color.red);
                    #endif
                }
            }
        }

        protected virtual void Attack()
        {
        
        }

        /// <summary>
        /// Slowly rotate towards a location
        /// This function needs to be called every frame to really work
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="???"></param>
        protected void SlowRotate()
        {
            
        }


#if UNITY_EDITOR
        [SerializeField] private bool showAlways;
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!showAlways) return;
            Gizmos.color = Color.green;
            Vector3 fwd = head.up;
            Vector3 pos = head.position;
            Vector3 up = head.forward;
            Gizmos.DrawWireSphere(pos, enemyStats.MaxTargetDistance);
            Gizmos.DrawRay(pos, fwd * enemyStats.MaxEyeDist);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(pos, Quaternion.AngleAxis(enemyStats.EyeAngle, up) * fwd * enemyStats.MaxEyeDist);
            Gizmos.DrawRay(pos, Quaternion.AngleAxis(-enemyStats.EyeAngle, up) * fwd * enemyStats.MaxEyeDist);

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 fwd = head.up;
            Vector3 pos = head.position;
            Vector3 up = head.forward;
            Gizmos.DrawWireSphere(pos, enemyStats.MaxTargetDistance);
            Gizmos.DrawRay(pos, fwd * enemyStats.MaxEyeDist);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(pos, Quaternion.AngleAxis(enemyStats.EyeAngle, up) * fwd * enemyStats.MaxEyeDist);
            Gizmos.DrawRay(pos, Quaternion.AngleAxis(-enemyStats.EyeAngle, up) * fwd * enemyStats.MaxEyeDist);
        }
#endif
        
    }
}
