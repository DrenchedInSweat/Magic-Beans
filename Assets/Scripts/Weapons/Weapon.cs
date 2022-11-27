using Characters;
using Characters.BaseStats;
using Characters.Upgrades;
using UnityEngine;

namespace Weapons
{
    public enum ESprayPattern
    {
        Circle, 
        Line,
        Random,
        Star,
        
    }

    public abstract class Weapon : MonoBehaviour
    {
        //public int ProjectilesFired => projectilesFired;
        protected Character owner;

        public bool tryingToShoot;
        private bool isShooting;
        
        protected float curShotTime;
        private WeaponStatsSo stats;


        public void Init(Character myOwner)
        {
            owner = myOwner;
            tryingToShoot = false;
            stats = GetStats<WeaponStatsSo>();
        }

        private void Update()
        {
            curShotTime += Time.deltaTime;

            if (tryingToShoot)
            {
                if (!isShooting)
                {
                    isShooting = true;
                    owner.MultSpeed(stats.SlowDown);
                    StartFire();
                }
                TryShoot();
            }
            else if (isShooting)
            {
                isShooting = false;
                owner.MultSpeed(1 / stats.SlowDown);
                StopFire();
            }
        }
        
        protected abstract void StartFire();
        protected abstract void StopFire();
        protected abstract void TryShoot();

        protected abstract bool CanShoot();
        //Because I can
        public abstract void Upgrade<T>(T upgrade) where T : WeaponUpgradeSo;
        public abstract T GetStats<T>() where T : WeaponStatsSo;
        
    }
}