using System;
using System.Collections.Generic;
using Characters.Upgrades;
using UnityEditor;
using UnityEngine;

namespace Characters.BaseStats
{
    [CreateAssetMenu(menuName = "Game/Stats/CharacterStats", fileName = "CharacterStats", order = 1)]
    public class CharacterStatsSo : ScriptableObject
    {
        //Scriptable objects means that data usage decreases. It also means that changes to the SO are only kept during this current run!
        //In other words, saving then reapplying the SO should work.
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
        public Queue<CharacterUpgradeSo> StoredUpgrades { get; private set; }
        
        //----------------------Floor Stuff--------------------//
        [field: Header("Floor Stuff")]
        [field:SerializeField] public LayerMask FloorLayers { get; private set; }
        [Tooltip("The transform where the center of the footsies are")]
        [field:SerializeField] public Vector3 FeetCenter { get; private set; }
        [Tooltip("Range of the footsies")]
        [field:SerializeField] public float Range { get; private set; }

        //----------------------Audio------------------//
        [field: Header("Audio")]
        [field:SerializeField] public AudioClip HurtSound { get; private set; }
        [field:SerializeField] public AudioClip DieSound { get; private set; }
        [field:SerializeField] public AudioClip HealSound { get; private set; }
        [field:SerializeField] public AudioClip WalkSound { get; private set; }
        [field:SerializeField] public AudioClip UpgradeSound { get; private set; }
        [field:SerializeField, Min(0)] public float WalkSoundDelay { get; private set; }
        
        //----------------------Upgrading------------------//
        public void UpgradeCharacter(CharacterUpgradeSo upgrade)
        {
            StoredUpgrades ??= new Queue<CharacterUpgradeSo>();
            StoredUpgrades.Enqueue(upgrade);
            if (upgrade.Modifier == EModifier.Add)
            {
                AddUpgrade(upgrade);
                return;
            }
            MultiplyUpgrade(upgrade);
        }
        
        
        
        #if UNITY_EDITOR

        private void OnEnable()
        {
            Debug.LogWarning("Saved state");
            //EditorUtility.SetDirty(this);
            Undo.ClearUndo(this);
            Undo.RegisterCompleteObjectUndo(this, name);
        }

        private void OnValidate()
        {
            Debug.LogWarning("Saved state MANUAL");
            //EditorUtility.SetDirty(this);
            Undo.ClearUndo(this);
            Undo.RegisterCompleteObjectUndo(this, name);
            
        }

        private void OnDisable()
        {
            Debug.LogWarning("Undoing save");
            Undo.PerformUndo();
        }

        private void OnDestroy()
        {
            Debug.LogWarning("Undoing save");
            Undo.PerformUndo();
        }
#endif

        //TODO: This may cause problems:
        //1 Because SO, it would affect all of the same creature...
        
        #region Corny Upgrades
        private void AddUpgrade(CharacterUpgradeSo upgrade)
        {
            MoveSpeed += upgrade.MoveSpeed;
            MaxSpeed += upgrade.MaxSpeed;
            FloorDrag += upgrade.FloorDrag;
            AirDrag += upgrade.AirDrag;
            JumpForce += upgrade.JumpForce;
            MaxJumps += upgrade.MaxJumps;
            MaxHealth += upgrade.MaxHealth;
            ContactDamage += upgrade.ContactDamage;
        }
        private void MultiplyUpgrade(CharacterUpgradeSo upgrade)
        {
            MoveSpeed *= upgrade.MoveSpeed;
            MaxSpeed *= upgrade.MaxSpeed;
            FloorDrag *= upgrade.FloorDrag;
            AirDrag *= upgrade.AirDrag;
            JumpForce *= upgrade.JumpForce;
            MaxJumps *= upgrade.MaxJumps;
            MaxHealth *= upgrade.MaxHealth;
            ContactDamage *= upgrade.ContactDamage;
        }
        
        
        #endregion
    }
}
