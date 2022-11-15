using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WayPoint : MonoBehaviour
{
    [SerializeField] private float minDist;
    [SerializeField] private float maxDist;
    [SerializeField] private Vector3 maxSize;
    [SerializeField] private Vector3 minSize;

    private int playerLayer;
    private Transform cam;
    private SpriteRenderer sr;
    Transform myTrans;

    private Color o;
    // Start is called before the first frame update
    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        myTrans = transform.GetChild(0);
        sr = myTrans.GetComponent<SpriteRenderer>();
        o = new Color(1,1,1,0);
        sr.rendererPriority = 100;
        sr.sortingOrder = 100;
    }

    private void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        float dist = Vector3.Distance(myTrans.position, cam.position);
        print("Dist:" + dist);
        //d5, m10, M20 0.25 - 0.5
        //d10, m10, M20 0.5 - 1
        //d25, m10, M20
        
        //25-10)
        
        sr.color = Color.Lerp(o, Color.white, Mathf.Clamp((dist - minDist) / maxDist, 0, 1));
        myTrans.localScale = Vector3.Lerp(minSize, maxSize, Mathf.Clamp((dist - minDist) / maxDist, 0, 1));
        myTrans.LookAt(cam);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            Destroy(gameObject);
        }
    }
}
