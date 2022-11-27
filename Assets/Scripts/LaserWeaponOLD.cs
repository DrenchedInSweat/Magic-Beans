using Characters;
using UnityEngine;


public class LaserWeaponOLD : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform t = transform;
        Vector3 p = t.position;
        lineRenderer.SetPosition(0, p);
        CastLine(p, t.forward, 1, 200);
    }

    void CastLine(Vector3 prv, Vector3 direction, int idx, float remainingDist)
    {
        lineRenderer.positionCount = idx + 1;

        if (Physics.Raycast(prv, direction, out RaycastHit hit, remainingDist))
        {
            lineRenderer.SetPosition(idx, hit.point);
            if (hit.transform.CompareTag("Reflective"))
            {
                CastLine(hit.point, Vector3.Reflect(direction, hit.normal), idx+1, remainingDist - Vector3.Distance(prv, hit.point));
            }
            else if(hit.transform.TryGetComponent(out Character c))
            {
                //print("Damaging: " + c);
            }
        } 
        else
        {
            //print("Hitting nothing?");
            lineRenderer.SetPosition(idx, prv + direction * remainingDist);
        }
    }


}
