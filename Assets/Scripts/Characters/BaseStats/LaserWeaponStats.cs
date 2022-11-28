using Characters.Upgrades;
using UnityEngine;

namespace Characters.BaseStats
{
    [CreateAssetMenu(menuName = "Game/Stats/LaserWeaponStats", fileName = "LaserWeaponStats", order = 3)]
    public class LaserWeaponStats : WeaponStatsSo
    {
        
        protected override void AddUpgrade(WeaponUpgradeSo upgradeSo)
        {
            base.AddUpgrade(upgradeSo);
            LaserWeaponUpgradeSo stats = (LaserWeaponUpgradeSo)upgradeSo;

        }

        protected override void MultiplyUpgrade(WeaponUpgradeSo upgradeSo)
        {
            base.MultiplyUpgrade(upgradeSo);
            LaserWeaponUpgradeSo stats = (LaserWeaponUpgradeSo)upgradeSo;
        }
    }
}
