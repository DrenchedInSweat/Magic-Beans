using System.Collections.Generic;
using System.Linq;
using Characters;
using UIandMenu;
using UnityEngine;
using Random = UnityEngine.Random;

public class Temp_LevelPortal : MonoBehaviour
{
    [SerializeField] private AbilityScreen screen;
    [SerializeField] private UpgradeBaseSo[] upgrades;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform antSpawn;

    [SerializeField] private GameObject [] antSets;
    [SerializeField] private Temp_LevelPortal previousPortal;

    [SerializeField] private AudioClip openNoise;
    
    private bool isActive;

    private Transform t;
        
    private void Update()
    {
        //if the previous portal is active
        if (previousPortal.isActive && previousPortal.t.childCount == 0)
        {
            previousPortal.isActive = false;
            transform.GetChild(1).gameObject.SetActive(false);
            AudioSource.PlayClipAtPoint(openNoise, transform.position, 1);
        }
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
            GameManager.Instance.ToggleStop();
            
            isActive = true;
            //Set front off, back on 
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            t = Instantiate(antSets[Random.Range(0, antSets.Length)], antSpawn).transform;
            t.LookAt(p.transform);
            
            print("generating Upgrades");
            int len = 3;
            Instantiate(screen, parent).Init(p, GenerateUpgradeSet(len), len);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player p))
        {
            //set both on
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
