using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    private int playerLayer;
    [SerializeField] private GameObject DEBUG_BigObject;
    private LevelCore oldLevel;
    
    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        oldLevel = transform.parent.GetComponent<LevelCore>();
        transform.parent = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        print($"HIT{other.gameObject.layer} vs {playerLayer}");
        if (other.gameObject.layer == playerLayer)
        {
            print("Unloading all previous assets");
            Destroy(oldLevel.gameObject);

            print("Loading new Assets");
            Instantiate(DEBUG_BigObject);
            
            Destroy(gameObject);
        }
    }
}
