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
        [SerializeField] protected LaserWeaponStats stats;
        
        private LaserWeaponVFX myWeapon;
        private VisualEffect flash;
        [SerializeField, ColorUsage(true, true)]
        private Color col;
        //TODO: move
        public delegate void ApplyToCharacter(Character victim);

        private ApplyToCharacter onHit;

        private readonly int delayID = Shader.PropertyToID("Delay");
        private readonly int startID = Shader.PropertyToID("StartFire");
        private readonly int stopID = Shader.PropertyToID("StopFire");
        private readonly int colorID = Shader.PropertyToID("Color");
        private void Start()
        {
            myWeapon = transform.GetChild(0).GetComponent<LaserWeaponVFX>();
            flash = GetComponent<VisualEffect>();
            flash.SetFloat(delayID, stats.TimeBetweenShots);
            flash.SetVector4(colorID, col);
            onHit += victim =>
            {
                print("Killing Enemy");
                victim.TakeDamage(owner, stats.Damage);
            };
        }

        protected override void StartFire()
        {
            print("Start Beam");
            StartCoroutine(StartBeam());
        }


        protected override void StopFire()
        {
            print("Stop Beam");
            flash.SendEvent(stopID);
            flash.Stop();
            myWeapon.gameObject.SetActive(false);
            myWeapon.DeActivate();
        }

        private IEnumerator StartBeam()
        {
            flash.SendEvent(startID);
            yield return new WaitForSeconds(stats.TimeBetweenShots);
            myWeapon.gameObject.SetActive(true);
            myWeapon.Activate(col, onHit);
        }

        //I'm lazy
        protected override void TryShoot()
        {
        
        }
        
        protected override bool CanShoot()
        {
            //TODO add check, cannot shoot in safe zones, or if hand is being used to wall run.
            return (curShotTime > stats.TimeBetweenShots && stats.ProjectilesFired > 0);
        }

        public override void Upgrade<T>(T upgrade)
        {
            stats.Upgrade(upgrade);
        }

        public override T GetStats<T>()
        {
            return stats as T;
        }
    }
}
