using System;
using UnityEditor;

namespace Editor
{
    [CanEditMultipleObjects]
    public class CustomInspectorParent : UnityEditor.Editor
    {
        protected SerializedProperty spawnChance;
        private void OnEnable()
        {
            throw new NotImplementedException();
        }
        
        
    }
}

