using System;
using UnityEngine;


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


