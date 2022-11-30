using System;
using Characters;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum EType
    {
        Props,
        FocalPoints,
        Collectables,
        Enemies
    }

    [ExecuteInEditMode]
    public class LevelCore : MonoBehaviour
    {

        private SpawnInfo<Enemy>[] Enemies = Array.Empty<SpawnInfo<Enemy>>();
        private SpawnInfo<Collectable>[] Collectables = Array.Empty<SpawnInfo<Collectable>>();
        private SpawnInfo<Prop>[] Props = Array.Empty<SpawnInfo<Prop>>();
        private SpawnInfo<Prop>[] FocalPoints = Array.Empty<SpawnInfo<Prop>>();
        
#if UNITY_EDITOR
        //May break things...
        [SerializeField, Tooltip("This represents all the enemies that can be spawned in this section")]
        private Enemy[] enemies;

        [SerializeField, Tooltip("This represents all the collectables that can be spawned in this section")]
        private Collectable[] collectables;

        [SerializeField, Tooltip("This represents all the props that can be spawned in this section")]
        private Prop[] props;

        [SerializeField, Tooltip("This represents all the focalPoints that can be spawned in this section")]
        private Prop[] focalPoints;

        
        [Header("Customization")]
        public bool showEnemySpawns;
        public bool showCollectableSpawns;
        public bool showPropsSpawns;
        public bool showFocalPointsSpawns;

        public string enemyTexture;
        public string collectableTexture;
        public string propsTexture;
        public string focalPointTexture;
        
        
        private void Update()
        {
            
            if(Application.isPlaying)
                return;

            UpdateArray(ref Enemies, ref enemies);
            UpdateArray(ref Collectables, ref collectables);
            UpdateArray(ref Props, ref props);
            UpdateArray(ref FocalPoints, ref focalPoints);
        }

        private void UpdateArray<T>(ref SpawnInfo<T> []  main, ref T [] unityArr) where T : MonoBehaviour
        {
            
            if(unityArr == null)
                return;
            
            if (unityArr.Length != main.Length)
            {
                SpawnInfo<T>[] newArr = new SpawnInfo<T>[unityArr.Length];
                for (int index = 0; index < unityArr.Length; index++)
                {
                    T e = unityArr[index];
                    newArr[index] = new SpawnInfo<T>(e);
                }
                main = newArr;
            }
        }


        private void Awake()
        {
            UpdateArray(ref Enemies, ref enemies);
            UpdateArray(ref Collectables, ref collectables);
            UpdateArray(ref Props, ref props);
            UpdateArray(ref FocalPoints, ref focalPoints);
        }
        
#endif

        public GameObject SpawnObject(EType type)
        {
            switch (type)
            {
                case EType.Enemies:
                    return Enemies[Random.Range(0, Enemies.Length)].spawnObj.gameObject;
                case EType.Collectables:
                    return Collectables[Random.Range(0, Collectables.Length)].spawnObj.gameObject;
                case EType.Props:
                    return Props[Random.Range(0, Props.Length)].spawnObj.gameObject;
                case EType.FocalPoints:
                    return FocalPoints[Random.Range(0, FocalPoints.Length)].spawnObj.gameObject;
            }

            return null;
        }


        private struct SpawnInfo<T> where T : MonoBehaviour
        {
            public readonly Vector3 scale;
            public readonly T spawnObj;
            
            public SpawnInfo(T spawnObj)
            {
                scale = spawnObj.GetComponent<Renderer>().bounds.size;
                this.spawnObj = spawnObj;
            }

        }
    }

    
}