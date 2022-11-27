using System;
using UnityEngine;


//For my fellow programmers wondering why I chose to make this a SO
//This is the intended use for SOs as they store single instance data that can both be manipulate during runtime and act as single framework objects,
//This means that there are never multiple instances of these objects, but rather pointers to the original data container, that we can manipulate while testing.
//There are some disadvantages, meaning that upgrades are static

namespace Characters
{
    public enum EModifier
    {
        Add,
        Multiply
    }
    public abstract class UpgradeBaseSo : ScriptableObject
    {
        [field: Header("Utility")]
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, Multiline] public string Description { get; private set; }
        [field: SerializeField, Min(0)] public EModifier Modifier { get; private set; }
    }
}
