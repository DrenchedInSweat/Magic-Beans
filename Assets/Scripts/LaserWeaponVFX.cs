using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

//This code actively makes start up take longer
public class LaserWeaponVFX : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)]
    private Color col;

    [SerializeField] private float delay = 10;
    [SerializeField] private float intensityMin;
    [SerializeField] private float intensityMax = 0.3f;
    private VisualEffect effect;
    private Light myLight;
    private Transform lightTransform;
    private readonly int delayID = Shader.PropertyToID("Delay");
    private readonly int lengthID = Shader.PropertyToID("Length");
    private readonly int startID = Shader.PropertyToID("StartFire");
    private readonly int stopID = Shader.PropertyToID("StopFire");

    private float initDelay;

    private float remainingLength = 150;
    private bool beamIsActive;

    // Start is called before the first frame update
    private void Awake()
    {
        effect = transform.GetChild(0).GetComponent<VisualEffect>();
        lightTransform = transform.GetChild(1);
        myLight = lightTransform.GetComponent<Light>();
        effect.SetVector4("Color", col);
        myLight.color = col;
        effect.SetFloat(delayID, delay);
        //effect.SetFloat(lengthID , 100);
        effect.SendEvent(startID);
        myLight.intensity = intensityMin;
        StartCoroutine(StartLight());
    }

    // Update is called once per frame
    private void Update()
    {
        if (!beamIsActive)
            return;
        //Start beam Color
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, remainingLength))
        {
            print("Hit Target");
            float dist  =hit.distance /2 ;
            effect.SetFloat(lengthID,dist);
            lightTransform.localPosition = new Vector3(0,0,dist);
            //myLight.
            if (hit.transform.CompareTag("Reflective"))
            {
                //CastLine(hit.point, Vector3.Reflect(direction, hit.normal), idx+1, remainingDist - Vector3.Distance(prv, hit.point));
            }
            else if (hit.transform.TryGetComponent(out Character c))
            {
                print("Damaging: " + c);
            }
        }
        else
        {
            print("Hitting nothing?");
            //lineRenderer.SetPosition(idx, prv + direction * remainingDist);
        }

    }

    private IEnumerator StartLight()
    {
        yield return new WaitForSeconds(delay);
        beamIsActive = true;
        float t = 0;
        while (t < delay)
        {
            t += Time.deltaTime;
            myLight.intensity = Mathf.Lerp(intensityMin, intensityMax, t/delay);
            yield return null;
        }

        
    }

}
