using UnityEngine;

namespace Characters.Upgrades
{

    [CreateAssetMenu(menuName = "Game/Upgrades/CharacterUpgrade", fileName = "CharacterUpgrade", order = 3)]
    public class CharacterUpgradeSo : UpgradeBaseSo
    {
        //----------------------MOVEMENT--------------------//
        [field: Header("Movement")]
        [Tooltip("XZ move speed of the character")]
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; }
        
        [Tooltip("The Absolute speed limit of the object")]
        [field:SerializeField] public float MaxSpeed { get; private set; }

        [Tooltip("The drag against the player while on the ground (Limiting slide)")]
        [field:SerializeField] public float FloorDrag { get; private set; }
    
        [Tooltip("The drag against the player while on the ground (Limiting slide)")]
        [field:SerializeField] public float AirDrag { get; private set; }

        [Tooltip("Jump force of the character")]
        [field:SerializeField, Min(0)] public float JumpForce { get; private set; }

        [Tooltip("The number of jumps a character can preform from the ground. ")]
        [field:SerializeField, Min(1)] public int MaxJumps { get; private set; }
        
        //----------------------Player Stats------------------//
        [field: Header("Character Stats")]
        [Tooltip("Health after fully healing")]
        [field: SerializeField] public float MaxHealth {  get; private set; }
        
        [Tooltip("Damage an enemy takes when you hit them (Mario Stomp)")]
        [field: SerializeField] public float ContactDamage {  get; private set; }
    }
}
