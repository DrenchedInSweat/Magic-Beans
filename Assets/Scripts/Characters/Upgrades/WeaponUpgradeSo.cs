using UnityEngine;
using Weapons;

namespace Characters.Upgrades
{
    public abstract class WeaponUpgradeSo : UpgradeBaseSo
    {
        
        //--------------------------------- HANDLING ---------------------------------//
        [field: Header("Handling")]
        [field: SerializeField] public float TimeBetweenShots { get; private set; }
        
        //--------------------------------- SHOOTING ---------------------------------//
        [field: Header("Shooting")]
        [Tooltip("This is the formation of the bullets")]
        [field:  SerializeField] public ESprayPattern SprayPattern { get; private set; }
        
        [Tooltip("If the bullets should move angularly like a shotgone, or directly like a mastiff")]
        [field:  SerializeField] public bool IsAngular { get; private set; }
        
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField, Range(0,1)] public float SlowDown { get; private set; }
        
        [Tooltip("This is how many bullets are shot")]
        [field:  SerializeField] public int ProjectilesFired { get; private set; }
        [field:  SerializeField] public int Bounces { get; private set; }
    }
}
