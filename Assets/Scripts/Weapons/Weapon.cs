using System;
using Characters;
using Characters.BaseStats;
using Characters.Upgrades;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapons
{
    public enum ESprayPattern
    {
        Circle, 
        Line,
        Random,
        Star
    }

    public abstract class Weapon : MonoBehaviour
    {
        //public int ProjectilesFired => projectilesFired;
        protected Character owner;

        [NonSerialized] public bool tryingToShoot;
        [NonSerialized] public AudioClip idleClip;
        private bool isShooting;
        
        protected float curShotTime;
        private WeaponStatsSo defStats;
        
        private VisualEffect flash;
        private readonly int startID = Shader.PropertyToID("StartFire");
        private readonly int stopID = Shader.PropertyToID("StopFire");
        private readonly int delayID = Shader.PropertyToID("Delay");
        private readonly int colorID = Shader.PropertyToID("Color");
        [SerializeField] [GradientUsage(true)] protected Gradient gradient;
        public void Init(Character myOwner)
        {
            owner = myOwner;
            tryingToShoot = false;
            defStats = GetStats<WeaponStatsSo>();
            #if UNITY_EDITOR
                defStats = (WeaponStatsSo)defStats.Clone();
            #endif
            idleClip = defStats.IdleNoise;
            owner.SetLoopedNoise(idleClip);
            
            flash = GetComponent<VisualEffect>();
            flash.SetFloat(delayID, defStats.TimeBetweenShots);
            flash.SetGradient(colorID, gradient);
            
        }

        private void Update()
        {
            curShotTime += Time.deltaTime;

            if (tryingToShoot)
            {
                if (!isShooting)
                {
                    isShooting = true;
                    owner.MultSpeed(defStats.SlowDown);
                    StartFire();
                }
                TryShoot();
            }
            else if (isShooting)
            {
                isShooting = false;
                owner.MultSpeed(1 / defStats.SlowDown);
                StopFire();
            }
        }

        protected virtual void StartFire()
        {
            print("Started firing");
            flash.SendEvent(startID);
            owner.SetLoopedNoise(defStats.FireNoise);
        }

        protected virtual void StopFire()
        {
            print("Stopped firing");
            owner.SetLoopedNoise(defStats.IdleNoise);
            flash.Stop();
            flash.SendEvent(stopID);
        }

        protected abstract void TryShoot();

        protected abstract bool CanShoot();
        //Because I can
        public abstract void Upgrade<T>(T upgrade) where T : WeaponUpgradeSo;
        public abstract T GetStats<T>() where T : WeaponStatsSo;
        
    }
}