using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TeleportToPoint : MonoBehaviour
{
    [SerializeField] Transform returnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            other.gameObject.transform.position = returnPoint.position;
        }
    }

}
