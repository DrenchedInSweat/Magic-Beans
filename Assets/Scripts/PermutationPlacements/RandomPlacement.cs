using System;
using Managers;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

//TODO: Generate spawnable objects in bounds
//TODO: Add remaining patterns
//TODO: Make the nodes spawn on spawn
    [ExecuteInEditMode]
    public class RandomPlacement : MonoBehaviour
    {

        [Serializable]
        private enum ESpawnLayouts
        {
            RandomSphere,
            Line,
            FlatCircle,
            FlatStar,
            Sphere,
            Box,
            SineWave
        }

        [Header("Parameters")]
        [SerializeField, Tooltip("This is automatic, if this is not changing, you have two level cores.")] private LevelCore core;
        [SerializeField, Tooltip("This determines what the spawner will contain")] private EType type;
        [SerializeField, Tooltip("This determines the creatures based on any selected stages")] private ESpawnLayouts layout;
        [SerializeField, Tooltip("Chance for this node to activate"), Range(0, 1)] private float chanceToSpawn;

        [Header("Spawning")]
        [SerializeField, Tooltip("MAX Objects to be spawned in range"), Min(0)] private int maxObjectsToSpawn;
        [SerializeField, Tooltip("MAX size of objects spawned"),Min(0)] private float maxSizeOfObjects;
        [SerializeField, Tooltip("MIN size of objects spawned"), Min(0)] private int minObjectsToSpawn;
        [SerializeField, Tooltip("MIN Objects to be spawned in range"), Min(0)] private float minSizeOfObjects;
        [SerializeField, Tooltip("Dist between Spawns")] private float distBetweenSpawns;
        [SerializeField, Tooltip("If true, can only spawn one type")] private bool mustBeMatching;
        [SerializeField, Tooltip("What objects can spawn here")] private GameObject[] spawnableObjects;
        private Vector3[] spawnPoints;

        private void Start()
        {
            GeneratePoints(); // I suspect unless spawnPoints are serialized, it will need to be like this.
            if (Random.value < chanceToSpawn)
            {
                int num = Random.Range(minObjectsToSpawn, maxObjectsToSpawn);
                GameObject thingToSpawn = core.SpawnObject(type);
                //Generate Array
                for (int i = 0; i < num; ++i)
                {
                   GameObject go = Instantiate(thingToSpawn, spawnPoints[i], Quaternion.identity);
                   go.name = $"{gameObject.name} [{i}] --> {go.name}";
                   if (!mustBeMatching) 
                       thingToSpawn = core.SpawnObject(type);
                }
            }
        }

        private void GeneratePoints()
        {
            spawnPoints = new Vector3[maxObjectsToSpawn];
            Vector3 p = transform.position;
            switch (layout)
            {
                case ESpawnLayouts.RandomSphere:
                    for(int i = 0; i < maxObjectsToSpawn; ++i)
                    {
                        spawnPoints[i] = p + Random.insideUnitSphere * distBetweenSpawns;
                    }
                    break;
                case ESpawnLayouts.Line:
                    for(int i = 0; i < maxObjectsToSpawn; ++i)
                    {
                        spawnPoints[i] = p + ((i & 1) == 0?-1:1) * distBetweenSpawns * ((i + 1)/2) * transform.forward  ;
                    }
                    break;
                case ESpawnLayouts.FlatCircle:

                    float radsBetween = 360 / distBetweenSpawns * Mathf.Deg2Rad;
                    
                    for(int i = 0; i < maxObjectsToSpawn; ++i)
                    {
                        spawnPoints[i] = p + transform.forward * distBetweenSpawns;
                    }
                    break;
            }
            
            
            
        }
        
        
        #if UNITY_EDITOR
        [SerializeField] private bool displayAlways;
        
        private Quaternion prvRot;
        private Vector3 prvPos;
        private void OnDrawGizmos()
        {
            
            Vector3 p = transform.position;
            switch (type)
            {
                case EType.Collectables:
                    if(core.showCollectableSpawns)
                        Gizmos.DrawIcon(p, core.collectableTexture, true);
                    break;
                case EType.Enemies:
                    if(core.showEnemySpawns)
                        Gizmos.DrawIcon(p, core.enemyTexture, true);
                    break;
                case EType.Props:
                    if(core.showPropsSpawns)
                        Gizmos.DrawIcon(p, core.propsTexture, true);
                    break;
                case EType.FocalPoints:
                    if(core.showFocalPointsSpawns)
                        Gizmos.DrawIcon(p, core.focalPointTexture, true);
                    break;
            }

            if (displayAlways && Selection.activeGameObject != gameObject)
            {
                for (int i = 0; i < maxObjectsToSpawn; i++)
                {
                    Vector3 n = spawnPoints[i];
                    Color col = i < minObjectsToSpawn ? new Color(0, 1f, 0, 1) : new Color(1f, 0.5f, 0, 1);
                    Gizmos.color = col;
                    Gizmos.DrawWireSphere(n, maxSizeOfObjects);
                    Gizmos.color = col * 0.7f;
                    Gizmos.DrawSphere(n, minSizeOfObjects);
                }
            }
        }

        private ESpawnLayouts old;
        private float oldDist;
        private void OnDrawGizmosSelected()
        {
            if (!core)
                core = FindObjectOfType<LevelCore>();

            if (maxObjectsToSpawn < minObjectsToSpawn )
            {
                minObjectsToSpawn = maxObjectsToSpawn;
            }

            if (minSizeOfObjects > maxSizeOfObjects)
                maxSizeOfObjects = minSizeOfObjects;
            

            if (spawnPoints.Length != maxObjectsToSpawn || old != layout || prvRot != transform.rotation || oldDist != distBetweenSpawns || prvPos != transform.position)
            {
                GeneratePoints();
                old = layout;
                prvRot = transform.rotation;
                oldDist = distBetweenSpawns;
                prvPos = transform.position;
            }
            
            Vector3 p = transform.position;
            for (int i = 0; i < maxObjectsToSpawn; i++)
            {
                Vector3 n = spawnPoints[i];
                Color col = i < minObjectsToSpawn ? new Color(0, 1f, 0, 1) : new Color(1f, 0.5f, 0, 1);
                Gizmos.color = col;
                Gizmos.DrawWireSphere(n, maxSizeOfObjects);
                Gizmos.color = col * 0.7f;
                Gizmos.DrawSphere(n, minSizeOfObjects);
            }
        }
        #endif
    }

