using Characters.Upgrades;
using UnityEngine;
using Weapons;

namespace Characters.BaseStats
{
    
    public abstract class WeaponStatsSo : ScriptableObject
    {
        //--------------------------------- UI ---------------------------------//
        [field: Header("UI")]
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public string Name { get; private set; }//TODO: May be unnecessary.
        [field: SerializeField, Multiline] public string Description { get; private set; }//TODO: May be unnecessary.
        
        //--------------------------------- HANDLING ---------------------------------//
        [field: Header("Handling")]
        [field: SerializeField] public float TimeBetweenShots { get; private set; }
        
        //--------------------------------- SHOOTING ---------------------------------//
        [field: Header("Shooting")]
        [Tooltip("This is the formation of the bullets")]
        [field:  SerializeField] public ESprayPattern SprayPattern { get; private set; }
        
        [Tooltip("WIP If the bullets should move angularly like a shotgone, or directly like a mastiff")]
        [field:  SerializeField] public bool IsAngular { get; private set; } //TODO implement
        
        [field: SerializeField] public float Damage { get; private set; }
        
        [Tooltip("This is how many bullets are shot")]
        [field:  SerializeField] public int ProjectilesFired { get; private set; }

        //Will this work polymorphically?
        public void Upgrade(WeaponUpgradeSo upgradeSo)
        {
            SprayPattern = upgradeSo.SprayPattern;
            IsAngular = upgradeSo.IsAngular;
            
            if (upgradeSo.Modifier == EModifier.Add)
                AddUpgrade(upgradeSo);
            else
                MultiplyUpgrade(upgradeSo);
        }

        protected virtual void AddUpgrade(WeaponUpgradeSo upgradeSo)
        {
            TimeBetweenShots += upgradeSo.TimeBetweenShots;
            Damage += upgradeSo.Damage;
            ProjectilesFired += upgradeSo.ProjectilesFired;
        }
        protected virtual void MultiplyUpgrade(WeaponUpgradeSo upgradeSo)
        {
            TimeBetweenShots *= upgradeSo.TimeBetweenShots;
            Damage *= upgradeSo.Damage;
            ProjectilesFired *= upgradeSo.ProjectilesFired;
        }
    }
}
