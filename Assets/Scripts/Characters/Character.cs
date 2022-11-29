using Characters.Upgrades;
using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(Collider), typeof(AudioSource))] // All are enemies going to have Rigids?
    public class Character : MonoBehaviour
    {
        [Header("Character Information")]
        [SerializeField] protected CharacterStatsSo stats;
        
        protected const float MaxJumpTime = 0.2f;
        protected float jumpTime;
        protected float curHealth;
        private float curWalkTime;
        
        protected int curJump;
        
        protected Vector3 directionVector;
        
        protected bool isGrounded;
        protected bool canAttack = true;
        protected bool isMoving;

        [SerializeField] protected  Animator animator;
        protected AudioSource source;

        protected float speed;
        protected float maxSpeed;

        protected readonly int walkAnimID = Animator.StringToHash("IsMoving");
        protected readonly int attackAnimID = Animator.StringToHash("Attack");
        
    
        // Start is called before the first frame update
        protected virtual void Awake()
        {
            source = GetComponent<AudioSource>();
            curHealth = stats.MaxHealth;
            
            speed = stats.MoveSpeed;
            maxSpeed = stats.MaxSpeed;
        }
        

        protected virtual void Update()
        {
            jumpTime += Time.deltaTime;

            Move();
            CheckFloor();
            animator.SetBool(walkAnimID, isMoving);
            if (isMoving)
            {
                curWalkTime += Time.deltaTime;
                if (isGrounded && curWalkTime > stats.WalkSoundDelay){
                    source.PlayOneShot(stats.WalkSound, 0.1f);
                    curWalkTime = 0;
                }
            }
            // TODO: move somewhere more appropriate
            if(Time.timeScale == 0)
                source.Stop();

        }

        /// <summary>
        /// Check for allowed floors below... 
        /// </summary>
        private void CheckFloor()
        {
            Transform t = transform;
            isGrounded = Physics.Raycast(t.position + stats.FeetCenter, -t.up, stats.Range, stats.FloorLayers);

            #if UNITY_EDITOR
            Debug.DrawRay(t.position + stats.FeetCenter, -t.up * stats.Range, isGrounded?Color.green:Color.red);
            #endif
        
            if (isGrounded && jumpTime > MaxJumpTime)
            {
                curJump = 0;
            }
        }



        /// <summary>
        /// Makes a character take damage. Attacker can be used to handle achievements
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="amount"></param>
        /// <param name="force"></param>
        public virtual void TakeDamage(Character attacker, float amount)
        {
            print("HELP ME: " + gameObject.name + " I'm being killed by: " + attacker.gameObject.name);
            curHealth -= amount;
            if (curHealth < 0)
            {
                Die(attacker, amount);
                return;
            }
            source.PlayOneShot(stats.HurtSound);
        }

        public virtual void TakeDamage(Character attacker, float amount, Vector3 force)
        {
            print("HELP ME: " + gameObject.name + " I'm being killed by: " + attacker.gameObject.name);
            curHealth -= amount;
            if (curHealth < 0)
            {
                Die(attacker, amount);
                return;
            }
            source.PlayOneShot(stats.HurtSound);
        }

        /// <summary>
        /// Kills and deletes a Character after a set time
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="amount"></param>
        /// TODO: Play animation / Ragdoll --> Sink into floor --> Delete
        protected virtual void Die(Character attacker, float amount)
        {
            source.PlayOneShot(stats.DieSound);
            Destroy(gameObject);
        }
    
        /// <summary>
        /// Kills and deletes a Character after a set time
        /// </summary>
        /// <param name="amount"></param>
        /// TODO: Play animation / Ragdoll --> Sink into floor --> Delete
        protected virtual void Heal(float amount)
        {
            source.PlayOneShot(stats.HealSound);
            curHealth = Mathf.Min(stats.MaxHealth, curHealth + amount);
        }

        protected virtual void Move()
        {
        
        }

        protected bool CanJump()
        {
            return jumpTime >= MaxJumpTime && curJump++ < stats.MaxJumps;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            //If hitting another character 
            if (collision.transform.TryGetComponent(out Character c))
            {
                c.TakeDamage(this, stats.ContactDamage, -collision.impulse);
            }
        }

        public virtual void UpgradeCharacter(CharacterUpgradeSo upgrade)
        {
            stats.UpgradeCharacter(upgrade);
            curHealth = stats.MaxHealth;
            source.PlayOneShot(stats.UpgradeSound);
            speed = stats.MoveSpeed;
            maxSpeed = stats.MaxSpeed;
        }


#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position + stats.FeetCenter, -transform.up * stats.Range);
        }
#endif
        public void MultSpeed(float statsSlowDown)
        {
            maxSpeed *= statsSlowDown;
            speed *= statsSlowDown;
        }

        public void SetLoopedNoise(AudioClip noise)
        {
            if (!noise)
            {
                source.Stop();
                return;
            }

            source.clip = noise;
            source.loop = true;
            source.Play();
        }
    }
}
