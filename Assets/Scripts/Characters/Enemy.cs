using System.Collections;
using Characters.BaseStats;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Enemy : Character
    {
        
        [SerializeField] protected EnemyStatsSo enemyStats;
        [SerializeField] private Transform head;
        protected NavMeshAgent agent;
        private Player ply;
        
        private bool targetWasVisible;
        private float targetTime;
        private const float MaxTargetTime = 5;
        protected bool targeting;
        
        [SerializeField]private float maxIdleTime = 10;
        [SerializeField]private float minIdleTime = 10;
        [SerializeField]private float attackDist;
        
        protected float damage;
        private float curTimeBetweenAttacks;
        private float maxHealth;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<NavMeshAgent>();
            StartCoroutine(StartIdle());
            print("Got the navMesh");
        }

        private void Start()
        {
            ply = GameManager.Instance.Player;
            targetTime = MaxTargetTime;

            damage = stats.ContactDamage;
            attackDist = enemyStats.AttackDist;
            maxHealth = stats.MaxHealth;
            agent.speed = stats.MaxSpeed;
            
            // ------------------ SET SIZE VARIATION -------------------------- //
            float odds = Random.Range(0, 1f);
            print("Gamble A: " + odds);
            odds -= enemyStats.SmallStatsModifier.Probability;
            //Small
            if (odds < 0)
            {
                SetSizeDependentStats(enemyStats.SmallStatsModifier);
                return;
            }
            print("Gamble B: " + odds);
            odds -= enemyStats.LargeStatsModifier.Probability;
            if (odds < 0)
            {
                SetSizeDependentStats(enemyStats.LargeStatsModifier);
                return;
            }
            print("Gamble C: " + odds);
            odds -= enemyStats.MiniBossStatsModifer.Probability;
            if (odds < 0)
            {
                SetSizeDependentStats(enemyStats.MiniBossStatsModifer);
            }
            print("Gamble D (Failed): " + odds);
        }

        private void SetSizeDependentStats(StatType s)
        {
            damage *= s.DamageScalar;
            maxHealth *= s.HealthScalar;
            agent.speed *= s.SpeedScalar;
            transform.localScale *= s.ScaleScalar;
            attackDist *= s.ScaleScalar;
        }

        protected override void Heal(float amount)
        {
            source.PlayOneShot(stats.HealSound);
            curHealth = Mathf.Min(maxHealth, curHealth + amount);
        }

        // Update is called once per frame
        protected override void TrueUpdate()
        {
            base.TrueUpdate();
            
            // If possible, move towards player
            if (agent)
            {
                //Convert to plane because this system is garbage
                //If close enough to damage player
                curTimeBetweenAttacks -= Time.deltaTime;
                if (curTimeBetweenAttacks > 0)
                    return;
                
                if (curTimeBetweenAttacks < 0 && CanSeePlayer() < attackDist)
                {
                    print("I attacked the player!");
                    curTimeBetweenAttacks = enemyStats.TimeBetweenAttacks;
                    agent.SetDestination(transform.position);
                    animator.SetTrigger(attackAnimID);
                    return;
                }
                
                //
                
                print($"Travelling to: {transform.position} --> {agent.destination},  {agent.pathStatus}");
                //Because this is absolute trash and wont work properly sometimes without this :))))
                animator.SetBool(walkAnimID, agent.pathStatus != NavMeshPathStatus.PathComplete);

                if (targetWasVisible)
                {
                    targetTime -= Time.deltaTime;
                    if (targetTime < 0)
                    {
                        targetWasVisible = false;
                        targetTime = MaxTargetTime;
                        StartCoroutine(StartIdle());
                    }
                    else
                    {
                        agent.SetDestination(Vector3.ProjectOnPlane(ply.transform.position, transform.up));
                    }
                }
                
                //Player is in range
                Vector3 dir = ply.transform.position - head.position;
                //If the player is in range
                if (dir.magnitude < enemyStats.MaxEyeDist &&
                    Mathf.Abs(Vector3.Angle(dir, head.up)) < enemyStats.EyeAngle)
                {
                    #if UNITY_EDITOR
                    Debug.DrawRay(transform.position, dir, Color.blue);
                    #endif
                    // If can see player.
                    float x = CanSeePlayer();
                    print("LOOKING: " + x);
                    if (x < enemyStats.MaxEyeDist)
                    {
                        
#if UNITY_EDITOR
                        Debug.DrawRay(transform.position, dir, Color.red);
#endif
                        targetWasVisible = true;
                        targetTime = MaxTargetTime;
                        agent.SetDestination(Vector3.ProjectOnPlane(ply.transform.position, transform.up));
                        print("Setting destination" + transform.position +" --> " + agent.destination );
                        targeting = true;
                    }
                }
            }
        }

        private IEnumerator StartIdle()
        {
            print("Idling");
            agent.SetDestination(transform.position);
            targeting = false;
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
            if (!targeting)
            {
                ChooseRandomSpot();
            }
        }

        protected virtual void ChooseRandomSpot() // Also used for going towards mushrooms to eat and whatnot
        {
            StartCoroutine(StartIdle());
            Vector3 moveTo = Vector3.ProjectOnPlane( transform.position + Random.insideUnitSphere * enemyStats.MaxEyeDist, transform.up);
            //Physics.Raycast(transform.position, moveTo, out RaycastHit h, enemyStats.MaxTargetDistance, ~gameObject.layer);
            agent.SetDestination(moveTo);
            print("Choosing Random Location" + transform.position + " --> " + agent.destination);
        }
        
        private float CanSeePlayer()
        {
            Vector3 position = head.position;
            bool a = Physics.Raycast(position, (ply.transform.position - position), out RaycastHit hit, enemyStats.MaxEyeDist);
            bool b = 1 << hit.transform.gameObject.layer == GameManager.Instance.PlayerLayer;
            print("DEBUG: " + a + " + " + b);
            
            if (!a || !b)
                return 100;
            return hit.distance;
        }

        protected virtual void AttackHit()
        {
            print("ATTACK HIT");
            Vector3 position = head.position;
            Vector3 dir = ply.transform.position - position;
            if (  Physics.Raycast(position, dir, out RaycastHit hit,
                    attackDist * 2, GameManager.Instance.PlayerLayer) &&
                  hit.transform.TryGetComponent(out Player p))
            {
                p.TakeDamage(this, damage, dir.normalized *  4);
            }
        }
        
        protected virtual void ContinueLogic()
        {
            print("ATTACK FINISHED");
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
