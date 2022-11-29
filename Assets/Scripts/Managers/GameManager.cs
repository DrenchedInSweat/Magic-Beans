using System;
using Characters;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}

        [SerializeField] private Transform bulletParent;
        public Transform BulletParent => bulletParent;

        [SerializeField] private LayerMask playerLayer;
        public LayerMask PlayerLayer => playerLayer;
    
        [SerializeField] private LayerMask enemyLayer;
        public LayerMask EnemyLayer => enemyLayer;
    
        [SerializeField] private LayerMask hittableLayers;
        public LayerMask HittableLayers => hittableLayers;
    
        [SerializeField] private LayerMask electricLayers;
        public LayerMask ElectricLayers => electricLayers;

        [field: SerializeField] public float SpawnScale { get; private set; }
        [field: SerializeField] public int Seed { get; private set; }
        [field: SerializeField] public Player Player { get; private set; }


        private void Awake()
        {
            if (Instance == null || Instance != this)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            DontDestroyOnLoad(gameObject);
            Random.InitState(Seed);
        }


// Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
    
    [Flags]
    public enum CurrentStage
    {
        Tutorial = 1,
        Cave = 2,
        IceCave = 4,
        GasCave = 8,
        MushroomCave = 16,
        Core = 32
    }


