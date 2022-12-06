using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Weapons;
public class MassReflector : MonoBehaviour
{
    [SerializeField] private bool projectOnToPlane;
    
    private bool isOn;
    private bool loaded;
    private LaserWeaponVFX[] lasers;
    private Transform parent;
    private void Start()
    {
        parent = transform.GetChild(0);
        int count = parent.childCount;
        lasers = new LaserWeaponVFX[count];
        for (int i = 0; i < count; ++i)
            lasers[i] = parent.GetChild(i).GetComponent<LaserWeaponVFX>();
        parent.gameObject.SetActive(false);
    }

    public void Activate(Quaternion rot, Color c, LaserWeapon.ApplyToCharacter myDel)
    {
        if (projectOnToPlane)
            rot = Quaternion.Euler(0, rot.eulerAngles.y,0);
        parent.rotation = rot;
        
        
        isOn = true;
        if(loaded)return;
        loaded = true;
        parent.gameObject.SetActive(true);
        foreach (LaserWeaponVFX vfx in lasers)
        {
            vfx.Activate(c, myDel);
        }
    }

    private void LateUpdate()
    {
        //Try turning off every frame
        if (isOn)
        {
            isOn = false;
        }
        else
        {
            Deactivate();   
        }
    }

    private void Deactivate()
    {
        loaded = false;
        foreach (LaserWeaponVFX vfx in lasers)
        {
            vfx.DeActivate();
        }
        parent.gameObject.SetActive(false);
    }


#if UNITY_EDITOR
    [SerializeField] private LaserWeaponVFX laserPrefab;
    [SerializeField] private int lasersToMake;
    [SerializeField] private bool makeLasers;
    [SerializeField] private float fwdVec;
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * fwdVec);
        
        if (transform.childCount == 0)
            parent = Instantiate(new GameObject("RotationPoint DO NOT DELETE"), transform).transform;

        if (!parent)
            parent = transform.GetChild(0);
        
        if (makeLasers)
        {
            int count = parent.childCount;
            for(int i = 0; i < count; ++i)
                DestroyImmediate(parent.GetChild(0).gameObject);

            float degs = 360f / lasersToMake;
            for (int i = 0; i < lasersToMake; ++i)
            {
                Transform t = Instantiate(laserPrefab, parent.position,
                    Quaternion.Euler(0, degs * i + 1, 0), parent).transform;
                t.localPosition += t.forward * fwdVec;
                t.gameObject.SetActive(true);
            }
            makeLasers = false;
        }
    }


#endif
}
