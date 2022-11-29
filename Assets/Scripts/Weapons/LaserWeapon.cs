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
        private VisualEffect flash;
        [SerializeField, ColorUsage(true, true)]
        private Color col;
        public delegate void ApplyToCharacter(Character victim);

        private ApplyToCharacter onHit;

        private readonly int delayID = Shader.PropertyToID("Delay");
        private readonly int startID = Shader.PropertyToID("StartFire");
        private readonly int stopID = Shader.PropertyToID("StopFire");
        private readonly int colorID = Shader.PropertyToID("Color");
        private void Start()
        {
            flash = GetComponent<VisualEffect>();
            flash.SetFloat(delayID, stats.TimeBetweenShots);
            flash.SetVector4(colorID, col);
            onHit += victim =>
            {
                print("Killing Enemy");
                victim.TakeDamage(owner, stats.Damage);
            };
            SetChildBeams();
        }
        

        protected override void StartFire()
        {
            print("Start Beam");
            StopAllCoroutines();
            StartCoroutine(StartBeam());
        }


        protected override void StopFire()
        {
            print("Stop Beam");
            flash.SendEvent(stopID);
            flash.Stop();
            myWeapon.DeActivate();
            myWeapon.gameObject.SetActive(false);
        }

        private IEnumerator StartBeam()
        {
            flash.SendEvent(startID);
            yield return new WaitForSeconds(stats.TimeBetweenShots);
            if (tryingToShoot)
            {
                myWeapon.gameObject.SetActive(true);
                myWeapon.Activate(col, onHit);
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
