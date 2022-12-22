using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class Slime : Enemy
{
    private int layerMask;

    private bool isElectrocuted;
    public void Electrocute()
    {
        print("Electric slime time!");
        isElectrocuted = true;
    }

    protected override void Awake()
    {
        base.Awake();
        layerMask = 1 << LayerMask.NameToLayer("PuzzlePiece");
    }

    protected override void TrueUpdate()
    {
        //base.TrueUpdate();

        if (isElectrocuted)
        {
            print("Checking: " + layerMask);
            Collider[] cols = new Collider[3];
            int c = Physics.OverlapSphereNonAlloc(transform.position, 5, cols, layerMask);
            if (c != 0)
            {
               
                while (--c >= 0)
                {
                    print("Detected Target: " + c);
                    if (cols[c].transform.TryGetComponent(out PuzzleSwitch sw))
                        sw.Activate();
                }
            }
        }
    }
    #if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(isElectrocuted)
            Gizmos.DrawSphere(transform.position, 5);
    }
    #endif
}
