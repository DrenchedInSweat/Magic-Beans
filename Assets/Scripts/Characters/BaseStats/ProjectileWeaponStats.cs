using Characters.Upgrades;
using UnityEngine;
using Weapons;

namespace Characters.BaseStats
{
    //Having this be a root class allows for enemies to be more optimized.
    [CreateAssetMenu(menuName = "Game/Stats/ProjectileWeaponStats", fileName = "ProjectileWeaponStats", order = 2)]
    public class ProjectileWeaponStats : WeaponStatsSo
    {

        [field: Header("Shooting")]
        [field: SerializeField] public Projectile Projectile { get; private set; }

        [field:SerializeField] public float BulletSpeed { get; private set; }
        [field:SerializeField] public float RandomSpread { get; private set; }
        [field:SerializeField] public float AreaOfEffect { get; private set; }
        [field:SerializeField] public int RecursionFactor { get; private set; }
        
        [field:Header("Only affects player right now")]
        [field: SerializeField] public Vector2 Recoil { get; private set; }
        [field:SerializeField] public float RecoilMultiplier { get; private set; }
        
        
        protected override void AddUpgrade(WeaponUpgradeSo upgradeSo)
        {
            base.AddUpgrade(upgradeSo);
            ProjectileWeaponUpgradeSo stats = (ProjectileWeaponUpgradeSo)upgradeSo;
            if (stats.Projectile)
                Projectile = stats.Projectile;
            BulletSpeed += stats.BulletSpeed;
            RandomSpread += stats.RandomSpread;
            AreaOfEffect += stats.AreaOfEffect;
            RecursionFactor += stats.RecursionFactor;
        }

        protected override void MultiplyUpgrade(WeaponUpgradeSo upgradeSo)
        {
            base.MultiplyUpgrade(upgradeSo);
            ProjectileWeaponUpgradeSo stats = (ProjectileWeaponUpgradeSo)upgradeSo;
            if (stats.Projectile)
                Projectile = stats.Projectile;
            BulletSpeed *= stats.BulletSpeed;
            RandomSpread *= stats.RandomSpread;
            AreaOfEffect *= stats.AreaOfEffect;
            RecursionFactor *= stats.RecursionFactor;
        }
    }
}
