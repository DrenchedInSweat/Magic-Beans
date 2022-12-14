using System.Collections;
using Characters;
using Characters.BaseStats;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapons
{
    public class LaserWeapon : Weapon
    {
        [Header("Weapon Stats", order = 1)]
        [SerializeField] private LaserWeaponStats stats;
        [SerializeField] private LaserWeaponVFX laserPrefab;
        
        private LaserWeaponVFX myWeapon;
        
        
        public delegate void ApplyToCharacter(Character victim);

        private ApplyToCharacter onHit;

        
        
       
        private void Start()
        {
            
            onHit += victim =>
            {
                victim.TakeDamage(owner, stats.Damage);
            };
            SetChildBeams();
        }
        

        protected override void StartFire()
        {
            base.StopFire();
            StopAllCoroutines();
            StartCoroutine(StartBeam());
        }


        protected override void StopFire()
        {
            base.StopFire();
            myWeapon.DeActivate();
            myWeapon.gameObject.SetActive(false);
        }

        private IEnumerator StartBeam()
        {
            yield return new WaitForSeconds(stats.TimeBetweenShots);
            if (tryingToShoot)
            {
                myWeapon.gameObject.SetActive(true);
                myWeapon.Activate(gradient, onHit);
            }
        }

        private void SetChildBeams()
        {
            Transform prv = transform;
            for (int i = 0; i <= stats.Bounces; i++)
            {
                prv = Instantiate(laserPrefab, prv).transform;
            }

            myWeapon = transform.GetChild(0).GetComponent<LaserWeaponVFX>();
        }

        //I'm lazy
        protected override void TryShoot()
        {
            
        }
        
        protected override bool CanShoot()
        {
            //TODO add check, cannot shoot in safe zones, or if hand is being used to wall run.
            //return (curShotTime > stats.TimeBetweenShots && stats.ProjectilesFired > 0);
            return false;
        }

        public override void Upgrade<T>(T upgrade)
        {
            stats.Upgrade(upgrade);
            SetChildBeams();
        }

        public override T GetStats<T>()
        {
            return stats as T;
        }
    }
}
