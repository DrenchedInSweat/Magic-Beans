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
        protected Transform atkPt;
        protected Character owner;

        public bool tryingToShoot;
        private bool isShooting;
        
        protected float curShotTime;


        public void Init(Character myOwner)
        {
            owner = myOwner;
            atkPt = owner.transform;
            tryingToShoot = false;
        }

        private void Update()
        {
            curShotTime += Time.deltaTime;

            if (tryingToShoot)
            {
                if (!isShooting)
                {
                    isShooting = true;
                    StartFire();
                }
                TryShoot();
            }
            else if (isShooting)
            {
                isShooting = false;
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