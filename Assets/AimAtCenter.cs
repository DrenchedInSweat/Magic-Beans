using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AimAtCenter : MonoBehaviour
{
    [SerializeField] private Image img;
    private Transform root;
    private float handDist;
    private int enemyLayer;
    private int prv;

    [SerializeField] private AimAtColor[] colors;
    
    
    // Start is called before the first frame update
    void Start()
    {
        root = transform.root.GetChild(0);
        handDist = Vector3.Distance(transform.position, root.position);
    }

    // Update is called once per frame
    void Update()
    {
        bool b = Physics.Raycast(root.position, root.forward, out RaycastHit hit, 1000);
        Vector3 hitToHand = Vector3.Normalize(hit.point - transform.position );
        transform.forward = hitToHand;
        if (!b) return;
        int n = 1 << hit.transform.gameObject.layer;
        if (n == prv) return;
        prv = n;
        foreach (AimAtColor c in colors)
        {
            if (n == c.layer)
            {
                img.color = c.color;
                return;
            }
        }
    }
    
    [Serializable]
    private struct AimAtColor
    {
        public LayerMask layer;
        public Color color; 
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
