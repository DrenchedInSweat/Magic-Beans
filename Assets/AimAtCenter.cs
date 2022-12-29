using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AimAtCenter : MonoBehaviour
{

    private Transform root;
    private float handDist;
    
    // Start is called before the first frame update
    void Start()
    {
        root = transform.root.GetChild(0);
        handDist = Vector3.Distance(transform.position, root.position);
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(root.position, root.forward, out RaycastHit hit, 1000);
        Vector3 hitToHand = Vector3.Normalize(hit.point - transform.position );

        transform.forward = hitToHand;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!root) root = transform.root.GetChild(0);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 100);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(root.position, root.forward * 100);
        
    }
#endif
    
}
