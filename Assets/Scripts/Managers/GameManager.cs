using System;
using System.Collections;
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
        
        public bool GameIsStopped { get; private set; } //Pause OR Upgrading
        public bool GameIsPaused { get; private set; } //Pause OR Upgrading
        
        
        // -------------------- AUDIO, May make sense to move to different file... --------------- //

        private float musicVolume;
        private float sfxVolume;
        private float masterVolume;

        public float MusicVolume => musicVolume * masterVolume;
        public float SfxVolume => sfxVolume * masterVolume;

        // -------------------- INPUT? ------------------------------------------------ // 
        public Action onPauseGamePaused;
        public Action onPauseGameUnpaused;

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

            //Random.InitState(Seed);
            Application.targetFrameRate = 120;
        }

        /// <summary>
        /// Stops and Pauses // Stops and Unpauses.
        /// </summary>
        public void ToggleStop()
        {
            GameIsPaused = GameIsStopped; // Above because will get toggled by toggle pause...?
            GameIsStopped = !GameIsStopped;
            SetLogic();
        }

        /// <summary>
        /// Pauses the game if the game is unlocked.
        /// </summary>
        public void TogglePause()
        {
            //Must run regardless.
            if(GameIsPaused)onPauseGameUnpaused.Invoke();
            else onPauseGamePaused.Invoke();
                
            if (GameIsStopped) return;
            //State change
            GameIsPaused = !GameIsPaused;

            SetLogic();
        }

        private void SetLogic()
        {
            Cursor.visible = GameIsPaused;
            if(GameIsPaused) {
                
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }
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


