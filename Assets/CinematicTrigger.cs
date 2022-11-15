using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
//[ExecuteInEditMode]
public class CinematicTrigger : MonoBehaviour
{
    [SerializeField] private bool stopGravity;
    private int playerLayer;
    private CinemachineTrackedDolly cmv;
    private Transform child;
    private Collider col;
    private int pts;
    private CinemachineVirtualCamera old;
    private Rigidbody playerRb;
    private Vector3 origin;
    private Transform playerTrans;
    private Camera main;
    private CinemachineBrain b;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        child = transform.GetChild(0);
        pts = child.GetComponent<CinemachineSmoothPath>().m_Waypoints.Length;
        cmv = child.GetChild(0).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
        col = GetComponent<Collider>();
        main = Camera.main;
        b = main.GetComponent<CinemachineBrain>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print($"Collision {other.gameObject.layer} && {playerLayer} vs {LayerMask.NameToLayer("Player")} ");
        if (other.gameObject.layer == playerLayer)
        {
            playerTrans= other.transform;
            old = playerTrans.GetComponentInChildren<CinemachineVirtualCamera>();
            playerRb = playerTrans.GetComponent<Rigidbody>();
            //Turn on the track
            child.position = playerTrans.position;
            
            child.gameObject.SetActive(true);
            StartCoroutine(SlowTime());
            col.enabled = false;
        }
    }

   

    private IEnumerator SlowTime()
    {
        old.enabled = false;
        origin = playerRb.velocity;
        if (stopGravity)
        {
            playerRb.useGravity = false;
            origin.y = -9.81f;
        }

        yield return new WaitWhile(()=>!b.IsBlending);
        while (b.IsBlending)
        {
            playerRb.velocity = Vector3.Lerp(origin, Vector3.zero, b.ActiveBlend.TimeInBlend / b.ActiveBlend.Duration);
            child.position = playerRb.transform.position;
            yield return null;
        }
        StartCoroutine(BeginDeath());
    }
    
    private IEnumerator BeginDeath()
    {
        while (cmv.m_PathPosition < pts)
        {
            yield return null;
        }
       
        child.gameObject.SetActive(false);
        old.enabled = true;
        
        StartCoroutine(SpeedTime());
    }
    
    private IEnumerator SpeedTime()
    {
        yield return new WaitWhile(()=>!b.IsBlending);
        while (b.IsBlending)
        {
            playerRb.velocity = Vector3.Lerp(Vector3.zero, origin, b.ActiveBlend.TimeInBlend / b.ActiveBlend.Duration);
            yield return null;
        }
        
        playerTrans.GetChild(2).rotation = main.transform.rotation;
        playerRb.useGravity = true;
        print("Reached");
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(cmv.m_Path.EvaluatePosition(0), 1);
    }
#endif
    
}
