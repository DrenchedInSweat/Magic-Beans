using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace Characters.BaseStats
{
    [CreateAssetMenu(menuName = "Game/Stats/EnemyStats", fileName = "EnemyStats", order = 2)]
    public class EnemyStatsSo : ScriptableObject
    {
        [field: Header("UI/UX", order = 1)]
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        
        [field: Header("VariantStats")]
        [field: SerializeField] public StatType SmallStatsModifier { get; private set; }
        [field: SerializeField] public StatType LargeStatsModifier { get; private set; }
        [field: SerializeField] public StatType MiniBossStatsModifer { get; private set; }
        
        [field: Header("Targeting")]
        [Tooltip("Distance the enemy can be from the player to begin targeting")]
        [field: SerializeField, Min(0)] public float MaxTargetDistance { get; private set; }
        
        [Tooltip("How close the player can get before getting hit")]
        [field: SerializeField, Min(0)] public float AttackDist { get; private set; }
        
        [Tooltip("How close the player can get before getting hit")]
        [field: SerializeField, Min(0)] public float TimeBetweenAttacks { get; private set; }
        
        [Tooltip("How far the eyes can see")]
        [field: SerializeField, Min(0)] public float MaxEyeDist { get; private set; }
        [Tooltip("Angles of the eyes")]
        [field: SerializeField, Range(0, 180)] public float EyeAngle { get; private set; }
        
        [Tooltip("Rotation Speed for orientation in Degs per second")]
        [field: SerializeField, Range(0, 180)] public float RotationSpeed { get; private set; }
        
        [field: Header("WIP")]
        [Tooltip("How close the player can get before it just targets")]
        [field: SerializeField, Min(0)] public float TargetMinDistance { get; private set; }

        
    }

    [Serializable]
    public struct StatType
    {
        [Tooltip("NOTE: Probability of each should sum to smaller than 1. Remainder is chance that it's normal.")]
        [field: SerializeField, Range(0,1)] public float Probability { get; private set; }
        [field: SerializeField, Min(0)] public float SpeedScalar { get; private set; }
        [field: SerializeField, Min(0)] public float HealthScalar { get; private set; }
        [field: SerializeField, Min(0)] public float DamageScalar { get; private set; }
        [field: SerializeField, Min(0)] public float TimeBetweenAttacksScalar { get; private set; }
        [field: SerializeField, Min(0)] public float ScaleScalar { get; private set; }
        [field: SerializeField, Min(0)] public UnityEvent AdditionalEvent { get; private set; }
        [field: SerializeField, Min(0)] public VisualEffect AdditionalEffect { get; private set; }
    }
}
