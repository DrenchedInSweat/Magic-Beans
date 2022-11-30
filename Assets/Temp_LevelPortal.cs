using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using UIandMenu;
using UnityEngine;
using Random = UnityEngine.Random;

public class Temp_LevelPortal : MonoBehaviour
{
    [SerializeField] private AbilityScreen screen;
    [SerializeField] private Transform TelToCam; // Teleport to this cam
    
    [SerializeField] private UpgradeBaseSo[] upgrades;
    [SerializeField] private Transform canvas;

    private void Awake()
    {
        
    }


    private UpgradeBaseSo[] GenerateUpgradeSet(int size)
    {
        //Create array
        UpgradeBaseSo[] ups = new UpgradeBaseSo[Mathf.Min(size, upgrades.Length)];
        List<UpgradeBaseSo> old = upgrades.ToList();
        for (int i = 0; i < ups.Length; i++)
        {
            int idx = Random.Range(0, old.Count);
            print("Creating upgrade: " + idx);
            ups[i] = old[idx];
            old.RemoveAt(idx);
        }

        return ups;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player p))
        {
            print("generating Upgrades");
            int len = 3;
            Instantiate(screen, canvas).Init(p, GenerateUpgradeSet(len), len);
        }
    }
    
    

}
