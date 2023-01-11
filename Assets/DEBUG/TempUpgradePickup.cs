using System;
using UnityEngine;
using UnityEngine.VFX;

public class TempUpgradePickup : MonoBehaviour
{
    private readonly int activateEvent = Shader.PropertyToID("Activate");
    private readonly int targetPosition = Shader.PropertyToID("Target");
    private bool activated;
    private VisualEffect fx;
    private Transform target;
    
    
    
    private void Awake()
    {
        fx = GetComponent<VisualEffect>();
        target = GameManager.Instance.Player.transform;
    }

    private void Update()
    {
        if(activated)
            fx.SetVector3(targetPosition, target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        activated = true;
        fx.SendEvent(activateEvent);
        Destroy(gameObject, 5);
    }

    private void OnDestroy()
    {
        if (!activated) return;
        print("Display Upgrade Menu");
    }
}
