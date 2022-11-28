using UnityEngine;
using Weapons;

namespace Characters.Upgrades
{
    [CreateAssetMenu(menuName = "Game/Upgrades/ProjectileWeaponUpgrade", fileName = "ProjectileUpgrade", order = 4)]
    public class ProjectileWeaponUpgradeSo : WeaponUpgradeSo
    {
        [field: Header("Shooting")]
        [field: SerializeField] public Projectile Projectile { get; private set; }

        [field:SerializeField] public float BulletSpeed { get; private set; }
        [field:SerializeField] public float RandomSpread { get; private set; }
        [field:SerializeField] public float AreaOfEffect { get; private set; }
        [field:SerializeField] public int RecursionFactor { get; private set; }
    }
}
