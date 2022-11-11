using System;
using UnityEngine;
using Random = UnityEngine.Random;

    public enum SpawnSize
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge
    }

    [Flags]
    public enum Type
    {
        Props=1,
        Collectables=2,
        Enemies=4
    }


    public class RandomPlacement : MonoBehaviour
    {
        [SerializeField, Tooltip("This also changes what objects can be put in here")] private SpawnSize spawnSize;
        [SerializeField, Tooltip("This determines what the spawner will contain")] private Type type;
        [SerializeField, Tooltip("This determines the creatures based on any selected stages")] private CurrentStage stage;
        [SerializeField, Tooltip("Chance for this node to activate")] private float chanceToSpawn;
        [SerializeField, Tooltip("Objects to be spawned")] private int minObjectsToSpawn;
        [SerializeField, Tooltip("Objects to be spawned")] private int maxObjectsToSpawn;

        private void Awake()
        {
            if (Random.value < chanceToSpawn)
            {
                //Spawn the entity
                //DeferredManager.
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            throw new NotImplementedException();
        }
        #endif
    }

